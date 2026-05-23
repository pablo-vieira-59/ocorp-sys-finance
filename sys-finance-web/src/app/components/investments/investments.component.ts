import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinanceService } from '../../services/finance.service';

@Component({
  selector: 'app-investments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './investments.component.html'
})
export class InvestmentsComponent implements OnInit {
  financeService = inject(FinanceService);
  items: any[] = [];
  newItem: any = { name: '', type: '', investedAmount: 0, currentValue: 0, date: new Date().toISOString().substring(0,10) };

  ngOnInit() { this.loadData(); }
  loadData() { this.financeService.getInvestments().subscribe(data => this.items = data); }
  
  add() {
    this.financeService.addInvestment(this.newItem).subscribe(() => {
      this.loadData();
      this.newItem = { name: '', type: '', investedAmount: 0, currentValue: 0, date: new Date().toISOString().substring(0,10) };
    });
  }
}
