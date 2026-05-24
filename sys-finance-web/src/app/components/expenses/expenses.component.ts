import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExpenseDto, FinanceService } from '../../services/finance.service';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexXAxis,
  ApexTitleSubtitle,
  ApexDataLabels,
  ApexFill,
  ApexYAxis,
  ApexTooltip,
  ApexStroke,
  ApexNonAxisChartSeries,
  ApexResponsive,
  ApexTheme,
  NgApexchartsModule,
  ApexPlotOptions,
  ApexLegend,
  ApexGrid
} from 'ng-apexcharts';
import { BarChartOptions, DonutChartOptions } from '../dashboard/dashboard.component';


@Component({
  selector: 'app-expenses',
  standalone: true,
  imports: [CommonModule, FormsModule, NgApexchartsModule],
  templateUrl: './expenses.component.html'
})
export class ExpensesComponent implements OnInit {
  topItemsChartOptions?: DonutChartOptions;
  topCategoriesChartOptions?: BarChartOptions;
  expenses: ExpenseDto[] = [];
  categoriesExpenses: ExpenseDto[] = [];
  isLoading = false;
  showCreateModal = false;
  error = '';
  expenseToAdd = {} as ExpenseDto;
  mothlyExpense = { totalExpenses: 0, percentalOfIncome: 0 }
  highestCost = { amount: 0, category: '' }
  mostExpensiveCategory = { category: '', percentual: 0 }

  constructor(
    private financeService: FinanceService,
    private cd: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.isLoading = true;

    this.financeService.getExpenses().subscribe({
      next: (e) => {
        this.expenses = e;
        this.mapExpensesCategories();
        this.initCharts();
        this.calcDashboard();
        this.isLoading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Erro no login';
        this.isLoading = false;
      }
    });
  }

  calcDashboard() {
    this.mothlyExpense = { totalExpenses: 0, percentalOfIncome: 0 }
    this.highestCost = { amount: 0, category: '' }
    this.mostExpensiveCategory = { category: '', percentual: 0 }
    
    this.expenses.forEach(x => {
      this.mothlyExpense.totalExpenses += x.amount;
      if (x.amount > this.highestCost.amount) {
        this.highestCost.amount = x.amount;
        this.highestCost.category = x.description;
      }
    });

    if (this.categoriesExpenses.length > 0) {
      this.mostExpensiveCategory.category = this.categoriesExpenses[0].category;

      var total = 0;
      this.categoriesExpenses.forEach(x => {
        total += x.amount;
      });
      this.mostExpensiveCategory.percentual = (this.categoriesExpenses[0].amount / total);
    }

    this.financeService.getIncomes().subscribe({
      next: (e) => {
        var totalIncome = 0;
        e.forEach(x => {
          totalIncome += x.amount;
        });
        this.mothlyExpense.percentalOfIncome = this.mothlyExpense.totalExpenses / totalIncome;
      },
      error: (err) => {
        this.error = err.error?.message || 'Erro no login';
        this.isLoading = false;
      }
    })
  }

  mapExpensesCategories() {
    this.categoriesExpenses = [];

    this.expenses.forEach(expense => {
      const existingCategory = this.categoriesExpenses.find(
        x => x.category === expense.category
      );

      if (existingCategory) {
        existingCategory.amount += expense.amount;
      } else {
        this.categoriesExpenses.push({
          description: expense.category,
          category: expense.category,
          amount: expense.amount,
          date: expense.date,
          id: expense.id,
          userId: expense.userId
        });
      }
    });
    this.categoriesExpenses.sort((a, b) => b.amount - a.amount)
  }

  initCharts() {
    this.topItemsChartOptions = {
      series: this.expenses.map(ti => ti.amount),
      chart: {
        type: 'donut',
        height: 300,
        fontFamily: 'Plus Jakarta Sans, sans-serif'
      },
      dataLabels: {
        enabled: false,

      },
      labels: this.expenses.map(ti => ti.description),
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

    this.topCategoriesChartOptions = {
      series: [
        {
          name: 'Valor',
          data: this.categoriesExpenses.map(x => x.amount)
        }
      ],

      title: {
        text: ''
      },

      responsive: [],

      chart: {
        type: 'bar',
        height: 280,
        toolbar: {
          show: false
        },
        fontFamily: 'Plus Jakarta Sans, sans-serif'
      },

      plotOptions: {
        bar: {
          horizontal: true,
          borderRadius: 4,
          barHeight: '40%',
          distributed: false
        }
      },



      dataLabels: {
        enabled: true,
        textAnchor: "end",
        offsetX: 100,
        style: {
          colors: ['#000000'],
          fontSize: '14px',
          fontWeight: '800'
        },

        formatter: function (value: number) {
          return `$${value.toFixed(2)}`;
        }
      },

      xaxis: {
        categories: this.categoriesExpenses.map(x => x.category),

        labels: {
          show: false
        },

        axisBorder: {
          show: false
        },

        axisTicks: {
          show: false
        }
      },

      yaxis: {
        labels: {
          style: {
            fontSize: '14px',
            fontWeight: 500
          }
        }
      },

      grid: {
        show: false
      },

      tooltip: {
        enabled: false
      },

      legend: {
        show: false
      },

      theme: {
        mode: 'light', palette: 'palette1'
      }
    };
  }

  editExpense(expense: ExpenseDto) {
    this.showCreateModal = true;
    this.expenseToAdd = expense;
  }

  deleteExpense(expense: ExpenseDto) {
    this.isLoading = true;
    this.financeService.deleteExpense(expense.id).subscribe({
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

  addExpense() {
    this.isLoading = true;

    if (this.validateExpense()) {
      this.financeService.addExpense(this.expenseToAdd).subscribe({
        next: () => {
          this.loadData();
          this.expenseToAdd = {} as ExpenseDto;
          this.isLoading = false;
          this.showCreateModal = false;
        },
        error: (err) => {
          this.error = err.error?.message || 'Erro ao adicionar despesa.';
          this.isLoading = false;
        }
      });
    }
  }

  validateExpense() {
    if (this.expenseToAdd.amount <= 0 || this.expenseToAdd.amount == null) {
      this.error = "Valor tem que ser maior que 0."
      return false;
    }
    if (this.expenseToAdd.description == "" || this.expenseToAdd.description == null) {
      this.error = "É necessário preencher uma descrição."
      return false;
    }
    if (this.expenseToAdd.category == "" || this.expenseToAdd.category == null) {
      this.error = "É necessário selecionar uma categoria."
      return false;
    }
    this.error = "";
    return true;
  }
}
