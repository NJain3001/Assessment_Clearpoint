using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Api.Services;
using Xunit;

namespace TodoList.Api.UnitTests
{
    public class TodoItemServiceTests
    {
        [Fact]
        public async Task GetIncompleteTodoItemsAsync_ReturnsIncompleteItems()
        {
            // Arrange
        var dbContext = GetDbContextWithData(); // Create a DbContext with sample data
            var todoItemService = new TodoItemService(dbContext);

            // Act
            var incompleteItems = await todoItemService.GetIncompleteTodoItemsAsync();

            // Assert
            Assert.NotNull(incompleteItems);
            Assert.True(incompleteItems.All(item => !item.IsCompleted));
        }
        [Fact]
        public async Task GetTodoItemAsync_WithValidId_ReturnsTodoItem()
        {
            // Arrange
            var id = new Guid("6B29FC40-CA47-1067-B31D-00DD010662DA"); // Replace with a valid GUID
            var dbContext = GetDbContextWithData(); // Create a DbContext with sample data
            var todoItemService = new TodoItemService(dbContext);

            // Act
            var todoItem = await todoItemService.GetTodoItemAsync(id);

            // Assert
            Assert.NotNull(todoItem);
            Assert.Equal(id, todoItem.Id);
        }
        [Fact]
        public async Task GetTodoItemAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid(); // Use an invalid ID
            var dbContext = GetDbContextWithData(); 
            var todoItemService = new TodoItemService(dbContext);

            // Act
            var todoItem = await todoItemService.GetTodoItemAsync(id);

            // Assert
            Assert.Null(todoItem);
        }
        [Fact]
        public async Task UpdateTodoItemAsync_WithValidModel_UpdatesItem()
        {
            // Arrange
            var dbContext = GetDbContextWithData(); 
            var todoItemService = new TodoItemService(dbContext);
            var existingItem = dbContext.TodoItems.First(); // Get an existing item
            var updatedItem = new TodoItem
            {
                Id = existingItem.Id,
                Description = "Updated Description", // Modify some properties
                IsCompleted = !existingItem.IsCompleted
            };
            dbContext.Entry(existingItem).State = EntityState.Detached;
            // Act
            await todoItemService.UpdateTodoItemAsync(updatedItem);

            // Assert
            var updatedFromDb = await dbContext.TodoItems.FindAsync(existingItem.Id);
            Assert.NotNull(updatedFromDb);
            Assert.Equal("Updated Description", updatedFromDb.Description);
            Assert.Equal(!existingItem.IsCompleted, updatedFromDb.IsCompleted);
        }
        [Fact]
        public async Task CreateTodoItemAsync_WithValidModel_CreatesItem()
        {
            // Arrange
            var dbContext = GetDbContextWithData();
            var todoItemService = new TodoItemService(dbContext);
            var newItem = new TodoItem
            {
                Description = "New Task",
                IsCompleted = false
            };

            // Act
            await todoItemService.CreateTodoItemAsync(newItem);

            // Assert
            var createdFromDb = await dbContext.TodoItems.FindAsync(newItem.Id);
            Assert.NotNull(createdFromDb);
            Assert.Equal(newItem.Description, createdFromDb.Description);
            Assert.Equal(newItem.IsCompleted, createdFromDb.IsCompleted);
        }

        [Fact]
        public void TodoItemIdExists_WithExistingId_ReturnsTrue()
        {
            // Arrange
            var dbContext = GetDbContextWithData();
            var todoItemService = new TodoItemService(dbContext);
            var existingItem = dbContext.TodoItems.First(); // Get an existing item

            // Act
            var exists = todoItemService.TodoItemIdExists(existingItem.Id);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public void TodoItemIdExists_WithNonExistingId_ReturnsFalse()
        {
            // Arrange
            var dbContext = GetDbContextWithData();
            var todoItemService = new TodoItemService(dbContext);
            var nonExistingId = Guid.NewGuid(); // Use a non-existing ID

            // Act
            var exists = todoItemService.TodoItemIdExists(nonExistingId);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public void TodoItemDescriptionExists_WithExistingDescription_ReturnsTrue()
        {
            // Arrange
            var dbContext = GetDbContextWithData();
            var todoItemService = new TodoItemService(dbContext);
            var existingItem = dbContext.TodoItems.First(); // Get an existing item

            // Act
            var exists = todoItemService.TodoItemDescriptionExists(existingItem.Description);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public void TodoItemDescriptionExists_WithNonExistingDescription_ReturnsFalse()
        {
            // Arrange
            var dbContext = GetDbContextWithData();
            var todoItemService = new TodoItemService(dbContext);
            var nonExistingDescription = "Non-Existing Description"; // Use a non-existing description

            // Act
            var exists =  todoItemService.TodoItemDescriptionExists(nonExistingDescription);

            // Assert
            Assert.False(exists);
        }


        private TodoContext GetDbContextWithData()
        {
            
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dbContext = new TodoContext(options);
            var sampleData = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Description = "Todo Item 1", IsCompleted = false },
            new TodoItem { Id = Guid.NewGuid(), Description = "Todo Item 2", IsCompleted = true },
            new TodoItem{ Id = new Guid("6B29FC40-CA47-1067-B31D-00DD010662DA"),Description = "Todo Item 3", IsCompleted = true }
                                         
        };
            dbContext.AddRange(sampleData);
            dbContext.SaveChanges();

            return dbContext;
        }
    }

}
