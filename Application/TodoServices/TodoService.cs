

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

        public async Task<Response<TodoResponseDTO>> GetTodoAllAsync()
        {
            var response = new Response<TodoResponseDTO>();

            try
            {

                var todos = await _todoRepository.GetAllAsync();
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
        public Task<Response<TodoResponseDTO>> FilterTodoAsync(int? status, int? priority, string? title, DateTime? dueDate)
        {
            // 1. Construir clave única a partir de todos los parámetros
            string cacheKey = $"Filtro:{status}|{priority}|{title}|{dueDate:yyyy-MM-dd}";

            // 2. Obtener (o calcular+almacenar) la lista de DTO en _cacheFiltro
            var listaDto = _cacheFiltro.GetOrAdd(cacheKey, key =>
            {
                // Si no existe aún en el diccionario, esto se ejecuta:
                // - Llamamos al repositorio para obtener dominio Todo
                // - Mapeamos a DTO
                // NOTA: Esto es síncrono dentro de GetOrAdd; el repositorio es async,
                //       así que bloqueamos con .Result (solo en este contexto de ejemplo).
                var todosDominio = _todoRepository.filterTodoAsync(status, priority, title, dueDate).Result;
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

        public async Task<Response<TodoResponseDTO>> GetTodoByIdAsync(int id)
        {
            var response = new Response<TodoResponseDTO>();
            try
            {
                // Usamos el GetbyIdAsync del Repositorio
                // y lo asignamos a la propiedad DataList de la respuesta
                // para así poder usarlo o enviarlo en el servicio
                var t = await _todoRepository.GetByIdAsync(id);

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
                response.Message = "La fecha de vencimiento no puede ser anterior a la fecha actual.";
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

                    // 2) Publicar evento de dominio
                    var evt = new TodoCreatedEvent(todo.Id, todo.Title, todo.CreatedAt);
                    Console.WriteLine($"[TodoService] Publicando TaskCreatedEvent para tarea #{todo.Id}");
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
        public async Task<Response<string>> UpdateTodoAsync(Todo todo, int id)
        {
            var response = new Response<string>();

            var t = await _todoRepository.GetByIdAsync(id);
            if (t is null)
            {
                response.Successful = false;
                response.Message = "El elemento no existe en la base de datos.";

                //Al usar return el resto del codigo solo se ejecuta si no se entra en el if
                return response;
            }


            Func<Todo, bool> validate = todo =>
            !string.IsNullOrEmpty(todo.Title)
            && todo.DueDate.HasValue && todo.DueDate > DateTime.UtcNow;

            if (!validate(todo))
            {
                response.Successful = false;
                response.Message = "La fecha de vencimiento no puede ser anterior a la fecha actual.";
                return response;
            }

            else 
            {

                try
                {
                    // Usamos el UpdateAsync del Repositorio
                    // y lo asignamos a la propiedad DataList de la respuesta
                    // para así poder usarlo o enviarlo en el servicio
                    var result = await _todoRepository.UpdateAsync(todo);
                    response.Message = result.Message;

                    response.Successful = result.IsSucces;

                    // **Encolamos** el trabajo de procesar este Todo
                    if (response.Successful)
                        _queue.Enqueue(todo);
                    _cachePorcentaje.TryRemove("PorcentajeTareasCompletadas", out _);

                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }
                //Devolvemos la respuesta
                return response;
            }

        }

        //public async Task<double> ContarTareasCompletadasAsync()
        //{
        //    return await _todoRepository.ContarTareasCompletadasAsync();
        //}
        public async Task<double> ContarTareasCompletadasAsync()
        {
            const string cacheKey = "PorcentajeTareasCompletadas";

            // 1. Si existe en caché, devuélvelo de inmediato
            if (_cachePorcentaje.TryGetValue(cacheKey, out double cachedValue))
                return cachedValue;

            // 2. Si no hay valor en caché, llama al repositorio de forma asíncrona
            double porcentaje = await _todoRepository.ContarTareasCompletadasAsync();

            // 3. Guarda en caché y retorna
            _cachePorcentaje[cacheKey] = porcentaje;
            return porcentaje;
        }


        public async Task<double> ContarTareasPendientesAsync() 
        {
            const string cacheKey = "PorcentajeTareasPendientes";

            // 1. Si existe en caché, devuélvelo de inmediato
            if (_cachePorcentaje.TryGetValue(cacheKey, out double cachedValue))
                return cachedValue;

            // 2. Si no hay valor en caché, llama al repositorio de forma asíncrona
            double porcentaje = await _todoRepository.ContarTareasPendientesAsync();

            // 3. Guarda en caché y retorna
            _cachePorcentaje[cacheKey] = porcentaje;
            return porcentaje;
        }


        public async Task<Response<string>> DeleteTodoAsync(int id)
        {
            var response = new Response<string>();

            // 1. Recupera la entidad para poder encolarla luego
            var todo = await _todoRepository.GetByIdAsync(id);
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

        public async Task<Response<string>> AddHighPriorityTodoAsync(CreateTodoRequestDto dto)
        {
            var response = new Response<string>();
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
            var response = new Response<string>();
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
            var response = new Response<string>();
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
        private TodoResponseDTO MapToResponseDto(Todo t) => new TodoResponseDTO
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            IsCompleted = t.IsCompleted,
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
