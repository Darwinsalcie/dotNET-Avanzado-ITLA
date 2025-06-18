using API.Controllers;
using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Application.Services;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace UnitTesting
{
    public class TodoBusinessLogicTests
    {
        [Fact]
        public async Task GetTodoAllAsync_ReturnsAllTodos()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.GetTodoAllAsync())
                .ReturnsAsync(new Response<TodoResponseDTO> { Successful = true, DataList = new List<TodoResponseDTO>() });

            var controller = new TodoController(todoServiceMock.Object);

            var result = await controller.GetTodoAllAsync();

            Assert.NotNull(result.Value);
            Assert.True(result.Value!.Successful);
            todoServiceMock.Verify(s => s.GetTodoAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithExistingId_ReturnsTodo()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.GetTodoByIdAsync(1))
                .ReturnsAsync(new Response<TodoResponseDTO> { Successful = true, SingleData = new TodoResponseDTO() });

            var controller = new TodoController(todoServiceMock.Object);

            var result = await controller.GetTodoByIdAsync(1);

            Assert.NotNull(result.Value);
            Assert.True(result.Value!.Successful);
            todoServiceMock.Verify(s => s.GetTodoByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithNonExistingId_ReturnsError()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.GetTodoByIdAsync(99))
                .ReturnsAsync(new Response<TodoResponseDTO> { Successful = false, Errors = new() { "No encontrado" } });

            var controller = new TodoController(todoServiceMock.Object);

            var result = await controller.GetTodoByIdAsync(99);

            Assert.NotNull(result.Value);
            Assert.False(result.Value!.Successful);
            Assert.Contains("No encontrado", result.Value.Errors);
        }

        [Fact]
        public async Task FilterTodoAsync_ByStatusAndPriority_ReturnsFiltered()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.FilterTodoAsync(1, 2, null, null))
                .ReturnsAsync(new Response<TodoResponseDTO> { Successful = true, DataList = new List<TodoResponseDTO>() });

            var controller = new TodoController(todoServiceMock.Object);

            var result = await controller.GetTodobyFilter(1, 2, null, null);

            Assert.NotNull(result.Value);
            Assert.True(result.Value!.Successful);
            todoServiceMock.Verify(s => s.FilterTodoAsync(1, 2, null, null), Times.Once);
        }

        [Fact]
        public async Task CreateHighPriorityTodoAsync_ReturnsSuccess()
        {
            var todoServiceMock = new Mock<ITodoService>();
            var dto = new CreateTodoRequestDto { Title = "Alta", Description = "Prueba" };
            todoServiceMock.Setup(s => s.AddHighPriorityTodoAsync(dto))
                .ReturnsAsync(new Response<string> { Successful = true });

            var controller = new TodoController(todoServiceMock.Object);

            var result = await controller.CreateHigh(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<Response<string>>(okResult.Value);
            Assert.True(response.Successful);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithExistingId_ReturnsSuccess()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.DeleteTodoAsync(1))
                .ReturnsAsync(new Response<string> { Successful = true });

            var controller = new TodoController(todoServiceMock.Object);

            var result = await controller.DeleteTodoAsync(1);

            Assert.NotNull(result.Value);
            Assert.True(result.Value!.Successful);
            todoServiceMock.Verify(s => s.DeleteTodoAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetPorcentajeCompletadas_ReturnsCorrectValue()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.ContarTareasCompletadasAsync())
                .ReturnsAsync(75.0);

            var controller = new TodoController(todoServiceMock.Object);

            var result = await controller.GetPorcentajeCompletadas();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Aquí, okResult.Value es un objeto anónimo, así que usa reflection o dynamic
            var porcentaje = okResult.Value.GetType().GetProperty("PorcentajeCompletadas")?.GetValue(okResult.Value, null);
            Assert.Equal(75.0, (double)porcentaje);
        }
    }
}
