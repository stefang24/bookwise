import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-internal-server-error-page',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './internal-server-error.page.html',
  styleUrl: './internal-server-error.page.css'
})
export class InternalServerErrorPage {}
