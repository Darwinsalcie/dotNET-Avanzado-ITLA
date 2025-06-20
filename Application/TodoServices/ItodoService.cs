using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Domain.DTOs;
using Domain.Entities;


namespace Application.Services
{
    public interface ITodoService
    {
        Task<Response<TodoResponseDTO>> GetTodoAllAsync(int userId);
        Task<Response<TodoResponseDTO>> GetTodoByIdAsync(int id, int userId);
        Task<Response<TodoResponseDTO>> FilterTodoAsync(int userId, int? status, int? priority, string? title, DateTime? dueDate);
        Task<Response<string>> AddTodoAsync(Todo request);
        Task<Response<string>> UpdateTodoAsync(int id, int userId, UpdateTodoRequestDto dto);
        Task<Response<string>> DeleteTodoAsync(int id, int userId);
        Task<Response<string>> DeleteSoftTodoAsync(int id, int userId);
        Task<double> ContarTareasCompletadasAsync(int userId);
        Task<double> ContarTareasPendientesAsync(int userId);
        Task<Response<string>> AddHighPriorityTodoAsync(CreateTodoRequestDto dto);
        Task<Response<string>> AddMediumPriorityTodoAsync(CreateTodoRequestDto dto);
        Task<Response<string>> AddLowPriorityTodoAsync(CreateTodoRequestDto dto);

    }

}

