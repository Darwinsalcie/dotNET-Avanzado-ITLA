using Application.DTOs.Response;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.RequesDTO;
using Application.Services;
using API.Extensions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoAllAsync()
            => await _todoService.GetTodoAllAsync();

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoByIdAsync(int id)
            => await _todoService.GetTodoByIdAsync(id);


        // GET: api/Todo/status/6
        [HttpGet("status/{status}")]
        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoByStatusAsync(int status)
            => await _todoService.GetTodoByStatusAsync(status);



        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<Response<string>>> AddTodoAsync(Todo todo)
            => await _todoService.AddTodoAsync(todo);


        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Response<string>>> UpdateTodoAsync(Todo todo, int id)
            => await _todoService.UpdateTodoAsync(todo, id);

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<string>>> DeleteTodoAsync(int id)
            => await _todoService.DeleteTodoAsync(id);


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
