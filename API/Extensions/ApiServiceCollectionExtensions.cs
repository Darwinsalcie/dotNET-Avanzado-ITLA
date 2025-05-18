using Application.Factory;
using Application.Services;
using Application.ValidateDTO.ValidateTodo;
using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.Repositories;

namespace API.Extensions
{
    public static class ApiServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {

            // Repositorio genérico (Scoped)
            services.AddScoped(typeof(IGenericRepository<Todo>), typeof(TodoRepository));

            // Validación de DTOs
            services.AddScoped<CreateTodoDtoValidator>();

            //Fabrica de entidades
            services.AddScoped<ITodoFactory, TodoFactory>();

            // Servicios de aplicación 
            services.AddScoped<ITodoService, TodoService>();

            //// Registrar repositorio (ajusta según tu implementación)
            //services.AddScoped<ITodoRepository<string>, TodoRepository<string>>();

            //// Validación (Scoped)
            //services.AddScoped<EntityValidator<Todo>>(_ =>
            //    todo => !string.IsNullOrWhiteSpace(todo.Title)
            //            && (!todo.DueDate.HasValue || todo.DueDate > DateTime.UtcNow)
            //);

            // Notificación (Scoped)
            services.AddScoped<Action<Todo>>(_ =>
                todo => Console.WriteLine($"[Notification] Todo #{todo.Id} – {todo.Title}")
            );




            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
    }
}
