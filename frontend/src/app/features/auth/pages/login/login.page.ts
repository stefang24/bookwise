import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../../shared/services/auth.service';
import { LoginRequest } from '../../../../shared/models/login-request.model';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { AuthResponse } from '../../../../shared/models/auth-response.model';
import { LoaderService } from '../../../../shared/services/loader.service';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink, InputTextModule, PasswordModule, ButtonModule, MessageModule],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css'
})
export class LoginPage {
  private authService: AuthService = inject(AuthService);
  private router: Router = inject(Router);
  private loaderService: LoaderService = inject(LoaderService);

  email = signal<string>('');
  password = signal<string>('');
  errorMessage = signal<string>('');

  get isLoading(): boolean {
    return this.loaderService.isLoading('login');
  }

  onLogin(): void {
    this.errorMessage.set('');
    this.loaderService.show('login');

    const request: LoginRequest = {
      email: this.email(),
      password: this.password()
    };

    this.authService.login(request).subscribe({
      next: (response: ResultResponse<AuthResponse>) => {
        this.loaderService.hide('login');
        if (response.success) {
          this.authService.setSession(response.data);
          this.router.navigate(['/home']);
        } else {
          this.errorMessage.set(response.message);
        }
      },
      error: () => {
        this.loaderService.hide('login');
        this.errorMessage.set('Login failed. Please try again.');
      }
    });
  }
}
