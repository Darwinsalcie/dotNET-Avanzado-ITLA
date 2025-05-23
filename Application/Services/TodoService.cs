

using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Application.Events.Interfaces;
using Application.Factory;
using Application.ValidateDTO.ValidateTodo;
using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class TodoService : ITodoService
    {
        private readonly IGenericRepository<Todo> _repository;
        private readonly ITodoFactory _factory;
        private readonly ITodoProcessingQueue _queue;
        public TodoService(IGenericRepository<Todo> repository, ITodoFactory factory, ITodoProcessingQueue queue)
        {
            _repository = repository;
            _factory = factory;
            _queue = queue;
        }

        ValidateTodoDto ValidateTodoDto = new ValidateTodoDto();

        public async Task<Response<TodoResponseDTO>> GetTodoAllAsync()
        {
            var response = new Response<TodoResponseDTO>();

            try
            {

                var todos = await _repository.GetAllAsync();
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

        public async Task<Response<TodoResponseDTO>> GetTodoByIdAsync(int id)
        {
            var response = new Response<TodoResponseDTO>();
            try
            {
                // Usamos el GetbyIdAsync del Repositorio
                // y lo asignamos a la propiedad DataList de la respuesta
                // para así poder usarlo o enviarlo en el servicio
                var t = await _repository.GetByIdAsync(id);

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
            var response = new Response<string>();

            var errors = ValidateTodoDto.Validate(todo);

            if (errors.Any())
            {
                response.Successful = false;
                response.Message = "La fecha de vencimiento no puede ser anterior a la fecha actual.";
                return response;
            }

            try
            {

                // Usamos el AddAsync del Repositorio
                // y lo asignamos a la propiedad DataList de la respuesta
                // para así poder usarlo o enviarlo en el servicio
                var result = await _repository.AddAsync(todo);
                response.Message = result.Message;

                response.Successful = result.IsSucces;

                // **Encolamos** el trabajo de procesar este Todo
                if (response.Successful)
                    _queue.Enqueue(todo);

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

            var t = await _repository.GetByIdAsync(id);
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
                    var result = await _repository.UpdateAsync(todo);
                    response.Message = result.Message;

                    response.Successful = result.IsSucces;

                    // **Encolamos** el trabajo de procesar este Todo
                    if (response.Successful)
                        _queue.Enqueue(todo);
                }
                catch (Exception ex)
                {
                    response.Errors.Add(ex.Message);
                }
                //Devolvemos la respuesta
                return response;
            }

        }

        public async Task<Response<string>> DeleteTodoAsync(int id)
        {
            var response = new Response<string>();

            // 1. Recupera la entidad para poder encolarla luego
            var todo = await _repository.GetByIdAsync(id);
            if (todo is null)
            {
                response.Successful = false;
                response.Message = "El elemento no existe.";
                return response;
            }

            try
            {
                // 2. Elimina en el repositorio
                var result = await _repository.DeleteAsync(id);
                response.Message = result.Message;
                response.Successful = result.IsSucces;

                // 3. Encola solo si la eliminación fue exitosa
                if (response.Successful)
                {
                    _queue.Enqueue(todo);
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
                var result = await _repository.AddAsync(todo);
                response.Message = result.Message;
                response.Successful = result.IsSucces;

                // **Encolamos** el trabajo de procesar este Todo
                if (response.Successful)
                    _queue.Enqueue(todo);
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
                var result = await _repository.AddAsync(todo);
                response.Message = result.Message;
                response.Successful = result.IsSucces;

                // **Encolamos** el trabajo de procesar este Todo
                if (response.Successful)
                    _queue.Enqueue(todo);
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
                var result = await _repository.AddAsync(todo);
                response.Message = result.Message;
                response.Successful = result.IsSucces;

                // **Encolamos** el trabajo de procesar este Todo
                if (response.Successful)
                    _queue.Enqueue(todo);
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
        //Metod privados para mapear los objetos de respuesta
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
