import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-tools',
  standalone: true,
  imports: [CommonModule, FormsModule,],
  templateUrl: './tools.component.html',
  styleUrl: './tools.component.css'
})
export class ToolsComponent {
  uberOptions = [
    {name:'Uber X',price:1.5},
    {name:'Uber Comfort',price:1.9},
    {name:'Uber Black',price:2.4},
  ];

  data = {
    kmLiter:0,
    kmMonth : 0,
    gasPrice : 0,
    vehiclePrice : 0,
    dividendsYeild : 0,
    uberPrice : 0,
    ipva : 0,
    issurence : 0,
    maintence : 0,
    gasTotal : 0,
    uberTotal : 0,
    uberExpense : 0,
    vehicleTotal: 0,
    yeild:0,
    totalDiff : 0
  }

  calculate(){
    this.data.ipva = (this.data.vehiclePrice * 0.04) / 12;
    this.data.issurence = (this.data.vehiclePrice * 0.05) / 12;
    this.data.maintence = (this.data.vehiclePrice * 0.01) / 12;
    this.data.gasTotal = (this.data.kmMonth/this.data.kmLiter)*this.data.gasPrice;
    this.data.vehicleTotal = this.data.ipva + this.data.issurence + this.data.maintence + this.data.gasTotal;

    this.data.yeild = this.data.vehiclePrice * (this.data.dividendsYeild/100);
    this.data.uberExpense = this.data.uberPrice * this.data.kmMonth;
    this.data.uberTotal = this.data.uberExpense - this.data.yeild;

    this.data.totalDiff = Math.abs(this.data.vehicleTotal - this.data.uberTotal);
  }
}
