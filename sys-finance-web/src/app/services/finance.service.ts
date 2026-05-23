import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface ExpenseDto{
  id :string,
  description : string,
  amount : number,
  date : Date,
  category : string,
  userId :string
}

export interface IncomeDto{
  id :string,
  description:string,
  amount:number;
  discounts:number,
  type:string,
  userId:string
}

export interface InvestmentDto{
  id :string,
  name:string,
  investedAmount:number,
  currentValue:number,
  date :Date,
  category:string
}

export interface AssetDto{
  id?:string|null,
  name:string,
  description:string,
  estimatedValue:number,
  type:string
}

export interface AssetHistoryDto{
  id:string;
  userId:string;
  amount:number;
  date:Date
}

export interface PatrimonySummaryDto{
  totalInvestments :number,
  totalAssets:number,
  overallTotal:number,
  totalExpenses:number,
  totalIncome:number,
  investments:InvestmentDto[],
  assets:AssetDto[],
  assetHistories:AssetHistoryDto[]
}

@Injectable({
  providedIn: 'root'
})
export class FinanceService {
  private apiUrl = environment.baseUrl + '/api/finance';

  constructor(private http: HttpClient) { }

  deleteExpense(id :string){return this.http.delete<any>(`${this.apiUrl}/expenses/`+id); }
  getExpenses() { return this.http.get<ExpenseDto[]>(`${this.apiUrl}/expenses`); }
  addExpense(expense: ExpenseDto) { return this.http.post<ExpenseDto>(`${this.apiUrl}/expenses`, expense); }

  getInvestments() { return this.http.get<any[]>(`${this.apiUrl}/investments`); }
  addInvestment(investment: any) { return this.http.post<any>(`${this.apiUrl}/investments`, investment); }

  getAssets() { return this.http.get<any[]>(`${this.apiUrl}/assets`); }
  addAsset(asset: AssetDto) { return this.http.post<AssetDto>(`${this.apiUrl}/assets`, asset); }

  getPatrimony() { return this.http.get<PatrimonySummaryDto>(`${this.apiUrl}/patrimony`); }

  getIncomes() { return this.http.get<IncomeDto[]>(`${this.apiUrl}/income`)}
  addIncome(income: IncomeDto) { return this.http.post<IncomeDto>(`${this.apiUrl}/income`, income); }
  deleteIncome(id :string){return this.http.delete<any>(`${this.apiUrl}/income/`+id); }

}
