import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ExpensesComponent } from './components/expenses/expenses.component';
import { InvestmentsComponent } from './components/investments/investments.component';
import { AssetsComponent } from './components/assets/assets.component';
import { authGuard } from './guards/auth.guard';
import { AppLayoutComponent } from './components/app-layout/app-layout.component';
import { IncomeComponent } from './components/income/income.component';
import { ToolsComponent } from './components/tools/tools.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: '',
    component: AppLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'income', component: IncomeComponent, canActivate: [authGuard] },
      { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
      { path: 'expenses', component: ExpensesComponent, canActivate: [authGuard] },
      { path: 'investments', component: InvestmentsComponent, canActivate: [authGuard] },
      { path: 'assets', component: AssetsComponent, canActivate: [authGuard] },
      { path: 'tools', component: ToolsComponent, canActivate: [authGuard] },
      { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
      { path: '**', redirectTo: '/dashboard' }
    ]
  }
];
