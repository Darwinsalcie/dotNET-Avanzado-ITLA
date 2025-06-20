// TestBase.cs
using System.Collections.Generic;
using System.Security.Claims;
using API.Controllers;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTesting
{
    public abstract class TestBase
    {
        protected readonly Mock<ITodoService> ServiceMock;
        protected readonly TodoController Controller;

        protected TestBase()
        {
            ServiceMock = new Mock<ITodoService>();
            Controller = new TodoController(ServiceMock.Object);
        }

        protected void Authenticate(int userId = 1, string? role = null)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            if (role != null) claims.Add(new Claim(ClaimTypes.Role, role));

            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"))
                }
            };
        }
    }
}
