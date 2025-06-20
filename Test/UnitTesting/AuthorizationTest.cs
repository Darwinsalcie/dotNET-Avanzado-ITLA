// AuthorizationTests.cs
using API.Controllers;
using Application.DTOs.Response;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting
{
    public class AuthorizationTests : TestBase
    {
        [Fact]
        public async Task DeleteTodoAsync_WithoutAdminRole_HasAuthorizeAttribute()
        {
            // Arrange
            Authenticate();

            // Act
            await Controller.DeleteTodoAsync(1);

            // Assert
            var method = typeof(TodoController).GetMethod("DeleteTodoAsync");
            var has = method.GetCustomAttributes(typeof(AuthorizeAttribute), false)
                             .Cast<AuthorizeAttribute>()
                             .Any(a => a.Roles == "Admin");
            Assert.True(has);
        }

        [Fact]
        public void AllEndpoints_RequireAuthentication()
        {
            // Act
            var has = typeof(TodoController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Any();

            // Assert
            Assert.True(has);
        }

        [Fact]
        public async Task DeleteTodoAsync_WithAdminRole_CallsService()
        {
            // Arrange
            ServiceMock.Setup(s => s.DeleteTodoAsync(It.IsAny<int>(), It.IsAny<int>()))
                       .ReturnsAsync(new Response<string> { Successful = true });
            Authenticate(role: "Admin");

            // Act
            await Controller.DeleteTodoAsync(1);

            // Assert
            ServiceMock.Verify(s => s.DeleteTodoAsync(1, It.IsAny<int>()), Times.Once);
        }
    }
}
