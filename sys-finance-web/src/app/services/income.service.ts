import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { IncomeDto } from './finance-models';

@Injectable({
  providedIn: 'root'
})
export class IncomeService {
  private apiUrl = environment.baseUrl + '/api/finance';

  constructor(private http: HttpClient) { }

  getIncomes() { return this.http.get<IncomeDto[]>(`${this.apiUrl}/income`); }
  addIncome(income: IncomeDto) { return this.http.post<IncomeDto>(`${this.apiUrl}/income`, income); }
  deleteIncome(id :string){return this.http.delete<any>(`${this.apiUrl}/income/${id}`); }
}