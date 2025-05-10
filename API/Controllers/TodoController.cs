
using Microsoft.AspNetCore.Mvc;
using Application.DTOs;
using Application.Interfaces;

namespace API.Controllers
{
[ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService<string> _service;

        public TodosController(ITodoService<string> service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoDto<string>>>> GetAll()
        {
            var todos = await _service.GetAllAsync();
            return Ok(todos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TodoDto<string>>> GetById(int id)
        {
            var todo = await _service.GetByIdAsync(id);
            if (todo == null)
                return NotFound();
            return Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<TodoDto<string>>> Create(CreateTodoDto<string> dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TodoDto<string>>> Update(int id, UpdateTodoDto<string> dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }



        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<TodoDto<string>>>> GetPending()
        {
            var todos = await _service.GetPendingAsync();
            return Ok(todos);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<TodoDto<string>>>> GetOverdue()
        {
            var todos = await _service.GetOverdueAsync();
            return Ok(todos);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TodoDto<string>>>> Search(string keyword)
        {
            var todos = await _service.SearchAsync(keyword);
            return Ok(todos);
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
