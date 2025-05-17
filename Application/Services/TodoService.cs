

using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Application.Interfaces;
namespace Application.Services
{
    public class TodoService : ITodoService
    {
        private readonly IGenericRepository<Todo> _repository;

        public TodoService(IGenericRepository<Todo> repository)
        {
            _repository = repository;
        }

        public async Task<Response<Todo>> GetTodoAllAsync()
        {
            var response = new Response<Todo>();
            try
            {
                // Usamos el GetAllAsyn del Repositorio
                // y lo asignamos a la propiedad DataList de la respuesta
                // para así poder usarlo o enviarlo en el servicio
                response.DataList = await _repository.GetAllAsync();
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

        public async Task<Response<Todo>> GetTodoByIdAsync(int id)
        {
            var response = new Response<Todo>();
            try
            {
                // Usamos el GetbyIdAsync del Repositorio
                // y lo asignamos a la propiedad DataList de la respuesta
                // para así poder usarlo o enviarlo en el servicio
                var result = await _repository.GetByIdAsync(id);

                if (result != null)
                {
                    response.SingleData = result;
                    response.Successful = true;

                }
                else
                {
                    response.Successful = false;
                    response.Message = "No se encontró el elemento con el Id proporcionado.";
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

        public async Task<Response<string>> UpdateTodoAsync(Todo todo)
        {
            var response = new Response<string>();
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
