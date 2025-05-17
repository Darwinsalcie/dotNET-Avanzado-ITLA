using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Domain.DTOs;
using Domain.Entities;


namespace Application.Interfaces
{
    public interface ITodoService
    {
        Task<Response<TodoResponseDTO>> GetTodoAllAsync();
        Task<Response<TodoResponseDTO>> GetTodoByIdAsync(int id);

        Task<Response<string>> AddTodoAsync(Todo request);
        Task<Response<string>> UpdateTodoAsync(Todo request);
        Task<Response<string>> DeleteTodoAsync(int id);
    }

}

