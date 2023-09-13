using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace TodoList.Api.Services
{
    public interface ITodoItemService
    {
        Task<List<TodoItem>> GetIncompleteTodoItemsAsync();
        Task<TodoItem> GetTodoItemAsync(Guid id);
        Task UpdateTodoItemAsync(TodoItem todoItem);
        Task CreateTodoItemAsync(TodoItem todoItem);
        Task<bool> MarkTodoItemAsCompleteAsync(Guid id);
        bool TodoItemIdExists(Guid id);
        bool TodoItemDescriptionExists(string description);
        
    }

   
}
