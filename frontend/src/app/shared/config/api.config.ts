export const API_BASE_URL: string = 'http://localhost:5227';

export function withApi(path: string): string {
  const normalizedPath: string = path.startsWith('/') ? path : `/${path}`;
  return `${API_BASE_URL}${normalizedPath}`;
}

export function toAssetUrl(path: string | null | undefined): string | null {
  if (!path) return null;
  if (path.startsWith('http://') || path.startsWith('https://')) return path;
  return withApi(path);
}
