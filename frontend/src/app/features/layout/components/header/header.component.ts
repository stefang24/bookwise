import { Component, inject, computed } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../../shared/services/auth.service';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, ButtonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  private authService: AuthService = inject(AuthService);

  isLoggedIn = computed<boolean>(() => this.authService.isLoggedIn());
  username = computed<string>(() => this.authService.username());
  userId = computed<number | null>(() => this.authService.userId());
  role = computed<string>(() => this.authService.role());

  onLogout(): void {
    this.authService.logout();
  }
}
