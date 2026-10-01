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