using Application.DTOs.Security;
using Application.Events.Interfaces;
using Application.Factory;
using Application.Services;
using Application.ValidateDTO.ValidateTodo;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Security;
using Infraestructure.Repositories;
using Infrastructure.Events;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reactive.Concurrency;
using System.Text;

namespace API.Extensions
{
    public static class ApiServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            //Servicios de Auentication y Authorization
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthRepository, AuthRepository>();

            // Configuracion JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]!)),
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // 1) Registrar el repositorio específico de Todo
            services.AddScoped<ITodoRepository, TodoRepository>();


            // 2) (Opcional) También se puede seguir registrando la interfaz genérica, 
            //    en caso de que en algún otro lugar quiera inyectar IGenericRepository<Todo>
            services.AddScoped<IGenericRepository<Todo>, TodoRepository>();

            // Validación de DTOs
            services.AddScoped<CreateTodoDtoValidator>();

            //Fabrica de entidades
            services.AddScoped<ITodoFactory, TodoFactory>();



            // Registrar cola de procesamiento Rx como Singleton
            services.AddSingleton<ITodoProcessingQueue>(provider =>
                new TodoProcessingQueue(
                    subscribeOnScheduler: TaskPoolScheduler.Default,
                    observeOnScheduler: TaskPoolScheduler.Default));

           
            
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

            return services;
        }
    }
}
