using API.Controllers;
using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Application.Services;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace UnitTesting
{
    public class TodoBusinessLogicTests
    {
        [Fact]
        public async Task GetTodoAllAsync_ReturnsAllTodos()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.GetTodoAllAsync(1))
                .ReturnsAsync(new Response<TodoResponseDTO> { Successful = true, DataList = new List<TodoResponseDTO>() });

            var controller = new TodoController(todoServiceMock.Object);

            // Simula usuario autenticado con userId = 1
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin") // Simula un rol de usuario
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.GetTodoAllAsync();

            Assert.NotNull(result.Value);
            Assert.True(result.Value!.Successful);
            todoServiceMock.Verify(s => s.GetTodoAllAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithExistingId_ReturnsTodo()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.GetTodoByIdAsync(1,1))
                .ReturnsAsync(new Response<TodoResponseDTO> { Successful = true, SingleData = new TodoResponseDTO() });

            var controller = new TodoController(todoServiceMock.Object);

            // Simula usuario autenticado con userId = 1
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.GetTodoByIdAsync(1);

            Assert.NotNull(result.Value);
            Assert.True(result.Value!.Successful);
            todoServiceMock.Verify(s => s.GetTodoByIdAsync(1, 1), Times.Once);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithNonExistingId_ReturnsError()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.GetTodoByIdAsync(99,1))
                .ReturnsAsync(new Response<TodoResponseDTO> { Successful = false, Errors = new() { "No encontrado" } });

            var controller = new TodoController(todoServiceMock.Object);


            // Simula usuario autenticado con userId = 1
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.GetTodoByIdAsync(99);

            Assert.NotNull(result.Value);
            Assert.False(result.Value!.Successful);
            Assert.Contains("No encontrado", result.Value.Errors);
        }

        [Fact]
        public async Task FilterTodoAsync_ByStatusAndPriority_ReturnsFiltered()
        {
            var todoServiceMock = new Mock<ITodoService>();
            // El userId simulado será 1
            int userId = 1;
            int? status = 2;
            int? priority = null;
            string? title = null;
            DateTime? dueDate = null;

            todoServiceMock.Setup(s => s.FilterTodoAsync(userId, status, priority, title, dueDate))
                .ReturnsAsync(new Response<TodoResponseDTO> { Successful = true, DataList = new List<TodoResponseDTO>() });

            var controller = new TodoController(todoServiceMock.Object);

            // Simula usuario autenticado con userId = 1
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.GetTodobyFilter(status, priority, title, dueDate);

            Assert.NotNull(result.Value);
            Assert.True(result.Value!.Successful);
            todoServiceMock.Verify(s => s.FilterTodoAsync(userId, status, priority, title, dueDate), Times.Once);
        }

        [Fact]
        public async Task CreateHighPriorityTodoAsync_ReturnsSuccess()
        {
            var todoServiceMock = new Mock<ITodoService>();
            var dto = new CreateTodoRequestDto { Title = "Alta", Description = "Prueba" };
            todoServiceMock.Setup(s => s.AddHighPriorityTodoAsync(dto))
                .ReturnsAsync(new Response<string> { Successful = true });

            var controller = new TodoController(todoServiceMock.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1")
                    }, "mock"))
                }
            };

            var result = await controller.CreateHigh(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<string>>(okResult.Value);
            Assert.True(response.Successful);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithExistingId_ReturnsSuccess()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.DeleteTodoAsync(1, It.IsAny<int>()))
                .ReturnsAsync(new Response<string> { Successful = true, Message = "Tarea eliminado correctamente." });

            var controller = new TodoController(todoServiceMock.Object);

            // Simula usuario autenticado con rol Admin
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await controller.DeleteTodoAsync(1);

            Assert.NotNull(result.Value);
            Assert.True(result.Value!.Successful);
            todoServiceMock.Verify(s => s.DeleteTodoAsync(1, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetPorcentajeCompletadas_ReturnsCorrectValue()
        {
            // Arrange
            int userId = 1;
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.ContarTareasCompletadasAsync(userId))
                .ReturnsAsync(75.0);

            var controller = new TodoController(todoServiceMock.Object);

            // Simula usuario autenticado con userId = 1
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.GetPorcentajeCompletadas();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Aquí, okResult.Value es un objeto anónimo, así que usa reflection o dynamic
            var porcentaje = okResult.Value.GetType().GetProperty("PorcentajeCompletadas")?.GetValue(okResult.Value, null);
            Assert.Equal(75.0, (double)porcentaje);

            todoServiceMock.Verify(s => s.ContarTareasCompletadasAsync(userId), Times.Once);
        }
    }
}
