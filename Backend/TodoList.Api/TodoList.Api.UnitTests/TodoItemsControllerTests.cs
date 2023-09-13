using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Api.Controllers;
using TodoList.Api.Services;
using Xunit;

namespace TodoList.Api.UnitTests
{
    public class TodoItemsControllerTests
    {
        [Fact]
        public async Task GetTodoItems_ReturnsOkResultWithTodoItems()
        {
            // Arrange
            var mockTodoItemService = new Mock<ITodoItemService>();
            var controller = new TodoItemsController(mockTodoItemService.Object, null);
            var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Description = "To do Item 1", IsCompleted = false },
            new TodoItem { Id = Guid.NewGuid(), Description = "To do Item 2", IsCompleted = true }
        };
            mockTodoItemService.Setup(service => service.GetIncompleteTodoItemsAsync()).ReturnsAsync(todoItems);

            // Act
            var result = await controller.GetTodoItems();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTodoItems = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Equal(todoItems, returnedTodoItems);
        }

        [Fact]
        public async Task GetTodoItem_WithValidId_ReturnsOkResultWithTodoItem()
        {
            // Arrange
            var mockTodoItemService = new Mock<ITodoItemService>();
            var controller = new TodoItemsController(mockTodoItemService.Object, null);
            var itemId = Guid.NewGuid();
            var todoItem = new TodoItem { Id = itemId, Description = "Item 1", IsCompleted = false };
            mockTodoItemService.Setup(service => service.GetTodoItemAsync(itemId)).ReturnsAsync(todoItem);

            // Act
            var result = await controller.GetTodoItem(itemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedTodoItem = Assert.IsType<TodoItem>(okResult.Value);
            Assert.Equal(todoItem, returnedTodoItem);
        }

        [Fact]
        public async Task GetTodoItem_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockTodoItemService = new Mock<ITodoItemService>();
            var controller = new TodoItemsController(mockTodoItemService.Object, null);
            var nonExistingId = Guid.NewGuid();
            mockTodoItemService.Setup(service => service.GetTodoItemAsync(nonExistingId)).ReturnsAsync((TodoItem)null);

            // Act
            var result = await controller.GetTodoItem(nonExistingId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutTodoItem_WithValidModelAndMatchingId_ReturnsNoContentResult()
        {
            // Arrange
            var mockTodoItemService = new Mock<ITodoItemService>();
            var controller = new TodoItemsController(mockTodoItemService.Object, null);
            var itemId = Guid.NewGuid();
            var todoItem = new TodoItem { Id = itemId, Description = "Item 1", IsCompleted = false };

            // Act
            var result = await controller.PutTodoItem(itemId, todoItem);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task PutTodoItem_WithInvalidModel_ReturnsBadRequestResult()
        {
            // Arrange
            var mockTodoItemService = new Mock<ITodoItemService>();
            var controller = new TodoItemsController(mockTodoItemService.Object, null);
            var itemId = Guid.NewGuid();
            var todoItem = new TodoItem { Id = Guid.NewGuid(), Description = "ToDo Item 1", IsCompleted = false };

            // Act
            var result = await controller.PutTodoItem(itemId, todoItem);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutTodoItem_WithNonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var mockTodoItemService = new Mock<ITodoItemService>();
            mockTodoItemService.Setup(service => service.UpdateTodoItemAsync(It.IsAny<TodoItem>()))
                .ThrowsAsync(new DbUpdateConcurrencyException());
            var controller = new TodoItemsController(mockTodoItemService.Object, null);
            var nonExistingId = Guid.NewGuid();
            var todoItem = new TodoItem { Id = nonExistingId, Description = "Task 1", IsCompleted = false };

            // Act
            var result = await controller.PutTodoItem(nonExistingId, todoItem);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task PostTodoItem_WithValidModel_CreatesItemAndReturnsCreatedAtAction()
        {
            // Arrange
            var mockTodoItemService = new Mock<ITodoItemService>();
            var controller = new TodoItemsController(mockTodoItemService.Object, null);
            var newItem = new TodoItem { Description = "New Task", IsCompleted = false };

            TodoItem capturedTodoItem = null;
            mockTodoItemService
                .Setup(service => service.CreateTodoItemAsync(It.IsAny<TodoItem>()))
                .Callback<TodoItem>((item) => capturedTodoItem = item)
                .Returns(Task.CompletedTask); 

            // Act
            var result = await controller.PostTodoItem(newItem);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdItem = Assert.IsType<TodoItem>(createdAtActionResult.Value);
            Assert.NotNull(createdItem);
            Assert.Equal(newItem.Description, createdItem.Description);
            Assert.Equal(newItem.IsCompleted, createdItem.IsCompleted);

            // Verify that the CreateTodoItemAsync method was called with the expected input item
            mockTodoItemService.Verify(service => service.CreateTodoItemAsync(It.IsAny<TodoItem>()), Times.Once);
            Assert.Equal(newItem.Description, capturedTodoItem.Description);
            Assert.Equal(newItem.IsCompleted, capturedTodoItem.IsCompleted);
        }

        [Fact]
        public async Task MarkTodoItemAsComplete_ItemExists_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockTodoItemService = new Mock<ITodoItemService>();
            mockTodoItemService.Setup(service => service.MarkTodoItemAsCompleteAsync(id)).ReturnsAsync(true); 
           
            var controller = new TodoItemsController(mockTodoItemService.Object, null);

            // Act
            var result = await controller.MarkTodoItemAsComplete(id);

            // Assert
            Assert.IsType<NoContentResult>(result); // Expecting NoContent (204) status code
        }

        [Fact]
        public async Task MarkTodoItemAsComplete_ItemNotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var todoItemService = new Mock<ITodoItemService>();
            todoItemService.Setup(service => service.MarkTodoItemAsCompleteAsync(id)).ReturnsAsync(false); 

            var controller = new TodoItemsController(todoItemService.Object, null);

            // Act
            var result = await controller.MarkTodoItemAsComplete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result); // Expecting NotFound (404) status code
        }

    }
}
