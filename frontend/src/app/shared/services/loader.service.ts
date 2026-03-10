import { Injectable, signal, computed } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {
  private loadingKeys = signal<Set<string>>(new Set());

  fullscreen = computed<boolean>(() => this.loadingKeys().has('fullscreen'));

  isLoading(key: string): boolean {
    return this.loadingKeys().has(key);
  }

  show(key: string = 'fullscreen'): void {
    this.loadingKeys.update((keys: Set<string>) => {
      const updated: Set<string> = new Set(keys);
      updated.add(key);
      return updated;
    });
  }

  hide(key: string = 'fullscreen'): void {
    this.loadingKeys.update((keys: Set<string>) => {
      const updated: Set<string> = new Set(keys);
      updated.delete(key);
      return updated;
    });
  }
}
