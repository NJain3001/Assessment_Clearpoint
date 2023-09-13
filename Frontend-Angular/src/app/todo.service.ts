import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TodoService {
  private apiUrl = 'https://localhost:44397/api/todoitems'; // Replace with your API URL

  constructor(private httpClient: HttpClient) {}

  markAsComplete(itemId: string): Observable<void> {
    const url = `${this.apiUrl}/mark-as-complete/${itemId}`;
    return this.httpClient.put<void>(url, {});
  }
  getTodoItems(): Observable<any[]> {
    return this.httpClient.get<any[]>(this.apiUrl);
  }

  addTodoItem(newTodoItem: any): Observable<any> {
    return this.httpClient.post<any>(this.apiUrl, newTodoItem);
  }
}
