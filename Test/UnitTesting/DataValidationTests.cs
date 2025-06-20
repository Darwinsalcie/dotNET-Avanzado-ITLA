// DataValidationTests.cs
using API.Controllers;
using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Domain.Common;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting
{
    public class DataValidationTests : TestBase
    {
        [Fact]
        public async Task AddTodoAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var valid = new Todo(1, "Tarea válida", "Desc", DateTime.UtcNow.AddDays(1), null, Status.Pendiente, Priority.Medium);
            ServiceMock.Setup(s => s.AddTodoAsync(It.IsAny<Todo>()))
                       .ReturnsAsync(new Response<string> { Successful = true });
            Authenticate();

            // Act
            var result = await Controller.AddTodoAsync(valid);

            // Assert
            Assert.IsType<ActionResult<Response<string>>>(result);
            ServiceMock.Verify(s => s.AddTodoAsync(It.IsAny<Todo>()), Times.Once);
        }

        [Fact]
        public async Task AddTodoAsync_WithEmptyTitle_ReturnsValidationError()
        {
            // Arrange
            var invalid = new Todo(1, "", "Desc", DateTime.UtcNow.AddDays(1), null, Status.Pendiente, Priority.Medium);
            ServiceMock.Setup(s => s.AddTodoAsync(It.Is<Todo>(t => string.IsNullOrWhiteSpace(t.Title))))
                       .ReturnsAsync(new Response<string> { Successful = false, Errors = { "El título es obligatorio." } });
            Authenticate();

            // Act
            var result = await Controller.AddTodoAsync(invalid);

            // Assert
            var response = Assert.IsType<Response<string>>(result.Value);
            Assert.False(response.Successful);
            Assert.Contains("El título es obligatorio.", response.Errors);
        }

        [Fact]
        public async Task AddTodoAsync_WithNullTitle_ReturnsValidationError()
        {
            // Arrange
            var invalid = new Todo(1, null!, "Desc", DateTime.UtcNow.AddDays(1), null, Status.Pendiente, Priority.Medium);
            ServiceMock.Setup(s => s.AddTodoAsync(It.Is<Todo>(t => t.Title == null)))
                       .ReturnsAsync(new Response<string> { Successful = false, Errors = { "El título es obligatorio." } });
            Authenticate();

            // Act
            var result = await Controller.AddTodoAsync(invalid);

            // Assert
            var response = Assert.IsType<Response<string>>(result.Value);
            Assert.False(response.Successful);
            Assert.Contains("El título es obligatorio.", response.Errors);
        }

        [Fact]
        public async Task AddTodoAsync_WithPastDueDate_ReturnsValidationError()
        {
            // Arrange
            var invalid = new Todo(1, "Tarea", "Desc", DateTime.UtcNow.AddDays(-1), null, Status.Pendiente, Priority.Medium);
            ServiceMock.Setup(s => s.AddTodoAsync(It.Is<Todo>(t => t.DueDate < DateTime.UtcNow)))
                       .ReturnsAsync(new Response<string> { Successful = false, Errors = { "La fecha de vencimiento no puede ser pasada." } });
            Authenticate();

            // Act
            var result = await Controller.AddTodoAsync(invalid);

            // Assert
            var response = Assert.IsType<Response<string>>(result.Value);
            Assert.False(response.Successful);
            Assert.Contains("La fecha de vencimiento no puede ser pasada.", response.Errors);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithInvalidData_ReturnsValidationError()
        {
            // Arrange
            var dto = new UpdateTodoRequestDto
            {
                Title = "",
                Description = "Descripción",
                DueDate = DateTime.UtcNow.AddDays(-2),
            };
            ServiceMock.Setup(s => s.UpdateTodoAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<UpdateTodoRequestDto>()))
                       .ReturnsAsync(new Response<string> { Successful = false, Errors = { "Datos inválidos." } });
            Authenticate();

            // Act
            var actionResult = await Controller.Update(1, dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var response = Assert.IsType<Response<string>>(badRequest.Value);
            Assert.False(response.Successful);
            Assert.Contains("Datos inválidos.", response.Errors);
        }
    }
}
