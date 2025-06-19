using Application.DTOs.Response;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.RequesDTO;
using Application.Services;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;

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

        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoAllAsync()
            => await _todoService.GetTodoAllAsync();

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoByIdAsync(int id)
            => await _todoService.GetTodoByIdAsync(id);


        // Cambia la ruta para que acepte parámetros como query string, no en la ruta
        [HttpGet("filter")]
        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodobyFilter(
            [FromQuery] int? status,
            [FromQuery] int? priority,
            [FromQuery] string? title,
            [FromQuery] DateTime? dueDate)
            => await _todoService.FilterTodoAsync(status, priority, title, dueDate);


        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<Response<string>>> AddTodoAsync(Todo todo)
            => await _todoService.AddTodoAsync(todo);


        // PUT: api/Todo/5
        [HttpPut("update/{id}")]
        public async Task<ActionResult<Response<string>>> UpdateTodoAsync(Todo todo, int id)
            => await _todoService.UpdateTodoAsync(todo, id);

        // DELETE: api/Todo/5
        //[HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult<Response<string>>> DeleteTodoAsync(int id)
        //    => await _todoService.DeleteTodoAsync(id);

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<string>>> DeleteTodoAsync(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response<string>
                {
                    Successful = false,
                    Message = "Usuario no autorizado para eliminar tareas."
                });
            }

            return await _todoService.DeleteTodoAsync(id);
        }


        // GET /api/todo/porcentaje-completadas
        [HttpGet("porcentaje-completadas")]
        public async Task<IActionResult> GetPorcentajeCompletadas()
        {
            double porcentaje = await _todoService.ContarTareasCompletadasAsync();
            return Ok(new { PorcentajeCompletadas = porcentaje });
        }


        [HttpGet("porcentaje-pendientes")]
        public async Task<IActionResult> GetPorcentajePendientes()
        {
            double porcentaje = await _todoService.ContarTareasPendientesAsync();
            return Ok(new { PorcentajePendientes = porcentaje });
        }



        //POST: api/Todo/high(prioridad alta)


        [HttpPost("high")]

        public async Task<IActionResult> CreateHigh([FromBody] CreateTodoRequestDto dto)
            => await this.ToActionResultAsync(_todoService.AddHighPriorityTodoAsync(dto));


        // prioridad media
        [HttpPost("medium")]
        public async Task<IActionResult> CreateMedium([FromBody] CreateTodoRequestDto dto)
            => await this.ToActionResultAsync(_todoService.AddMediumPriorityTodoAsync(dto));


        // prioridad baja
        [HttpPost("low")]
        public async Task<IActionResult> CreateLow([FromBody] CreateTodoRequestDto dto)
            => await this.ToActionResultAsync(_todoService.AddLowPriorityTodoAsync(dto));

    }
}
