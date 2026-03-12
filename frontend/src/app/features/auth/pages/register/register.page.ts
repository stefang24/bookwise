import { Component, computed, inject, signal } from '@angular/core';
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
import { SelectButtonModule } from 'primeng/selectbutton';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink, InputTextModule, PasswordModule, ButtonModule, MessageModule, SelectButtonModule],
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
  confirmPassword = signal<string>('');
  firstName = signal<string>('');
  lastName = signal<string>('');
  role = signal<string>('User');
  errorMessage = signal<string>('');

  emailError = computed<string>(() => {
    const val: string = this.email();
    if (!val) return '';
    const emailRegex: RegExp = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailRegex.test(val) ? '' : 'Invalid email format';
  });

  usernameError = computed<string>(() => {
    const val: string = this.username();
    if (!val) return '';
    if (val.length < 3) return 'Username must be at least 3 characters';
    return '';
  });

  passwordError = computed<string>(() => {
    const val: string = this.password();
    if (!val) return '';
    if (val.length < 6) return 'Password must be at least 6 characters';
    return '';
  });

  confirmPasswordError = computed<string>(() => {
    const val: string = this.confirmPassword();
    if (!val) return '';
    return val === this.password() ? '' : 'Passwords do not match';
  });

  hasValidationErrors = computed<boolean>(() => {
    return !!this.emailError() || !!this.usernameError() || !!this.passwordError() || !!this.confirmPasswordError();
  });

  roleOptions = [
    { label: 'User', value: 'User' },
    { label: 'Provider', value: 'Provider' }
  ];

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
      lastName: this.lastName(),
      role: this.role()
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
