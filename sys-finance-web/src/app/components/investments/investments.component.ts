import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgApexchartsModule } from 'ng-apexcharts';
import { FinanceService, InvestmentDto } from '../../services/finance.service';
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
  error = '';

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

    this.investments.forEach(x => {
      this.dashboard.totalInvested += x.investedAmount;
      this.dashboard.currentValue += x.currentValue;
    });

    this.dashboard.profit = this.dashboard.currentValue - this.dashboard.totalInvested;
    if (this.dashboard.totalInvested > 0) {
      this.dashboard.profitPercentual = this.dashboard.profit / this.dashboard.totalInvested;
    }
  }

  openAddInvestment() {
    this.investmentToAdd = {
      name: '',
      type: '',
      investedAmount: 0,
      currentValue: 0,
      monthlyDividendYield: 0,
      monthlyDividendYeild: 0,
      date: new Date().toISOString().substring(0, 10) as any
    } as InvestmentDto;
    this.showAddModal = true;
    this.error = '';
  }

  editInvestment(investment: InvestmentDto) {
    this.investmentToAdd = { ...investment }; // copy object
    this.investmentToAdd.monthlyDividendYield = this.investmentToAdd.monthlyDividendYield ?? this.investmentToAdd.monthlyDividendYeild ?? 0;
    this.investmentToAdd.monthlyDividendYeild = this.investmentToAdd.monthlyDividendYield;
    if (!this.investmentToAdd.date) {
      this.investmentToAdd.date = new Date().toISOString().substring(0,10) as any;
    } else {
      this.investmentToAdd.date = new Date(this.investmentToAdd.date).toISOString().substring(0,10) as any;
    }
    this.showAddModal = true;
    this.error = '';
  }

  deleteInvestment(investment: InvestmentDto) {
    if(!confirm('Deseja excluir este investimento?')) return;
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
    
    // Sync both fields to make sure we send the correct value to the backend
    if (this.investmentToAdd.monthlyDividendYield != null) {
      this.investmentToAdd.monthlyDividendYeild = this.investmentToAdd.monthlyDividendYield;
    } else if (this.investmentToAdd.monthlyDividendYeild != null) {
      this.investmentToAdd.monthlyDividendYield = this.investmentToAdd.monthlyDividendYeild;
    } else {
      this.investmentToAdd.monthlyDividendYield = 0;
      this.investmentToAdd.monthlyDividendYeild = 0;
    }

    if (this.validate()) {
      if (this.investmentToAdd.id) {
         this.financeService.updateInvestment(this.investmentToAdd.id, this.investmentToAdd).subscribe({
           next: () => {
             this.loadData();
             this.investmentToAdd = {} as InvestmentDto;
             this.isLoading = false;
             this.showAddModal = false;
           },
           error: (err: any) => {
             this.error = err.error?.message || 'Erro ao atualizar investimento.';
             this.isLoading = false;
           }
         });
      } else {
        if (!this.investmentToAdd.date) this.investmentToAdd.date = new Date().toISOString().substring(0,10) as any;
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
      }
    } else {
      this.isLoading = false;
    }
  }

  initCharts() {
    this.investmentChartOptions = {
      series: this.investments.map(ti => ti.currentValue),
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
    if (this.investmentToAdd.investedAmount == null || this.investmentToAdd.investedAmount <= 0) {
      this.error = "Valor investido deve ser maior que 0.";
      return false;
    }
    if (this.investmentToAdd.currentValue == null || this.investmentToAdd.currentValue <= 0) {
      this.error = "Valor atual deve ser maior que 0.";
      return false;
    }
    
    // Validate MonthlyDividendYeild/MonthlyDividendYield >= 0
    const yieldVal = this.investmentToAdd.monthlyDividendYield ?? this.investmentToAdd.monthlyDividendYeild;
    if (yieldVal == null || yieldVal < 0) {
      this.error = "Rendimento de dividendos mensais não pode ser menor que 0.";
      return false;
    }
    
    this.error = "";
    return true;
  }
}
