import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { timer } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [CommonModule],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Default
  
})
export class HomeComponent {
   
  public copyMessage  = signal<string|null>(null);
    getDate = new Date().toLocaleTimeString();
    
    copyToClipboard(text: string): void {
    navigator.clipboard
      .writeText(text)
      .then(() => {
        this.copyMessage.set(`"${text}" copied to clipboard!`);
        timer(3000).subscribe(() => this.copyMessage.set(null));
      })
      .catch(() => {
        this.copyMessage.set('Failed to copy text. Please try again.');
      });
  }

}
