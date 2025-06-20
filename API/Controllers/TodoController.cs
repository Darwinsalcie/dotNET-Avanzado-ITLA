using API.Extensions;
using Application.DTOs.RequesDTO;
using Application.DTOs.Response;
using Application.Services;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {

        private readonly ITodoService _todoService;
        //private readonly ITodoProcessingQueue _todoQueue;
        public TodoController(ITodoService todoService/*, ITodoProcessingQueue todoQueue*/)
        {
            _todoService = todoService;
            //_todoQueue = todoQueue;
        }

        // GET: api/Todo
        [HttpGet]

        /*Lo que sería <Response<TodoResponseDTO>> tenemo Response<T>, donde T es reemplazado por TodoResponseDTO
         * Envolver el dto en Response permite agregar como si fueran campos al objeto que se devuelve con informacion
         * sobre el resultado de la operación, como si hubo errores, si fue exitoso, etc.
         */

        /*Ese Response<dto> lo envolvemos en ActionResult que es una clase de ASP.NET que nos permite manejar respuestas http
         como 200, 400, 500, etc.
         */

        //public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoAllAsync()
        //   => await _todoService.GetTodoAllAsync();

        //public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoAllAsync(int userId)
        //    => await _todoService.GetTodoAllAsync(userId);

        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoAllAsync()
        {
            // Obtén el userId del claim del usuario autenticado
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new Response<TodoResponseDTO>
                {
                    Successful = false,
                    Message = "No se pudo identificar el usuario."
                });
            }

            int userId = int.Parse(userIdClaim.Value);
            return await _todoService.GetTodoAllAsync(userId);
        }



        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoByIdAsync(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new Response<TodoResponseDTO>
                {
                    Successful = false,
                    Message = "No se pudo identificar el usuario."
                });
            }

            int userId = int.Parse(userIdClaim.Value);
            return await _todoService.GetTodoByIdAsync(id, userId);
        }


        // Cambia la ruta para que acepte parámetros como query string, no en la ruta
        [HttpGet("filter")]
        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodobyFilter(
        [FromQuery] int? status,
        [FromQuery] int? priority,
        [FromQuery] string? title,
        [FromQuery] DateTime? dueDate)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new Response<TodoResponseDTO>
                {
                    Successful = false,
                    Message = "No se pudo identificar el usuario."
                });
            }

            int userId = int.Parse(userIdClaim.Value);
            return await _todoService.FilterTodoAsync(userId, status, priority, title, dueDate);
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<Response<string>>> AddTodoAsync([FromBody] Todo todo)
        {
            // Obtén el userId del claim del usuario autenticado
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new Response<string>
                {
                    Successful = false,
                    Message = "No se pudo identificar el usuario."
                });
            }

            int userId = int.Parse(userIdClaim.Value);
            todo.UserId = userId; // Asigna el userId al objeto Todo

            return await _todoService.AddTodoAsync(todo);
        }


        // PUT: api/Todo/5
        [HttpPut("update/{id}")]
        public async Task<ActionResult<Response<string>>> Update(int id, [FromBody] UpdateTodoRequestDto dto)
        {
            // 1. Obtén el userId del claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new Response<string>
                {
                    Successful = false,
                    Message = "No se pudo identificar el usuario."
                });

            int userId = int.Parse(userIdClaim.Value);

            // 2. Llama al servicio
            var result = await _todoService.UpdateTodoAsync(id, userId, dto);

            // 3. Devuelve el código HTTP adecuado
            if (!result.Successful)
                return BadRequest(result);

            return Ok(result);
        }

        // DELETE: api/Todo/5
        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult<Response<string>>> DeleteTodoAsync(int id)
        //    => await _todoService.DeleteTodoAsync(id);

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response<string>>> DeleteTodoAsync(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new Response<string>
                {
                    Successful = false,
                    Message = "No se pudo identificar el usuario."
                });
            }

            int userId = int.Parse(userIdClaim.Value);
            return await _todoService.DeleteTodoAsync(id, userId);
        }

        [HttpDelete("soft/{id}")]
        public async Task<ActionResult<Response<string>>> SoftDeleteTodoAsync(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new Response<string>
                {
                    Successful = false,
                    Message = "No se pudo identificar el usuario."
                });
            }
            int userId = int.Parse(userIdClaim.Value);
            return await _todoService.DeleteSoftTodoAsync(id, userId);
        }


        // GET /api/todo/porcentaje-completadas
        [HttpGet("porcentaje-completadas")]
        public async Task<IActionResult> GetPorcentajeCompletadas()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { Message = "No se pudo identificar el usuario." });
            }
            int userId = int.Parse(userIdClaim.Value);

            double porcentaje = await _todoService.ContarTareasCompletadasAsync(userId);
            return Ok(new { PorcentajeCompletadas = porcentaje });
        }

        [HttpGet("porcentaje-pendientes")]
        public async Task<IActionResult> GetPorcentajePendientes()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { Message = "No se pudo identificar el usuario." });
            }
            int userId = int.Parse(userIdClaim.Value);

            double porcentaje = await _todoService.ContarTareasPendientesAsync(userId);
            return Ok(new { PorcentajePendientes = porcentaje });
        }


        //POST: api/Todo/high(prioridad alta)

        [HttpPost("high")]
        public async Task<IActionResult> CreateHigh([FromBody] CreateTodoRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new Response<string>
                {
                    Successful = false,
                    Message = "No se pudo identificar el usuario."
                });
            }

            dto.UserId = int.Parse(userIdClaim.Value);
            return await this.ToActionResultAsync(_todoService.AddHighPriorityTodoAsync(dto));
        }

        [HttpPost("medium")]
        public async Task<IActionResult> CreateMedium([FromBody] CreateTodoRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new Response<string>
                {
                    Successful = false,
                    Message = "No se pudo identificar el usuario."
                });
            }

            dto.UserId = int.Parse(userIdClaim.Value);
            return await this.ToActionResultAsync(_todoService.AddMediumPriorityTodoAsync(dto));
        }

        [HttpPost("low")]
        public async Task<IActionResult> CreateLow([FromBody] CreateTodoRequestDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new Response<string>
                {
                    Successful = false,
                    Message = "No se pudo identificar el usuario."
                });
            }

            dto.UserId = int.Parse(userIdClaim.Value);
            return await this.ToActionResultAsync(_todoService.AddLowPriorityTodoAsync(dto));
        }

    }
}
