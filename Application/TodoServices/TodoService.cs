﻿
using Application.DTOs.EventHandler;
using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Application.Events.Interfaces;
using Application.Factory;
using Application.ValidateDTO.ValidateTodo;
using Domain.DTOs;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using System.Collections.Concurrent;

namespace Application.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        private readonly ITodoFactory _factory;
        private readonly ITodoProcessingQueue _queue;
        private readonly IPublisher _publisher;

        // ------- MEMOIZATION CACHES -------
        // Para el filtro, guardaremos listas de DTO ya mapeadas:
        private static readonly ConcurrentDictionary<string, List<TodoResponseDTO>> _cacheFiltro
            = new ConcurrentDictionary<string, List<TodoResponseDTO>>();

        // Para el porcentaje de tareas completadas, clave fija "PorcentajeTareasCompletadas":
        private static readonly ConcurrentDictionary<string, double> _cachePorcentaje
            = new ConcurrentDictionary<string, double>();
        // ----------------------------------

        public TodoService(ITodoRepository todoRepository, ITodoFactory factory, ITodoProcessingQueue queue, IPublisher publisher)
        {
            _todoRepository = todoRepository;
            _factory = factory;
            _queue = queue;
            _publisher = publisher;
        }

        ValidateTodoDto ValidateTodoDto = new ValidateTodoDto();
        CreateTodoDtoValidator CreateTodoDtoValidator = new CreateTodoDtoValidator();
        UpdateTodoValidator updateTodoValidator = new UpdateTodoValidator();

        public async Task<Response<TodoResponseDTO>> GetTodoAllAsync(int userId)
        {
            var response = new Response<TodoResponseDTO>();

            try
            {
                var todos = await _todoRepository.GetAllAsync(userId);

                //Hacemos un select con linq a el todo que es un IEnumerable
                /*Select recibe una lambda sobre lo que queremos traer del objeto Todo
                Ej: t => new TodoResponseDTO { Id = t.Id, Title = t.Title}
                En vez de pasar los capos podemos pasarle un objeto o un objeto anonimo
                
                Ej: t => new Todo { 
                                t.Id, 
                                t.Title 
                                   }
                */
                //Acá estamos pasando un MapResponse que devuelve TodosResponseDTO
                // Y select va tomando los campos de cada Todo y los asigna a los campos del DTO
                //Le pasamos a select un metodo privado que asigna valores a las proiedades del nuevo dto que crea
                response.DataList = todos.Select(MapToResponseDto).ToList();
                response.Successful = true;
            }
            catch (Exception ex)
            {
                response.Successful = false;
                response.Errors.Add(ex.Message);
            }
            //Devolvemos la respuesta
            return response;
        }


        //public async Task<Response<TodoResponseDTO>> FilterTodoAsync(int? status, int? priority, string? title, DateTime? dueDate)
        //{


        //    var response = new Response<TodoResponseDTO>();
        //    var todos = await _todoRepository.filterTodoAsync(status, priority, title, dueDate);
        //    response.DataList = todos.Select(MapToResponseDto).ToList();

        //    return response;
        //}


        // ------ MÉTODO FILTER con MEMOIZATION ------
        public Task<Response<TodoResponseDTO>> FilterTodoAsync(int userId, int? status, int? priority, string? title, DateTime? dueDate)
        {
            // 1. Construir clave única a partir de todos los parámetros
            string cacheKey = $"Filtro:{userId}|{status}|{priority}|{title}|{dueDate:yyyy-MM-dd}";

            // 2. Obtener (o calcular+almacenar) la lista de DTO en _cacheFiltro
            var listaDto = _cacheFiltro.GetOrAdd(cacheKey, key =>
            {

                /*GetorAdd explicación: 
                 
                 Dime si entendí, .GetorAdd agrega un key y un value al 
                 diccionario si esa clave no existe. Retorna el valor 
                 nuevo o el valor existente si la clave ya existía. 
                 */

                // Si no existe aún en el diccionario, esto se ejecuta:
                // - Llamamos al repositorio para obtener dominio Todo
                // - Mapeamos a DTO
                // NOTA: Esto es síncrono dentro de GetOrAdd; el repositorio es async,
                //       así que bloqueamos con .Result (solo en este contexto de ejemplo).
                var todosDominio = _todoRepository.filterTodoAsync(userId, status, priority, title, dueDate).Result;
                return todosDominio.Select(MapToResponseDto).ToList();
            });

            // 3. Construir la Response a partir de la lista ya memorizada
            var response = new Response<TodoResponseDTO>
            {
                DataList = listaDto,
                Successful = true
            };
            return Task.FromResult(response);
        }
        // -------------------------------------------

        public async Task<Response<TodoResponseDTO>> GetTodoByIdAsync(int id, int userId)
        {
            var response = new Response<TodoResponseDTO>();
            try
            {
                // Usamos el GetbyIdAsync del Repositorio
                // y lo asignamos a la propiedad DataList de la respuesta
                // para así poder usarlo o enviarlo en el servicio
                var t = await _todoRepository.GetByIdAsync(id, userId);

                if (t is null)
                {
                    response.Successful = false;
                    response.Message = "El elemento no existe en la base de datos.";


                }
                else
                {

                    //**OJO** Usar un mejor Mapping mas centralizado o Automapper

                    response.SingleData = MapToResponseDto(t);
                    response.Successful = true;
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message) ;
            }
            //Devolvemos la respuesta
            return response;
        }
        
        public async Task<Response<string>> AddTodoAsync(Todo todo)
        {

            var errors = ValidateTodoDto.Validate(todo);

            var response = new Response<string>();

            // Si hay alugn error, se devuelve la respuesta con el error.
            if (errors.Any())
            {
                response.Successful = false;
                response.Message = "Se han encontrado los siguientes errores:";
                response.Errors = errors;
                return response;
            }

            try
            {
                //-----------------------------------------------------------------------------------------
                // Funcionamiento de async/await
                //-----------------------------------------------------------------------------------------

                // Usamos el AddAsync del Repositorio y almacenamos lo que retorna en result
                // Esperamos a que se complete la tarea asincrónica para seguir con lo demás que hay
                // en el metodo.
                // Sin el await, el código de AddTodoAsync continuaría ejecutándose sin esperar a que se complete la tarea.

                //Se libera el hilo por si se hacer otra operacion de Create tambien se pueda ejecutar
                // de manera simultánea, el await no bloquea otras peticiones.

                //Await las async/await tasks hacen que el programa espere hasta que las tareas se completen
                var result = await _todoRepository.AddAsync(todo);
                response.Message = result.Message;

                response.Successful = result.IsSucces;


                //-----------------------------------------------------------------------------------------
                // Funcionamiento de la cola de procesamiento "(RX.NET)"
                //-----------------------------------------------------------------------------------------


                // **Encolamos** el trabajo de procesar este Todo

                /*Tenemos un procesamiento asincrono de las tareas en el que no tenemos garantizado que todas
                 las peticiones de AddTodoAsync que se hagan se guarden al mismo tiempo, es posible que la segunda 
                petición se guarde antes que la primera*/

                //Si la tarea fue exitosa , la encolamos para luego hacer algo con el resultado como enviar un correo, notificar, etc.

                if (response.Successful) 
                {
                    // 1) Encolar
                    _queue.Enqueue(todo);


                    _cachePorcentaje.TryRemove("PorcentajeTareasCompletadas", out _);
                    _cachePorcentaje.TryRemove("PorcentajeTareasPendientes", out _);

                    // 2) Publicar evento de dominio
                    var evt = new TodoCreatedEvent(todo.Id, todo.Title, todo.CreatedAt);
                    //Console.WriteLine($"[TodoService] Publicando TaskCreatedEvent para tarea #{todo.Id}");
                    await _publisher.PublishAsync(evt);
                }

            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            //Enviamos la tarea a la cola de tareas


            //Devolvemos la respuesta
            return response;

        }

        public async Task<Response<string>> UpdateTodoAsync(int id, int userId, UpdateTodoRequestDto dto)
        {
            var response = new Response<string>();

            // 1. Recupera la entidad existente (sea null si no existe o no pertenece al usuario)
            var todo = await _todoRepository.GetByIdAsync(id, userId);
            if (todo == null)
            {
                response.Successful = false;
                response.Message = "El elemento no existe o no pertenece al usuario.";
                return response;
            }

            // 2. Validaciones básicas
            var errors = updateTodoValidator.Validate(dto);
            bool valid =
                !string.IsNullOrWhiteSpace(dto.Title)
                && (!dto.DueDate.HasValue || dto.DueDate.Value > DateTime.UtcNow);

            if (!valid || errors.Any())
            {
                response.Successful = false;
                response.Message = "Debe introducir un título y fecha válidos.";
                response.Errors = errors;
                return response;
            }

            // 3. Aplica los cambios a la entidad recuperada
            todo.Update(
                title: dto.Title,
                description: dto.Description,
                dueDate: dto.DueDate,
                additionalData: dto.AdditionalData,
                status: dto.Status,
                isdeleted: dto.IsDeleted,
                priority: dto.Priority);

            // 4. Persiste los cambios
            try
            {
                var result = await _todoRepository.UpdateAsync(todo);
                response.Successful = result.IsSucces;
                response.Message = result.Message;

                if (response.Successful)
                {
                    // Encolar trabajo adicional
                    _queue.Enqueue(todo);
                    _cachePorcentaje.TryRemove("PorcentajeTareasCompletadas", out _);
                    _cachePorcentaje.TryRemove("PorcentajeTareasPendientes", out _);
                }
            }
            catch (Exception ex)
            {
                response.Successful = false;
                response.Message = "Error al actualizar el elemento en la base de datos.";
                response.Errors.Add(ex.Message);
            }

            return response;
        }



        //public async Task<double> ContarTareasCompletadasAsync()
        //{
        //    return await _todoRepository.ContarTareasCompletadasAsync();
        //}

        public async Task<double> ContarTareasCompletadasAsync(int userId)
        {
            const string cacheKey = "PorcentajeTareasCompletadas";

            // 1. Si existe en caché, devuélvelo de inmediato
            if (_cachePorcentaje.TryGetValue(cacheKey, out double cachedValue))
                return cachedValue;

            // 2. Si no hay valor en caché, llama al repositorio de forma asíncrona
            double porcentaje = await _todoRepository.ContarTareasCompletadasAsync(userId);

            // 3. Guarda en caché y retorna
            _cachePorcentaje[cacheKey] = porcentaje;
            return porcentaje;
        }


        public async Task<double> ContarTareasPendientesAsync(int userId) 
        {
            const string cacheKey = "PorcentajeTareasPendientes";

            // 1. Si existe en caché, devuélvelo de inmediato
            if (_cachePorcentaje.TryGetValue(cacheKey, out double cachedValue))
                return cachedValue;

            // 2. Si no hay valor en caché, llama al repositorio de forma asíncrona
            double porcentaje = await _todoRepository.ContarTareasPendientesAsync(userId);

            // 3. Guarda en caché y retorna
            _cachePorcentaje[cacheKey] = porcentaje;
            return porcentaje;
        }


        public async Task<Response<string>> DeleteTodoAsync(int id, int userId)
        {
            var response = new Response<string>();

            // 1. Recupera la entidad para poder encolarla luego
            var todo = await _todoRepository.GetByIdAsync(id, userId);
            if (todo is null)
            {
                response.Successful = false;
                response.Message = "El elemento no existe.";
                return response;
            }

            try
            {
                // 2. Elimina en el repositorio
                var result = await _todoRepository.DeleteAsync(id);
                response.Message = result.Message;
                response.Successful = result.IsSucces;

                // 3. Encola solo si la eliminación fue exitosa
                if (response.Successful)
                {
                    _queue.Enqueue(todo);
                    _cachePorcentaje.TryRemove("PorcentajeTareasCompletadas", out _);

                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }


        public async Task<Response<string>> DeleteSoftTodoAsync(int id, int userId)
        {
            var response = new Response<string>();

            // 1. Recupera la entidad para poder encolarla luego
            var todo = await _todoRepository.GetByIdAsync(id, userId);
            if (todo is null)
            {
                response.Successful = false;
                response.Message = "El elemento no existe.";
                return response;
            }

            try
            {
                // 2. Soft delete en el repositorio
                var result = await _todoRepository.DeleteSoftAsync(id);
                response.Message = result.Message;
                response.Successful = result.IsSucces;

                // 3. Encola solo si la eliminación fue exitosa
                if (response.Successful)
                {
                    _queue.Enqueue(todo);
                    _cachePorcentaje.TryRemove("PorcentajeTareasCompletadas", out _);
                    _cachePorcentaje.TryRemove("PorcentajeTareasPendientes", out _);
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<Response<string>> AddHighPriorityTodoAsync(CreateTodoRequestDto dto)
        {
            var errors = CreateTodoDtoValidator.Validate(dto);
            var response = new Response<string>();


            if (errors.Any())
            {
                response.Successful = false;
                response.Message = "Esta tarea ya existe en la base de datos";
                response.Errors = errors;
                return response;
            }


            try
            {
                var todo = _factory.CreateHighPriority(dto);
                var result = await _todoRepository.AddAsync(todo);
                response.Message = result.Message;
                response.Successful = result.IsSucces;

                // **Encolamos** el trabajo de procesar este Todo
                if (response.Successful)
                    _queue.Enqueue(todo);
                _cachePorcentaje.TryRemove("PorcentajeTareasCompletadas", out _);
                _cachePorcentaje.TryRemove("PorcentajeTareasPendientes", out _);

            }
            catch (ArgumentException ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }
            return response;
        }

        public async Task<Response<string>> AddMediumPriorityTodoAsync(CreateTodoRequestDto dto)
        {
            var errors = CreateTodoDtoValidator.Validate(dto);
            var response = new Response<string>();


            if (errors.Any())
            {
                response.Successful = false;
                response.Message = "Esta tarea ya existe en la base de datos";
                response.Errors = errors;
                return response;
            }

            try
            {
                var todo = _factory.CreateMediumPriority(dto);
                var result = await _todoRepository.AddAsync(todo);
                response.Message = result.Message;
                response.Successful = result.IsSucces;

                // **Encolamos** el trabajo de procesar este Todo
                if (response.Successful)
                    _queue.Enqueue(todo);
                _cachePorcentaje.TryRemove("PorcentajeTareasCompletadas", out _);
                _cachePorcentaje.TryRemove("PorcentajeTareasPendientes", out _);

            }
            catch (ArgumentException ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }
            return response;
        }

        public async Task<Response<string>> AddLowPriorityTodoAsync(CreateTodoRequestDto dto)
        {
            var errors = CreateTodoDtoValidator.Validate(dto);
            var response = new Response<string>();


            if (errors.Any())
            {
                response.Successful = false;
                response.Message = "Esta tarea ya existe en la base de datos";
                response.Errors = errors;
                return response;
            }

            try
            {
                var todo = _factory.CreateLowPriority(dto);
                var result = await _todoRepository.AddAsync(todo);
                response.Message = result.Message;
                response.Successful = result.IsSucces;

                // **Encolamos** el trabajo de procesar este Todo
                if (response.Successful)
                    _queue.Enqueue(todo);
                _cachePorcentaje.TryRemove("PorcentajeTareasCompletadas", out _);
                _cachePorcentaje.TryRemove("PorcentajeTareasPendientes", out _);

            }
            catch (ArgumentException ex)
            {
                response.Successful = false;
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }
            return response;
        }

        //----------------------------------------------------------------------------------------------
        //Metodo privados para mapear los objetos de respuesta
        //----------------------------------------------------------------------------------------------

        //Este es un metodo privado que devuelve un TodoResponseDTO
        // Crea un nuevo objeto TodoResponseDTO y asigna las propiedades del objeto Todo a las propiedades del DTO.
        private TodoResponseDTO MapToResponseDto(Todo t) => new TodoResponseDTO
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsDeleted = t.IsDeleted,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            Status = t.Status,
            Priority = t.Priority,
            AdditionalData = t.AdditionalData,
            DaysRemaining = CalcDaysRemaining(t)
        };


        // Func que calcula días restantes de manera reutilizable
        private static readonly Func<Todo, int> CalcDaysRemaining = todo =>
            todo.DueDate.HasValue
                ? (int)Math.Max(0, (todo.DueDate.Value.Date - DateTime.UtcNow.Date).TotalDays)
                : 0;


    }  

}
