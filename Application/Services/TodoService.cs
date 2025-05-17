

using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
using Application.DTOs.Response;
namespace Application.Services
{
    public class TodoService : ITodoService
    {
        private readonly IGenericRepository<Todo> _repository;

        public TodoService(IGenericRepository<Todo> repository)
        {
            _repository = repository;
        }


        // Func que calcula días restantes de manera reutilizable
        private static readonly Func<Todo, int> CalcDaysRemaining = todo =>
            todo.DueDate.HasValue
                ? (int)Math.Max(0, (todo.DueDate.Value.Date - DateTime.UtcNow.Date).TotalDays)
                : 0;


        public async Task<Response<TodoResponseDTO>> GetTodoAllAsync()
        {
            var response = new Response<TodoResponseDTO>();
            try
            {

                var todos = await _repository.GetAllAsync();

                response.DataList = todos.Select(t => new TodoResponseDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    DueDate = t.DueDate,
                    DaysRemaining = CalcDaysRemaining(t)
                }).ToList();

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

                    //**OJO** Usar Mapping para mapear el objeto y Calcular los días restantes
                    // dentro del objeto de respuesta
                    response.SingleData = new TodoResponseDTO
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        IsCompleted = t.IsCompleted,
                        DueDate = t.DueDate,
                        DaysRemaining = CalcDaysRemaining(t)
                    };
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

            Func<Todo, bool> validate = todo => 
            !string.IsNullOrEmpty(todo.Title)
            && todo.DueDate.HasValue && todo.DueDate > DateTime.UtcNow;

            if (!validate(todo))
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
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }
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
            try
            {
                // Usamos el DeleteAsync del Repositorio
                // y lo asignamos a la propiedad DataList de la respuesta
                // para así poder usarlo o enviarlo en el servicio
                var result = await _repository.DeleteAsync(id);
                response.Message = result.Message;


                response.Successful = result.IsSucces;
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }
            //Devolvemos la respuesta
            return response;
        }



    }

}
