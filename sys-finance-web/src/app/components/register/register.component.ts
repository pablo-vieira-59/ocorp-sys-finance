import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  data = { name: '', email: '', password: '', passwordConfirmation: '', terms: false };
  error = '';
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private cd: ChangeDetectorRef
  ) { }

  register() {
    this.isLoading = true;
    var isValid = this.validateForm();
    if (isValid) {
      this.authService.register(this.data).subscribe({
        next: () => {
          this.router.navigate(['/dashboard']);
          this.isLoading = false;
          this.cd.detectChanges();
        },
        error: (err) => {
          this.error = err.error?.message || 'Erro ao criar conta';
          this.isLoading = false;
          this.cd.detectChanges();
        }
      });
    }

    this.isLoading = false;
  }

  validateForm() {
    if (this.data.name == "" || this.data.name == null) {
      this.error = "Preencha o nome.";
      return false;
    }

    if (this.data.email == "" || this.data.email == null) {
      this.error = "Preencha o email.";
      return false;
    }

    if (this.data.password == "" || this.data.password == null) {
      this.error = "Preencha a senha.";
      return false;
    }

    if (this.data.password != this.data.passwordConfirmation) {
      this.error = "As senhas devem ser iguais.";
      return false;
    }

    if (this.data.terms != true) {
      this.error = "É necessário concordar com os termos e politica de privacidade.";
      return false;
    }

    return true;
  }
}
