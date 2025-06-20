// TodoControllerTests.cs
using API.Controllers;
using Application.DTOs.Response;
using Domain.DTOs;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting
{
    public class TodoControllerTests : TestBase
    {
        [Fact]
        public async Task Get_Ok()
        {
            // Arrange
            ServiceMock.Setup(s => s.GetTodoAllAsync(1))
                       .ReturnsAsync(new Response<TodoResponseDTO> { Successful = true, DataList = new List<TodoResponseDTO>() });
            Authenticate(role: "Admin");

            // Act
            var result = await Controller.GetTodoAllAsync();

            // Assert
            Assert.IsType<Response<TodoResponseDTO>>(result.Value!);
        }

        [Fact]
        public async Task Get_Quantity()
        {
            // Arrange
            int id = 1;
            ServiceMock.Setup(s => s.GetTodoByIdAsync(id, 1))
                       .ReturnsAsync(new Response<TodoResponseDTO>
                       {
                           SingleData = new TodoResponseDTO { Id = id, Title = "Test" },
                           DataList = new List<TodoResponseDTO>(),
                           Successful = true
                       });
            Authenticate(role: "Admin");

            // Act
            var result = await Controller.GetTodoByIdAsync(id);

            // Assert
            var response = Assert.IsType<Response<TodoResponseDTO>>(result.Value!);
            Assert.Empty(response.DataList);
            Assert.Equal(id, response.SingleData!.Id);
        }
    }
}
