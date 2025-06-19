using API.Controllers;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Moq;
using Application.DTOs.Response;
using Domain.DTOs;
namespace UnitTesting
{
    public class TodoControllerTests
    {
        private readonly TodoController _todoController;
        private readonly Mock<ITodoService> _mockTodoService;
        public TodoControllerTests()
        {
            // 1. Mock del servicio
            _mockTodoService = new Mock<ITodoService>();

            // 2. Crear el controlador con el servicio mockeado
            _todoController = new TodoController(_mockTodoService.Object);

            // 3. Simular autenticación (si el controlador usa [Authorize])
            _todoController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, "testuser"),
                        new Claim(ClaimTypes.Role, "Admin")
                    }, "mock"))
                }
            };
        }

        [Fact]
        public async Task Get_Ok()
        {
            // 4. Configurar el mock para devolver datos de prueba
            _mockTodoService.Setup(s => s.GetTodoAllAsync())
                .ReturnsAsync(new Response<TodoResponseDTO>
                {
                    DataList = new List<TodoResponseDTO>(),
                    Successful = true
                });

            // 5. Act: Llamar al método del controlador (usa "await" si es async)
            var actionResult = await _todoController.GetTodoAllAsync();
            Assert.NotNull(actionResult.Value);
            Assert.IsType<Response<TodoResponseDTO>>(actionResult.Value);
        }

        [Fact]
        public async Task Get_Quantity() 
        {
            int id = 1;

            // Configura el mock para devolver un solo elemento en SingleData y DataList vacío
            //Setup
            _mockTodoService.Setup(s => s.GetTodoByIdAsync(id))
                .ReturnsAsync(new Response<TodoResponseDTO>
                {
                    SingleData = new TodoResponseDTO { Id = id, Title = "Test" },
                    DataList = new List<TodoResponseDTO>(), // Debe estar vacío
                    Successful = true
                });

            var result = await _todoController.GetTodoByIdAsync(id);
            Assert.NotNull(result);

            var response = Assert.IsType<Response<TodoResponseDTO>>(result.Value);

            // Asegura que DataList esté vacío
            Assert.Empty(response.DataList);

            // Asegura que SingleData no sea null y tenga el id correcto
            Assert.NotNull(response.SingleData);
            Assert.Equal(id, response.SingleData.Id);
        }
    }
}