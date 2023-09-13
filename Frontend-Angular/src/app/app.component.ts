import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TodoService } from './todo.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  
  items: any[] = [];
  description: string = '';

  public constructor(private httpClient: HttpClient,private todoService: TodoService,) {}

  getItems() {
    this.todoService.getTodoItems().subscribe(
      (response) => {
        this.items = response;
      },
      (error) => {
        // Displaying an error message
        console.error('Error fetching Todo Items:', error);
      }
    );
  }

  handleAdd() {
    // Creating a new Todo Item object
    const newTodoItem = {
      description: this.description,
      isCompleted: false,
    };

    this.todoService.addTodoItem(newTodoItem).subscribe(
      (response) => {
        this.items.push(response);
        this.description = ''; // Clear the input field
      },
      (error) => {
        console.error('Error creating Todo Item:', error);
      }
    );
  }

  handleClear() {
    this.description = '';
  }

  handleMarkAsComplete(item: any) :void{
    this.todoService.markAsComplete(item.id).subscribe(
      () => {
        item.isCompleted = true;
        console.log('Item marked as complete successfully.');
      },
      (error) => {
        item.errorMessage = item;
        console.error('Error marking item as complete:', error);
      }
    );
  }
}
