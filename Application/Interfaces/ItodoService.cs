using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Domain.DTOs;
using Domain.Entities;


namespace Application.Interfaces
{
    public interface ITodoService
    {
        Task<Response<Todo>> GetTodoAllAsync();
        Task<Response<Todo>> GetTodoByIdAsync(int id);
        // … resto de métodos
    }

}

