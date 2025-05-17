using Application.Interfaces;
using Application.Services;
using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        //Acá supongo que debería inyectase una interfaz para desacoplar el controlador de la implementación concreta
        //Pero mientras tanto, lo dejo así para que funcione
        private readonly ITodoService _todoService;
        public TodoController(ITodoService todoService)
            => _todoService = todoService;

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<Response<Todo>>> GetTodoAllAsync()
            => await _todoService.GetTodoAllAsync();

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Response<Todo>>> GetTodoByIdAsync(int id)
            => await _todoService.GetTodoByIdAsync(id);


        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<Response<string>>> AddTodoAsync(Todo todo)
            => await _todoService.AddTodoAsync(todo);


        // PUT: api/Todo/5
        [HttpPut]
        public async Task<ActionResult<Response<string>>> UpdateTodoAsync(Todo todo)
            => await _todoService.UpdateTodoAsync(todo);

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<string>>> DeleteTodoAsync(int id)
            => await _todoService.DeleteTodoAsync(id);


    }
}
