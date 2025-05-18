using Microsoft.AspNetCore.Mvc;
using Domain.DTOs;
namespace API.Extensions
{
    public static class ControllerResponseExtensions
    {
        public static async Task<IActionResult> ToActionResultAsync<T>(this ControllerBase controller, Task<Response<T>> task)
        {
            var result = await task;
            if (result.Successful)
                return controller.Ok(result);
            return controller.BadRequest(result);
        }
    }

}
