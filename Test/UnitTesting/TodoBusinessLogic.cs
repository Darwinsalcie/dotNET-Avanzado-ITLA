// TodoBusinessLogicTests.cs
using API.Controllers;
using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting
{
    public class TodoBusinessLogicTests : TestBase
    {
        [Fact]
        public async Task GetTodoAllAsync_ReturnsAllTodos()
        {
            // Arrange
            ServiceMock.Setup(s => s.GetTodoAllAsync(1))
                       .ReturnsAsync(new Response<TodoResponseDTO> { Successful = true, DataList = new List<TodoResponseDTO>() });
            Authenticate(role: "Admin");

            // Act
            var res = await Controller.GetTodoAllAsync();

            // Assert
            Assert.True(res.Value!.Successful);
            ServiceMock.Verify(s => s.GetTodoAllAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithExistingId_ReturnsTodo()
        {
            // Arrange
            ServiceMock.Setup(s => s.GetTodoByIdAsync(1, 1))
                       .ReturnsAsync(new Response<TodoResponseDTO> { Successful = true, SingleData = new TodoResponseDTO() });
            Authenticate(role: "Admin");

            // Act
            var result = await Controller.GetTodoByIdAsync(1);

            // Assert
            Assert.True(result.Value!.Successful);
            ServiceMock.Verify(s => s.GetTodoByIdAsync(1, 1), Times.Once);
        }

        [Fact]
        public async Task GetTodoByIdAsync_WithNonExistingId_ReturnsError()
        {
            // Arrange
            ServiceMock.Setup(s => s.GetTodoByIdAsync(99, 1))
                       .ReturnsAsync(new Response<TodoResponseDTO> { Successful = false, Errors = { "No encontrado" } });
            Authenticate(role: "Admin");

            // Act
            var result = await Controller.GetTodoByIdAsync(99);

            // Assert
            Assert.False(result.Value!.Successful);
            Assert.Contains("No encontrado", result.Value.Errors);
        }

        [Fact]
        public async Task FilterTodoAsync_ByStatusAndPriority_ReturnsFiltered()
        {
            // Arrange
            int userId = 1;
            int? status = 2;
            int? priority = null;
            string? title = null;
            DateTime? dueDate = null;
            ServiceMock.Setup(s => s.FilterTodoAsync(userId, status, priority, title, dueDate))
                       .ReturnsAsync(new Response<TodoResponseDTO> { Successful = true, DataList = new List<TodoResponseDTO>() });
            Authenticate(userId);

            // Act
            var result = await Controller.GetTodobyFilter(status, priority, title, dueDate);

            // Assert
            Assert.True(result.Value!.Successful);
            ServiceMock.Verify(s => s.FilterTodoAsync(userId, status, priority, title, dueDate), Times.Once);
        }

        [Fact]
        public async Task CreateHighPriorityTodoAsync_ReturnsSuccess()
        {
            // Arrange
            var dto = new CreateTodoRequestDto { Title = "Alta", Description = "Prueba" };
            ServiceMock.Setup(s => s.AddHighPriorityTodoAsync(dto))
                       .ReturnsAsync(new Response<string> { Successful = true });
            Authenticate();

            // Act
            var result = await Controller.CreateHigh(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var resp = Assert.IsType<Response<string>>(ok.Value);
            Assert.True(resp.Successful);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithExistingId_ReturnsSuccess()
        {
            // Arrange
            ServiceMock.Setup(s => s.DeleteTodoAsync(1, It.IsAny<int>()))
                       .ReturnsAsync(new Response<string> { Successful = true });
            Authenticate(role: "Admin");

            // Act
            var result = await Controller.DeleteTodoAsync(1);

            // Assert
            Assert.True(result.Value!.Successful);
            ServiceMock.Verify(s => s.DeleteTodoAsync(1, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetPorcentajeCompletadas_ReturnsCorrectValue()
        {
            // Arrange
            int userId = 1;
            ServiceMock.Setup(s => s.ContarTareasCompletadasAsync(userId))
                       .ReturnsAsync(75.0);
            Authenticate(userId);

            // Act
            var ok = await Controller.GetPorcentajeCompletadas();

            // Assert
            var result = Assert.IsType<OkObjectResult>(ok);
            var pct = (double)result.Value!.GetType().GetProperty("PorcentajeCompletadas")!.GetValue(result.Value, null)!;
            Assert.Equal(75.0, pct);
            ServiceMock.Verify(s => s.ContarTareasCompletadasAsync(userId), Times.Once);
        }
    }
}
