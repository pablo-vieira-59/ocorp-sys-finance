import { ChangeDetectorRef, OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  NgApexchartsModule
} from 'ng-apexcharts';
import { FinanceService, IncomeDto } from '../../services/finance.service';
import { DonutChartOptions } from '../dashboard/dashboard.component';

@Component({
  selector: 'app-income',
  standalone: true,
  imports: [CommonModule, FormsModule, NgApexchartsModule],
  templateUrl: './income.component.html',
  styleUrl: './income.component.css'
})
export class IncomeComponent implements OnInit {
  incomes: IncomeDto[] = [];
  incomeChartOptions?: DonutChartOptions;
  discounts = { totalValue: 0, finalValue: 0, inss: 0, irrf: 0 }
  dashboard = { totalValue: 0, liquidValue: 0, topIncome: 0, topIncomeName: '', totalAfterDiscounts: 0, percentualAfterDiscounts: 0, discounts: 0 }
  showAddModal = false;
  isLoading = false;
  incomeToAdd = {} as IncomeDto;
  error = '';

  constructor(
    private financeService: FinanceService,
    private cd: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.isLoading = true;

    this.financeService.getIncomes().subscribe({
      next: (e) => {
        this.incomes = e;
        this.calculateDiscounts();
        this.initCharts();
        this.calcDashboard();
        this.isLoading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Erro ao carregar dados.';
        this.isLoading = false;
      }
    });


  }

  calcDashboard() {
    this.dashboard = {
      liquidValue: 0,
      percentualAfterDiscounts: 0,
      topIncome: 0,
      topIncomeName: '',
      totalAfterDiscounts: 0,
      totalValue: 0,
      discounts: 0
    };



    this.incomes.forEach(x => {
      if (this.dashboard.topIncome < x.amount) {
        this.dashboard.topIncome = x.amount;
        this.dashboard.topIncomeName = x.description;
      }

      this.dashboard.totalValue += x.amount;
      if (x.type != "Salário CLT") {
        this.dashboard.liquidValue += x.amount;
      }
    });
    console.log(this.dashboard.discounts);
    this.dashboard.liquidValue += this.discounts.finalValue;

    this.financeService.getExpenses().subscribe({
      next: (e) => {
        e.forEach(x => {
          this.dashboard.discounts += x.amount;
        });
        this.dashboard.totalAfterDiscounts = this.dashboard.liquidValue - this.dashboard.discounts;
        this.dashboard.percentualAfterDiscounts = this.dashboard.totalAfterDiscounts / this.dashboard.liquidValue
      },
      error: (err) => {
        this.error = err.error?.message || 'Erro ao carregar dados.';
      }
    });
  }

  editIncome(income: IncomeDto) {
    this.incomeToAdd = income;
    this.showAddModal = true;
    this.error = '';
  }

  deleteIncome(income: IncomeDto) {
    if (!confirm('Deseja excluir esta renda?')) return;

    this.isLoading = true;
    this.financeService.deleteIncome(income.id).subscribe({
      next: (e) => {
        this.loadData();
        this.cd.detectChanges();
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
      }
    });
  }

  addIncome() {
    this.isLoading = true;

    if (this.validate()) {
      this.financeService.addIncome(this.incomeToAdd).subscribe({
        next: () => {
          this.loadData();
          this.incomeToAdd = {} as IncomeDto;
          this.isLoading = false;
          this.showAddModal = false;
        },
        error: (err) => {
          this.error = err.error?.message || 'Erro ao adicionar despesa.';
          this.isLoading = false;
        }
      });
    }
  }

  initCharts() {
    this.incomeChartOptions = {
      tooltip: {},
      series: this.incomes.map(ti => ti.amount),
      chart: {
        type: 'donut',
        height: 300,
        fontFamily: 'Plus Jakarta Sans, sans-serif'
      },
      dataLabels: {
        enabled: false,

      },
      labels: this.incomes.map(ti => ti.description),
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

  calcularIRRF(baseCalculo: number) {

    if (baseCalculo <= 2428.80) {

      return 0;

    } else if (baseCalculo <= 2826.65) {

      return (baseCalculo * 0.075) - 182.16;

    } else if (baseCalculo <= 3751.05) {

      return (baseCalculo * 0.15) - 394.16;

    } else if (baseCalculo <= 4664.68) {

      return (baseCalculo * 0.225) - 675.49;

    } else {

      return (baseCalculo * 0.275) - 908.73;
    }
  }

  calcularINSS(salario: number) {

    if (salario <= 1518) {

      return salario * 0.075;

    } else if (salario <= 2793.88) {

      return (1518 * 0.075)
        + ((salario - 1518) * 0.09);

    } else if (salario <= 4190.83) {

      return (1518 * 0.075)
        + ((2793.88 - 1518) * 0.09)
        + ((salario - 2793.88) * 0.12);

    } else {

      return (1518 * 0.075)
        + ((2793.88 - 1518) * 0.09)
        + ((4190.83 - 2793.88) * 0.12)
        + ((salario - 4190.83) * 0.14);
    }
  }

  calculateDiscounts() {
    this.incomes.forEach(x => {
      if (x.type == "Salário CLT") {
        this.discounts.totalValue = x.amount;
      }
    })

    this.discounts.inss = this.calcularINSS(this.discounts.totalValue);
    var base = this.discounts.totalValue - this.discounts.inss - 607.2;
    this.discounts.irrf = this.calcularIRRF(base);
    this.discounts.finalValue = this.discounts.totalValue - this.discounts.inss - this.discounts.irrf;
  }

  validate() {
    if (this.incomeToAdd.type == "Salário CLT" && this.incomeToAdd.id == null) {
      var hasClt = this.incomes.find(x => x.type == "Salário CLT");

      if (hasClt) {
        this.error = "Já existe um salário CLT cadastrado."
        return false;
      }
    }

    if (this.incomeToAdd.amount <= 0 || this.incomeToAdd.amount == null) {
      this.error = "Valor tem que ser maior que 0."
      return false;
    }
    if (this.incomeToAdd.description == "" || this.incomeToAdd.description == null) {
      this.error = "É necessário preencher uma descrição."
      return false;
    }
    if (this.incomeToAdd.type == "" || this.incomeToAdd.type == null) {
      this.error = "É necessário selecionar uma categoria."
      return false;
    }
    this.error = "";
    return true;
  }
}
