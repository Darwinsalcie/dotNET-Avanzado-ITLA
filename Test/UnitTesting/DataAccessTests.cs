// DataAccessTests.cs
using API.Controllers;
using Application.DTOs.Response;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting
{
    public class DataAccessTests : TestBase
    {
        [Fact]
        public async Task DeleteTodoAsync_WithAdminRoleAndValidId_ReturnsSuccess()
        {
            // Arrange
            ServiceMock.Setup(s => s.DeleteTodoAsync(1, It.IsAny<int>()))
                       .ReturnsAsync(new Response<string> { Successful = true, Message = "Tarea eliminado correctamente." });
            Authenticate(role: "Admin");

            // Act
            var result = await Controller.DeleteTodoAsync(1);

            // Assert
            Assert.NotNull(result.Value);
            Assert.True(result.Value!.Successful);
            ServiceMock.Verify(s => s.DeleteTodoAsync(1, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetPorcentajeCompletadas_ReturnsCorrectPercentage()
        {
            // Arrange
            int userId = 1;
            ServiceMock.Setup(s => s.ContarTareasCompletadasAsync(userId))
                       .ReturnsAsync(60.0);
            Authenticate(userId);

            // Act
            var ok = await Controller.GetPorcentajeCompletadas();

            // Assert
            var result = Assert.IsType<OkObjectResult>(ok);
            var pct = result.Value!.GetType()
                           .GetProperty("PorcentajeCompletadas")!
                           .GetValue(result.Value, null);
            Assert.Equal(60.0, (double)pct!);
            ServiceMock.Verify(s => s.ContarTareasCompletadasAsync(userId), Times.Once);
        }
    }
}
