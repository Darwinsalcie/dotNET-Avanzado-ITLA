using API.Controllers;
using Application.DTOs.RequesDTO;
using Application.Services;
using Domain.Common;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;


namespace UnitTesting
{
    public class DataValidationTests
    {
        [Fact]
        public async Task AddTodoAsync_WithValidData_ReturnsSuccess()
        {
            var todoServiceMock = new Mock<ITodoService>();
            var validTodo = new Todo(1, "Tarea válida", "Descripción", DateTime.UtcNow.AddDays(1), null, Status.Pendiente, Priority.Medium);

            todoServiceMock.Setup(s => s.AddTodoAsync(It.IsAny<Todo>()))
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

            var result = await controller.AddTodoAsync(validTodo);

            var okResult = Assert.IsType<ActionResult<Response<string>>>(result);
            todoServiceMock.Verify(s => s.AddTodoAsync(It.IsAny<Todo>()), Times.Once);
        }

        [Fact]
        public async Task AddTodoAsync_WithEmptyTitle_ReturnsValidationError()
        {
            var todoServiceMock = new Mock<ITodoService>();
            var invalidTodo = new Todo(1, "", "Descripción", DateTime.UtcNow.AddDays(1), null, Status.Pendiente, Priority.Medium);

            todoServiceMock.Setup(s => s.AddTodoAsync(It.Is<Todo>(t => string.IsNullOrWhiteSpace(t.Title))))
                .ReturnsAsync(new Response<string> { Successful = false, Errors = new() { "El título es obligatorio." } });

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

            var result = await controller.AddTodoAsync(invalidTodo);

            var response = Assert.IsType<Response<string>>(result.Value);
            Assert.False(response.Successful);
            Assert.Contains("El título es obligatorio.", response.Errors);
        }

        [Fact]
        public async Task AddTodoAsync_WithNullTitle_ReturnsValidationError()
        {
            var todoServiceMock = new Mock<ITodoService>();
            var invalidTodo = new Todo(1,null, "Descripción", DateTime.UtcNow.AddDays(1), null, Status.Pendiente, Priority.Medium);

            todoServiceMock.Setup(s => s.AddTodoAsync(It.Is<Todo>(t => t.Title == null)))
                .ReturnsAsync(new Response<string> { Successful = false, Errors = new() { "El título es obligatorio." } });

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


            var result = await controller.AddTodoAsync(invalidTodo);

            var response = Assert.IsType<Response<string>>(result.Value);
            Assert.False(response.Successful);
            Assert.Contains("El título es obligatorio.", response.Errors);
        }

        [Fact]
        public async Task AddTodoAsync_WithPastDueDate_ReturnsValidationError()
        {
            var todoServiceMock = new Mock<ITodoService>();
            var invalidTodo = new Todo(1, "Tarea", "Descripción", DateTime.UtcNow.AddDays(-1), null, Status.Pendiente, Priority.Medium);

            todoServiceMock.Setup(s => s.AddTodoAsync(It.Is<Todo>(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow)))
                .ReturnsAsync(new Response<string> { Successful = false, Errors = new() { "La fecha de vencimiento no puede ser pasada." } });

            var controller = new TodoController(todoServiceMock.Object);

            // Simula usuario autenticado
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

            var result = await controller.AddTodoAsync(invalidTodo);

            var response = Assert.IsType<Response<string>>(result.Value);
            Assert.False(response.Successful);
            Assert.Contains("La fecha de vencimiento no puede ser pasada.", response.Errors);
        }

        [Fact]
        public async Task UpdateTodoAsync_WithInvalidData_ReturnsValidationError()
        {
            var todoServiceMock = new Mock<ITodoService>();
            var dto = new UpdateTodoRequestDto
            {
                Title = "",
                Description = "Descripción",
                DueDate = DateTime.UtcNow.AddDays(-2),
                // ...other properties as needed
            };

            todoServiceMock.Setup(s => s.UpdateTodoAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<UpdateTodoRequestDto>()))
                .ReturnsAsync(new Response<string> { Successful = false, Errors = new() { "Datos inválidos." } });

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

            var result = await controller.Update(1, dto); // <-- LÍNEA AGREGADA

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<Response<string>>(badRequestResult.Value);
            Assert.False(response.Successful);
            Assert.Contains("Datos inválidos.", response.Errors);
        }
    }
}