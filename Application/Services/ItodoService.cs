using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Domain.DTOs;
using Domain.Entities;


namespace Application.Services
{
    public interface ITodoService
    {
        Task<Response<TodoResponseDTO>> GetTodoAllAsync();
        Task<Response<TodoResponseDTO>> GetTodoByIdAsync(int id);
        Task<Response<TodoResponseDTO>> FilterTodoAsync(int? status, int? priority, string? title, DateTime? dueDate );
        Task<Response<string>> AddTodoAsync(Todo request);
        Task<Response<string>> UpdateTodoAsync(Todo request, int Id);
        Task<Response<string>> DeleteTodoAsync(int id);
        Task<double> ContarTareasCompletadasAsync();
        Task<double> ContarTareasPendientesAsync();
        Task<Response<string>> AddHighPriorityTodoAsync(CreateTodoRequestDto dto);
        Task<Response<string>> AddMediumPriorityTodoAsync(CreateTodoRequestDto dto);
        Task<Response<string>> AddLowPriorityTodoAsync(CreateTodoRequestDto dto);

    }

}

