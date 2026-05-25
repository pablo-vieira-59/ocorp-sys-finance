import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface ExpenseDto{
  id :string,
  description : string,
  amount : number,
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
  createdAt :Date,
  type:string,
  variable : VariableIncomeInvestmentDto,
  fixed : FixedIncomeInvestmentDto
}

export interface FixedIncomeInvestmentDto{
  id :string,
  investmentId :string,
  initialAmount:number,
  currentAmount:number,
  interestRate:number
}

export interface VariableIncomeInvestmentDto{
  id :string,
  investmentId :string,
  investedAmount:number,
  quantity:number,
  averagePrice:number,
  currentQuotePrice:number,
  monthlyDividendYield:number
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
  updateInvestment(id: string, investment: any) { return this.http.put<any>(`${this.apiUrl}/investments/${id}`, investment); }
  deleteInvestment(id: string) { return this.http.delete<any>(`${this.apiUrl}/investments/${id}`); }

  getAssets() { return this.http.get<AssetDto[]>(`${this.apiUrl}/assets`); }
  addAsset(asset: AssetDto) { return this.http.post<AssetDto>(`${this.apiUrl}/assets`, asset); }
  updateAsset(id: string, asset: AssetDto) { return this.http.put<AssetDto>(`${this.apiUrl}/assets/${id}`, asset); }
  deleteAsset(id: string) { return this.http.delete<any>(`${this.apiUrl}/assets/${id}`); }

  addAssetHistory(data: AssetHistoryDto) { return this.http.post<any>(`${this.apiUrl}/asset-history`, data); }
  deleteAssetHistory(id: string) { return this.http.delete<any>(`${this.apiUrl}/asset-history/${id}`); }

  getPatrimony() { return this.http.get<PatrimonySummaryDto>(`${this.apiUrl}/patrimony`); }

  getIncomes() { return this.http.get<IncomeDto[]>(`${this.apiUrl}/income`)}
  addIncome(income: IncomeDto) { return this.http.post<IncomeDto>(`${this.apiUrl}/income`, income); }
  deleteIncome(id :string){return this.http.delete<any>(`${this.apiUrl}/income/`+id); }

}
