import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AssetDto, FinanceService } from '../../services/finance.service';

@Component({
  selector: 'app-assets',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './assets.component.html'
})
export class AssetsComponent implements OnInit {
  financeService = inject(FinanceService);
  assets: any[] = [];
  newAsset: AssetDto = {id:null, name: '', description: '', estimatedValue: 0, type: '' };

  patrimony: any;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.financeService.getAssets().subscribe(data => this.assets = data);
    this.financeService.getPatrimony().subscribe(data => this.patrimony = data);
  }

  addAsset() {
    console.log(this.newAsset);
    this.financeService.addAsset(this.newAsset).subscribe(() => {
      this.loadData();
      this.newAsset = { id:null, name: '', description: '', estimatedValue: 0, type: '' };
    });
  }
}
