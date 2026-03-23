import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Router } from '@angular/router';
import { HeaderComponent } from '../components/header/header.component';
import { FooterComponent } from '../components/footer/footer.component';

@Component({
  selector: 'app-layout',
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './layout.page.html',
  styleUrl: './layout.page.css'
})
export class LayoutPage {
  constructor(private readonly router: Router) {}

  isHomeRoute(): boolean {
    return this.router.url === '/' || this.router.url.startsWith('/home');
  }
}
