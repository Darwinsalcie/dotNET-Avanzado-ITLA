using Application.DTOs.Response;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.RequesDTO;
using Application.Factory;
using Application.Services;
using API.Extensions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        //Acá supongo que debería inyectase una interfaz para desacoplar el controlador de la implementación concreta
        //Pero mientras tanto, lo dejo así para que funcione
        private readonly ITodoService _todoService;
        public TodoController(ITodoService todoService)  =>
        
            _todoService = todoService;
        

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoAllAsync()
            => await _todoService.GetTodoAllAsync();

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<TodoResponseDTO>>> GetTodoByIdAsync(int id)
            => await _todoService.GetTodoByIdAsync(id);


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
