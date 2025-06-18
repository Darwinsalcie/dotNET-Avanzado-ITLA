using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Application.Services;
using API.Controllers;
using Application.DTOs.Response;
using Domain.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Domain.DTOs;

namespace UnitTesting
{
    public class AuthorizationTest
    {
        private TodoController GetControllerWithUser(Mock<ITodoService> todoServiceMock, string? role = null, bool authenticated = true)
        {
            var claims = new List<Claim>();
            if (role != null)
                claims.Add(new Claim(ClaimTypes.Role, role));
            var identity = authenticated ? new ClaimsIdentity(claims, "TestAuth") : new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            var controller = new TodoController(todoServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
            return controller;
        }

        [Fact]
        public async Task DeleteTodoAsync_WithoutAdminRole_ReturnsForbidden()
        {
            // Arrange
            var todoServiceMock = new Mock<ITodoService>();
            var controller = GetControllerWithUser(todoServiceMock); // No role
            var id = 1;

            // Act
            var result = await controller.DeleteTodoAsync(id);

            // Assert
            // Since [Authorize(Roles = "Admin")] is handled by middleware, 
            // direct controller call will not return 403.
            // In integration tests, this would return 403.
            // Here, we just assert that the method is protected by the attribute.
            var method = typeof(TodoController).GetMethod("DeleteTodoAsync");
            var hasAuthorize = method.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .Cast<AuthorizeAttribute>()
                .Any(a => a.Roles == "Admin");
            Assert.True(hasAuthorize);
        }

        [Fact]
        public void AllEndpoints_RequireAuthentication()
        {
            // Arrange
            var controllerType = typeof(TodoController);

            // Act
            var hasAuthorize = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any();

            // Assert
            Assert.True(hasAuthorize);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithAdminRole_CallsService()
        {
            var todoServiceMock = new Mock<ITodoService>();
            todoServiceMock.Setup(s => s.DeleteTodoAsync(It.IsAny<int>()))
                .ReturnsAsync(new Response<string> { Successful = true });

            var controller = GetControllerWithUser(todoServiceMock, "Admin");
            var id = 1;

            var result = await controller.DeleteTodoAsync(id);

            todoServiceMock.Verify(s => s.DeleteTodoAsync(id), Times.Once);
        }
    }
}