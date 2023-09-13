using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Services;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemService _todoItemService;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(ITodoItemService todoItemService, ILogger<TodoItemsController> logger)
        {
            _todoItemService = todoItemService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            var todoItems = await _todoItemService.GetIncompleteTodoItemsAsync();
            return Ok(todoItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(Guid id)
        {
            var todoItem = await _todoItemService.GetTodoItemAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return Ok(todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            try
            {
                await _todoItemService.UpdateTodoItemAsync(todoItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_todoItemService.TodoItemIdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
        {
            if (string.IsNullOrWhiteSpace(todoItem?.Description))
            {
                return BadRequest("Description is required");
            }
            else if (_todoItemService.TodoItemDescriptionExists(todoItem.Description))
            {
                return BadRequest("Description already exists");
            }

            await _todoItemService.CreateTodoItemAsync(todoItem);

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        [HttpPut("mark-as-complete/{id}")]
        public async Task<IActionResult> MarkTodoItemAsComplete(Guid id)
        {
            var result = await _todoItemService.MarkTodoItemAsCompleteAsync(id);

            if (result)
            {
                return NoContent(); // Successfully marked as complete
            }
            else
            {
                return NotFound(); // Item not found or already marked as complete
            }
        }


    }

    //Original Code
    //public class TodoItemsController : ControllerBase
    //{
    //    private readonly TodoContext _context;
    //    private readonly ILogger<TodoItemsController> _logger;

    //    public TodoItemsController(TodoContext context, ILogger<TodoItemsController> logger)
    //    {
    //        _context = context;
    //        _logger = logger;
    //    }

    //    // GET: api/TodoItems
    //    [HttpGet]
    //    public async Task<IActionResult> GetTodoItems()
    //    {
    //        var results = await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();
    //        return Ok(results);
    //    }

    //    // GET: api/TodoItems/...
    //    [HttpGet("{id}")]
    //    public async Task<IActionResult> GetTodoItem(Guid id)
    //    {
    //        var result = await _context.TodoItems.FindAsync(id);

    //        if (result == null)
    //        {
    //            return NotFound();
    //        }

    //        return Ok(result);
    //    }

    //    // PUT: api/TodoItems/... 
    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
    //    {
    //        if (id != todoItem.Id)
    //        {
    //            return BadRequest();
    //        }

    //        _context.Entry(todoItem).State = EntityState.Modified;

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!TodoItemIdExists(id))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }

    //        return NoContent();
    //    } 

    //    // POST: api/TodoItems 
    //    [HttpPost]
    //    public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
    //    {
    //        if (string.IsNullOrEmpty(todoItem?.Description))
    //        {
    //            return BadRequest("Description is required");
    //        }
    //        else if (TodoItemDescriptionExists(todoItem.Description))
    //        {
    //            return BadRequest("Description already exists");
    //        } 

    //        _context.TodoItems.Add(todoItem);
    //        await _context.SaveChangesAsync();

    //        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
    //    } 

    //    private bool TodoItemIdExists(Guid id)
    //    {
    //        return _context.TodoItems.Any(x => x.Id == id);
    //    }

    //    private bool TodoItemDescriptionExists(string description)
    //    {
    //        return _context.TodoItems
    //               .Any(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
    //    }
    //}
}
