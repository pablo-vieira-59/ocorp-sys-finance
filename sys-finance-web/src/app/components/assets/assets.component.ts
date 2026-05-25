import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgApexchartsModule } from 'ng-apexcharts';
import { AssetDto, AssetHistoryDto, FinanceService, PatrimonySummaryDto } from '../../services/finance.service';
import { BarChartOptions, DonutChartOptions } from '../dashboard/dashboard.component';

@Component({
  selector: 'app-assets',
  standalone: true,
  imports: [CommonModule, FormsModule, NgApexchartsModule],
  templateUrl: './assets.component.html',
  styleUrl: './assets.component.css'
})
export class AssetsComponent implements OnInit {
  assets: AssetDto[] = [];
  assetHistories: AssetHistoryDto[] = [];
  patrimony: PatrimonySummaryDto | null = null;

  dashboard = { totalAssets: 0, totalInvestments: 0, overallTotal: 0, totalItems: 0 };

  assetChartOptions?: DonutChartOptions;
  historyChartOptions?: BarChartOptions;

  showAddAssetModal = false;
  showAddHistoryModal = false;
  isLoading = false;
  error = '';

  assetToAdd = {} as AssetDto;
  historyToAdd = {} as AssetHistoryDto;

  assetTypes = ['Imóvel', 'Veículo', 'Saldo Bancário', 'Outro'];

  constructor(
    private financeService: FinanceService,
    private cd: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.isLoading = true;
    this.financeService.getPatrimony().subscribe({
      next: (data) => {
        this.patrimony = data;
        this.assets = data.assets;
        this.assetHistories = (data.assetHistories || []).sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
        this.calcDashboard();
        this.initCharts();
        this.isLoading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Erro ao carregar dados.';
        this.isLoading = false;
      }
    });
  }

  calcDashboard() {
    if (!this.patrimony) return;
    this.dashboard = {
      totalAssets: this.patrimony.totalAssets,
      totalInvestments: this.patrimony.totalInvestments,
      overallTotal: this.patrimony.overallTotal,
      totalItems: this.assets.length
    };
  }

  // ---- Asset CRUD ----
  openAddAsset() {
    this.assetToAdd = {} as AssetDto;
    this.showAddAssetModal = true;
    this.error = '';
  }

  editAsset(asset: AssetDto) {
    this.assetToAdd = { ...asset };
    this.showAddAssetModal = true;
    this.error = '';
  }

  deleteAsset(asset: AssetDto) {
    if (!confirm('Deseja excluir este bem?')) return;
    this.isLoading = true;
    this.financeService.deleteAsset(asset.id!).subscribe({
      next: () => { this.loadData(); },
      error: () => { this.isLoading = false; }
    });
  }

  saveAsset() {
    if (!this.validateAsset()) return;
    this.isLoading = true;

    if (this.assetToAdd.id) {
      this.financeService.updateAsset(this.assetToAdd.id, this.assetToAdd).subscribe({
        next: () => {
          this.loadData();
          this.showAddAssetModal = false;
          this.assetToAdd = {} as AssetDto;
        },
        error: (err) => {
          this.error = err.error?.message || 'Erro ao atualizar bem.';
          this.isLoading = false;
        }
      });
    } else {
      this.financeService.addAsset(this.assetToAdd).subscribe({
        next: () => {
          this.loadData();
          this.showAddAssetModal = false;
          this.assetToAdd = {} as AssetDto;
        },
        error: (err) => {
          this.error = err.error?.message || 'Erro ao adicionar bem.';
          this.isLoading = false;
        }
      });
    }
  }

  validateAsset(): boolean {
    if (!this.assetToAdd.name) { this.error = 'É necessário preencher um nome.'; return false; }
    if (!this.assetToAdd.type) { this.error = 'É necessário selecionar um tipo.'; return false; }
    if (this.assetToAdd.estimatedValue == null || this.assetToAdd.estimatedValue <= 0) { this.error = 'Valor estimado deve ser maior que 0.'; return false; }
    this.error = '';
    return true;
  }

  // ---- History CRUD ----
  openAddHistory() {
    this.historyToAdd = {} as AssetHistoryDto;
    this.showAddHistoryModal = true;
    this.error = '';
  }

  saveHistory() {
    if (!this.historyToAdd.amount || this.historyToAdd.amount <= 0) {
      this.error = 'O valor deve ser maior que 0.';
      return;
    }
    if (!this.historyToAdd.date) {
      this.error = 'Selecione uma data.';
      return;
    }
    this.error = '';
    this.isLoading = true;
    this.financeService.addAssetHistory(this.historyToAdd).subscribe({
      next: () => {
        this.loadData();
        this.showAddHistoryModal = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Erro ao adicionar registro.';
        this.isLoading = false;
      }
    });
  }

  editHistory(h: AssetHistoryDto) {
    this.historyToAdd = h;
    this.showAddHistoryModal = true;
    this.error = '';
  }

  deleteHistory(history: AssetHistoryDto) {
    if (!confirm('Deseja excluir este registro do histórico?')) return;
    this.isLoading = true;
    this.financeService.deleteAssetHistory(history.id).subscribe({
      next: () => { this.loadData(); },
      error: () => { this.isLoading = false; }
    });
  }

  // ---- Charts ----
  initCharts() {
    if (this.assets.length > 0) {
      this.assetChartOptions = {
        tooltip : {},
        series: this.assets.map(a => a.estimatedValue),
        chart: { type: 'donut', height: 300, fontFamily: 'Plus Jakarta Sans, sans-serif' },
        dataLabels: { enabled: false },
        labels: this.assets.map(a => a.name),
        title: { text: '', align: 'center' },
        legend: { position: 'bottom' },
        responsive: [{ breakpoint: 1600, options: { chart: { width: '100%' }, legend: { show: true, position: 'bottom' } } }],
        theme: { mode: 'light', palette: 'palette1' }
      };
    }

    if (this.assetHistories.length > 0) {
      this.historyChartOptions = {
        series: [{ name: 'Patrimônio', data: this.assetHistories.map(x => x.amount) }],
        title: { text: '' },
        yaxis: {
          labels: { style: { fontSize: '14px', fontWeight: 500 }, show: false }
        },
        responsive: [{ breakpoint: 768, options: { yaxis: { labels: { show: false } } } }],
        chart: { type: 'area', height: 280, toolbar: { show: false }, fontFamily: 'Plus Jakarta Sans, sans-serif' },
        plotOptions: { bar: { horizontal: false, borderRadius: 16, barHeight: '10%', distributed: false } },
        dataLabels: {
          enabled: true, textAnchor: 'end', offsetX: 0,offsetY: -10,
          style: { fontSize: '14px', fontWeight: '800' },
          formatter: function (value: number) {
            return value.toLocaleString('pt-BR', {
              style: 'currency',
              currency: 'BRL'
            });
          }
        },
        xaxis: {
          categories: this.assetHistories.map(x =>
            new Date(x.date).toLocaleDateString('pt-BR', { month: '2-digit', year: 'numeric' })
          ),
          labels: { show: true },
          axisBorder: { show: true },
          axisTicks: { show: true }
        },
        grid: { show: true },
        tooltip: { enabled: true },
        legend: { show: false },
        theme: { mode: 'light', palette: 'palette1' }
      };
    }
  }
}
