using System.Security.Claims;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using API.Controllers;
using Domain.DTOs;

namespace UnitTesting
{
    public class DataAccessTests
    {
        [Fact]
        public async Task DeleteTodoAsync_WithAdminRoleAndValidId_ReturnsSuccess()
        {
            // Arrange
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.DeleteTodoAsync(1, It.IsAny<int>()))
                .ReturnsAsync(new Response<string> { Successful = true, Message = "Tarea eliminado correctamente." });

            var controller = new TodoController(todoServiceMock.Object);

            // Simular usuario con rol "Admin"
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.DeleteTodoAsync(1);

            // Assert
            Assert.NotNull(result.Value);
            Assert.True(result.Value!.Successful);
            Assert.Equal("Tarea eliminado correctamente.", result.Value.Message);
            todoServiceMock.Verify(s => s.DeleteTodoAsync(1, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetPorcentajeCompletadas_ReturnsCorrectPercentage()
        {
            // Arrange
            int userId = 1;
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.ContarTareasCompletadasAsync(userId))
                .ReturnsAsync(60.0);

            var controller = new TodoController(todoServiceMock.Object);

            // Simular usuario autenticado
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

            // Reflection para acceder a la propiedad del objeto anónimo
            var porcentaje = okResult.Value.GetType().GetProperty("PorcentajeCompletadas")?.GetValue(okResult.Value, null);
            Assert.Equal(60.0, (double)porcentaje!);

            todoServiceMock.Verify(s => s.ContarTareasCompletadasAsync(userId), Times.Once);
        }
    }
}