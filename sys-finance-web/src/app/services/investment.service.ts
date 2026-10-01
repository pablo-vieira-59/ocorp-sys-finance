import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { InvestmentDto } from './finance-models';

@Injectable({
  providedIn: 'root'
})
export class InvestmentService {
  private apiUrl = environment.baseUrl + '/api/finance';

  constructor(private http: HttpClient) { }

  getInvestments() { return this.http.get<any[]>(`${this.apiUrl}/investments`); }
  addInvestment(investment: any) { return this.http.post<any>(`${this.apiUrl}/investments`, investment); }
  updateInvestment(id: string, investment: any) { return this.http.put<any>(`${this.apiUrl}/investments/${id}`, investment); }
  deleteInvestment(id: string) { return this.http.delete<any>(`${this.apiUrl}/investments/${id}`); }
}