import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { ExpenseDto } from './finance-models';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private apiUrl = environment.baseUrl + '/api/finance';

  constructor(private http: HttpClient) { }

  deleteExpense(id :string){return this.http.delete<any>(`${this.apiUrl}/expenses/${id}`); }
  getExpenses() { return this.http.get<ExpenseDto[]>(`${this.apiUrl}/expenses`); }
  addExpense(expense: ExpenseDto) { return this.http.post<ExpenseDto>(`${this.apiUrl}/expenses`, expense); }
}