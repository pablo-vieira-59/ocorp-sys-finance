import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AssetDto, AssetHistoryDto, FinanceService, InvestmentDto, PatrimonySummaryDto } from '../../services/finance.service';
import { AuthService } from '../../services/auth.service';
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
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { InvestmentsComponent } from '../investments/investments.component';

export type BarChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  title: ApexTitleSubtitle;
  responsive: ApexResponsive[];
  theme: ApexTheme;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  legend: ApexLegend;
  tooltip: ApexTooltip;
  grid: ApexGrid;
  yaxis: ApexYAxis;
};

export type DonutChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  labels: string[];
  title: ApexTitleSubtitle;
  responsive: ApexResponsive[];
  theme: ApexTheme;
  dataLabels: ApexDataLabels;
  legend: ApexLegend;
  tooltip: ApexTooltip;
};

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, NgApexchartsModule, RouterLink],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  financeService = inject(FinanceService);
  authService = inject(AuthService);
  patrimony: PatrimonySummaryDto = {
    assets: [] as AssetDto[],
    investments: [] as InvestmentDto[],
    assetHistories: [] as AssetHistoryDto[]
  } as PatrimonySummaryDto;
  userName = '';
  assetHistoryChartOptions?: BarChartOptions;
  assetTypeChartOptions?: DonutChartOptions;
  investmentTypeChartOptions?: DonutChartOptions;

  constructor(private router: Router) { }

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => {
      if (user) this.userName = user.name || 'Usuário';
    });
    this.financeService.getPatrimony().subscribe({
      next: (data) => {
        this.patrimony = data;
        if (this.patrimony && this.patrimony.assetHistories) {
          this.patrimony.assetHistories.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
        }
        console.log(data);
        this.initCharts();
      }
    });
  }

  getExpensesBarSize() {
    var percentual = (this.patrimony.totalExpenses / this.patrimony.totalIncome) * 100;
    if (percentual > 100) percentual = 100;
    return "width:" + percentual.toString() + "%";
  }

  initCharts() {
    this.assetHistoryChartOptions = {
      series: [
        {
          name: 'Valor',
          data: this.patrimony.assetHistories.map(x => x.amount)
        }
      ],

      title: {
        text: ''
      },

      responsive: [
        {
          breakpoint: 768,
          options: {
            yaxis: {
              labels: {
                show: false
              }
            }
          }
        }
      ],

      chart: {
        type: 'area',
        height: 280,
        toolbar: {
          show: false
        },
        fontFamily: 'Plus Jakarta Sans, sans-serif'
      },

      plotOptions: {
      },

      dataLabels: {
        enabled: true,
        textAnchor: "end",
        offsetX: 0,
        offsetY: -10,
        style: {
          fontSize: '14px',
          fontWeight: '500'
        },
        formatter: function (value: number) {
          return value.toLocaleString('pt-BR', {
            style: 'currency',
            currency: 'BRL'
          });
        }
      },

      yaxis: {
        labels: {
          style: {
            fontSize: '14px',
            fontWeight: 500
          },
          show: false
        }
      },

      xaxis: {
        categories: this.patrimony.assetHistories.map(x => new Date(x.date).toLocaleDateString('pt-BR', {
          month: '2-digit',
          year: 'numeric'
        })),

        labels: {
          show: true
        },

        axisBorder: {
          show: true
        },

        axisTicks: {
          show: true
        }
      },

      grid: {
        show: true
      },

      tooltip: {
        enabled: true
      },

      legend: {
        show: false
      },

      theme: {
        mode: 'light', palette: 'palette1'
      }
    };

    this.assetTypeChartOptions = {
      tooltip: {},
      series: this.patrimony.assets.map(ti => ti.estimatedValue),
      chart: {
        type: 'donut',
        height: 300,
        fontFamily: 'Plus Jakarta Sans, sans-serif'
      },
      dataLabels: {
        enabled: false,

      },
      labels: this.patrimony.assets.map(ti => ti.name),
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

    this.investmentTypeChartOptions = {
      tooltip: {},
      series: this.patrimony.investments.map(ti => {
        let total = 0;

        if (ti.type == 'Renda Fixa') {
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
      chart: {
        type: 'donut',
        height: 300,
        fontFamily: 'Plus Jakarta Sans, sans-serif'
      },
      dataLabels: {
        enabled: false,
      },
      labels: this.patrimony.investments.map(ti => ti.name),
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
}
