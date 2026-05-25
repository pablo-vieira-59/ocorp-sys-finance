import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgApexchartsModule } from 'ng-apexcharts';
import { FinanceService, FixedIncomeInvestmentDto, InvestmentDto, VariableIncomeInvestmentDto } from '../../services/finance.service';
import { DonutChartOptions } from '../dashboard/dashboard.component';

@Component({
  selector: 'app-investments',
  standalone: true,
  imports: [CommonModule, FormsModule, NgApexchartsModule],
  templateUrl: './investments.component.html',
  styleUrl: './investments.component.css' // Using the same css file style name if they have one, fallback to component css
})
export class InvestmentsComponent implements OnInit {
  investments: InvestmentDto[] = [];
  investmentChartOptions?: DonutChartOptions;
  dashboard = { totalInvested: 0, profit: 0, profitPercentual: 0, activeInvestments: 0, currentValue: 0 }
  showAddModal = false;
  isLoading = false;
  investmentToAdd = {} as InvestmentDto;
  variableinvestimentToAdd = {} as VariableIncomeInvestmentDto;
  fixedInvestimentToAdd = {} as FixedIncomeInvestmentDto;
  error = '';

  investmentTypes = ["Renda Fixa", "Ações", "Fundos Imobiliários", "Criptomoedas"];

  constructor(
    private financeService: FinanceService,
    private cd: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.isLoading = true;
    this.financeService.getInvestments().subscribe({
      next: (data: InvestmentDto[]) => {
        this.investments = data;
        this.calcDashboard();
        this.initCharts();
        this.isLoading = false;
      },
      error: (err: any) => {
        this.error = err.error?.message || 'Erro ao carregar dados.';
        this.isLoading = false;
      }
    });
  }

  calcDashboard() {
    this.dashboard = {
      totalInvested: 0,
      profit: 0,
      profitPercentual: 0,
      activeInvestments: this.investments.length,
      currentValue: 0
    };

    this.dashboard.profit = this.dashboard.currentValue - this.dashboard.totalInvested;
    if (this.dashboard.totalInvested > 0) {
      this.dashboard.profitPercentual = this.dashboard.profit / this.dashboard.totalInvested;
    }
  }

  openAddInvestment() {
    this.investmentToAdd = {} as InvestmentDto;
    this.showAddModal = true;
    this.error = '';
  }

  editInvestment(investment: InvestmentDto) {
    this.investmentToAdd = investment;
    this.showAddModal = true;
    this.error = '';
  }

  deleteInvestment(investment: InvestmentDto) {
    if (!confirm('Deseja excluir este investimento?')) return;

    this.isLoading = true;
    this.financeService.deleteInvestment(investment.id).subscribe({
      next: () => {
        this.loadData();
        this.cd.detectChanges();
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  addInvestment() {
    this.isLoading = true;

    if (this.validate()) {
      this.financeService.addInvestment(this.investmentToAdd).subscribe({
        next: () => {
          this.loadData();
          this.investmentToAdd = {} as InvestmentDto;
          this.isLoading = false;
          this.showAddModal = false;
        },
        error: (err: any) => {
          this.error = err.error?.message || 'Erro ao adicionar investimento.';
          this.isLoading = false;
        }
      });
    } else {
      this.isLoading = false;
    }
  }

  initCharts() {
    this.investmentChartOptions = {
      series: this.investments.map(ti => 0),
      chart: {
        type: 'donut',
        height: 300,
        fontFamily: 'Plus Jakarta Sans, sans-serif'
      },
      dataLabels: {
        enabled: false,
      },
      labels: this.investments.map(ti => ti.name),
      title: {
        text: '',
        align: 'center'
      },
      legend: {
        position: 'bottom'
      },
      responsive: [
        {
          breakpoint: 1600,
          options: {
            chart: {
              width: '100%',
            },
            legend: {
              show: true,
              position: 'bottom'
            }
          }
        }
      ],
      theme: { mode: 'light', palette: 'palette1' }
    };
  }

  validate() {
    if (!this.investmentToAdd.name) {
      this.error = "É necessário preencher um nome.";
      return false;
    }
    if (!this.investmentToAdd.type) {
      this.error = "É necessário selecionar uma categoria.";
      return false;
    }

    if (this.investmentToAdd.type == this.investmentTypes[0]) {
      if (this.fixedInvestimentToAdd.initialAmount <= 0 || this.fixedInvestimentToAdd.initialAmount == null) {
        this.error = "Valor investido tem que ser maior que 0.";
        return false;
      }
      if (this.fixedInvestimentToAdd.currentAmount < 0 || this.fixedInvestimentToAdd.currentAmount == null) {
        this.error = "Valor atual tem que ser maior ou igual 0.";
        return false;
      }
      if (this.fixedInvestimentToAdd.interestRate < 0 || this.fixedInvestimentToAdd.interestRate == null) {
        this.error = "Taxa de juros tem que ser maior ou igual 0.";
        return false;
      }
    }
    else {
      if (this.variableinvestimentToAdd.quantity <= 0 || this.variableinvestimentToAdd.quantity == null) {
        this.error = "Quantidade tem que ser maior que 0.";
        return false;
      }
      if (this.variableinvestimentToAdd.averagePrice < 0 || this.variableinvestimentToAdd.averagePrice  == null) {
        this.error = "Preço médio tem que ser maior ou igual a 0.";
        return false;
      }
      if (this.variableinvestimentToAdd.currentQuotePrice < 0 || this.variableinvestimentToAdd.currentQuotePrice  == null) {
        this.error = "Preço atual da cota tem que ser maior ou igual a 0.";
        return false;
      }
      if (this.variableinvestimentToAdd.monthlyDividendYield < 0 || this.variableinvestimentToAdd.monthlyDividendYield  == null) {
        this.error = "Percentual de dividendos tem que ser maior ou igual a 0.";
        return false;
      }
    }

    this.error = "";
    return true;
  }
}
