import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../../shared/services/auth.service';
import { RegisterRequest } from '../../../../shared/models/register-request.model';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { AuthResponse } from '../../../../shared/models/auth-response.model';
import { LoaderService } from '../../../../shared/services/loader.service';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink, InputTextModule, PasswordModule, ButtonModule, MessageModule],
  templateUrl: './register.page.html',
  styleUrl: './register.page.css'
})
export class RegisterPage {
  private authService: AuthService = inject(AuthService);
  private router: Router = inject(Router);
  private loaderService: LoaderService = inject(LoaderService);

  email = signal<string>('');
  username = signal<string>('');
  password = signal<string>('');
  firstName = signal<string>('');
  lastName = signal<string>('');
  errorMessage = signal<string>('');

  get isLoading(): boolean {
    return this.loaderService.isLoading('register');
  }

  onRegister(): void {
    this.errorMessage.set('');
    this.loaderService.show('register');

    const request: RegisterRequest = {
      email: this.email(),
      username: this.username(),
      password: this.password(),
      firstName: this.firstName(),
      lastName: this.lastName()
    };

    this.authService.register(request).subscribe({
      next: (response: ResultResponse<AuthResponse>) => {
        this.loaderService.hide('register');
        if (response.success) {
          this.authService.setSession(response.data);
          this.router.navigate(['/home']);
        } else {
          this.errorMessage.set(response.message);
        }
      },
      error: () => {
        this.loaderService.hide('register');
        this.errorMessage.set('Registration failed. Please try again.');
      }
    });
  }
}
