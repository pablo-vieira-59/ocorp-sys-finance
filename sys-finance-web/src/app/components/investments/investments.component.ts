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
  investmentToAdd = {
    fixed: {} as FixedIncomeInvestmentDto,
    variable: {} as VariableIncomeInvestmentDto
  } as InvestmentDto;
  error = '';

  investmentTypes = ["Renda Fixa", "Ações", "Fundos Imobiliários", "Criptomoedas"];
  variableInvestments : InvestmentDto[] = [];
  fixedInvestments :InvestmentDto[] = [];

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
        console.log(this.investments);
        this.fixedInvestments = [];
        this.variableInvestments = [];
        this.investments.forEach(i => {
          if(i.type == this.investmentTypes[0]){
            this.fixedInvestments.push(i);
          }
          else{
            this.variableInvestments.push(i);
          }
        });

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
    var totalInvested = 0;
    this.investments.forEach(x => {
      if (x.type == this.investmentTypes[0]) {
        totalInvested += x.fixed.currentAmount;
      }
      else {
        (totalInvested += x.variable.quantity * x.variable.currentQuotePrice).toFixed(2);
      }
    });

    var initialInvestment = 0;
    this.investments.forEach(x => {
      if (x.type == this.investmentTypes[0]) {
        initialInvestment += x.fixed.initialAmount;
      }
      else {
        (initialInvestment += x.variable.investedAmount).toFixed(2);
      }
    });

    var profit = totalInvested - initialInvestment;
    var profitPercentual = profit / initialInvestment;

    this.dashboard = {
      totalInvested: initialInvestment,
      profit: profit,
      profitPercentual: profitPercentual,
      activeInvestments: this.investments.length,
      currentValue: totalInvested
    };
  }

  openAddInvestment() {
    this.investmentToAdd = {
      fixed: {} as FixedIncomeInvestmentDto,
      variable: {} as VariableIncomeInvestmentDto
    } as InvestmentDto;

    this.showAddModal = true;
    this.error = '';
  }

  editInvestment(investment: InvestmentDto) {
    if (investment.fixed == null) investment.fixed = {} as FixedIncomeInvestmentDto;
    if (investment.variable == null) investment.variable = {} as VariableIncomeInvestmentDto;

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
          this.cd.detectChanges();

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
      series: this.investments.map(ti => {
        let total = 0;

        if (ti.type == this.investmentTypes[0]) {
          total += Number(ti.fixed.currentAmount);
        }

        else {
          const variableTotal =
            Number(ti.variable.currentQuotePrice) *
            Number(ti.variable.quantity);

          total += variableTotal;
        }

        return Math.round(total * 100) / 100;
      }),
      tooltip: {

      },
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
      if (this.investmentToAdd.fixed.initialAmount <= 0 || this.investmentToAdd.fixed.initialAmount == null) {
        this.error = "Valor investido tem que ser maior que 0.";
        return false;
      }
      if (this.investmentToAdd.fixed.currentAmount < 0 || this.investmentToAdd.fixed.currentAmount == null) {
        this.error = "Valor atual tem que ser maior ou igual 0.";
        return false;
      }
      if (this.investmentToAdd.fixed.interestRate < 0 || this.investmentToAdd.fixed.interestRate == null) {
        this.error = "Taxa de juros tem que ser maior ou igual 0.";
        return false;
      }
    }
    else {
      if (this.investmentToAdd.variable.quantity <= 0 || this.investmentToAdd.variable.quantity == null) {
        this.error = "Quantidade tem que ser maior que 0.";
        return false;
      }
      if (this.investmentToAdd.variable.averagePrice < 0 || this.investmentToAdd.variable.averagePrice == null) {
        this.error = "Preço médio tem que ser maior ou igual a 0.";
        return false;
      }
      if (this.investmentToAdd.variable.currentQuotePrice < 0 || this.investmentToAdd.variable.currentQuotePrice == null) {
        this.error = "Preço atual da cota tem que ser maior ou igual a 0.";
        return false;
      }
      if (this.investmentToAdd.variable.monthlyDividendYield < 0 || this.investmentToAdd.variable.monthlyDividendYield == null) {
        this.error = "Percentual de dividendos tem que ser maior ou igual a 0.";
        return false;
      }
    }

    this.error = "";
    return true;
  }
}
