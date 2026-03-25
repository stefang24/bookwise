# BookWise - Platforma za zakazivanje usluga

Moderna full-stack aplikacija za upravljanje ličnim i profesionalnim uslugama sa real-time komunikacijom, zakazivanjem termina i katalogom dobavljača usluga.

## Sadržaj
- [Pregled](#pregled)
- [Arhitektura](#arhitektura)
- [Tehnologije](#tehnologije)
- [Preduslovi](#preduslovi)
- [Instalacija](#instalacija)
- [Pokretanje aplikacije](#pokretanje-aplikacije)
- [Struktura projekta](#struktura-projekta)
- [Testiranje](#testiranje)

## Pregled
BookWise je platforma koja omogućava:
- Dobavljačima usluga da objavljuju i vode svoje usluge
- Klijentima da pretražuju usluge i zakazuju termine
- Administratorima da upravljaju korisnicima i statistikama platforme

Glavne mogućnosti:
- Autentifikacija po ulogama (Admin, Dobavljač, Korisnik)
- Sistem zakazivanja termina
- Real-time chat preko SignalR
- Pretraga i filtriranje usluga
- Administratorski dashboard i statistika
- JWT bezbednost

## Arhitektura
```text
Browser (Angular)
    |
    | HTTP/JSON + WebSocket
    v
.NET API (ASP.NET Core)
    |
    +-- Kontroleri
    +-- JWT autentifikacija i autorizacija
    +-- SignalR Hub (/hubs/chat)
    |
    v
Servisi (biznis logika)
    |
    v
Repozitorijumi + EF Core
    |
    v
PostgreSQL baza
```

## Tehnologije

### Backend
- .NET 8 (ASP.NET Core)
- Entity Framework Core 8
- PostgreSQL
- JWT (auth)
- SignalR (real-time)
- Swagger/OpenAPI
- xUnit + Moq

### Frontend
- Angular 21 (standalone)
- TypeScript 5.9
- PrimeNG
- SignalR JS klijent
- Vitest

## Preduslovi
- .NET SDK 8+
- Node.js 20+
- PostgreSQL 14+
- Git

## Instalacija

### 1. Kloniranje repozitorijuma
```bash
git clone https://github.com/stefang24/bookwise.git
cd bookwise
```

### 2. Backend podešavanje
Kreiraj bazu u PostgreSQL:
```sql
CREATE DATABASE bookwise_db;
```

Podesi konekciju u backend/appsettings.Development.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bookwise_db;Username=bookwise_user;Password=your_secure_password"
  }
}
```

Instalacija backend zavisnosti:
```bash
cd backend
dotnet restore
dotnet build
```

> **Napomena:** Pri pokretanju aplikacije tabele baze će se automatski kreirati (migracije se automatski pokreću). Takođe, kreiraće se i default nalozi za testiranje:
> - **Korisnik:** `user@gmail.com` / `password`
> - **Dobavljač:** `provider@gmail.com` / `password`
> - **Admin:** `admin@gmail.com` / `password`

### 3. Frontend podešavanje
```bash
cd frontend
npm install
npm run build
```

## Pokretanje aplikacije

Terminal 1 (backend):
```bash
cd backend
dotnet run
```

Terminal 2 (frontend):
```bash
cd frontend
ng serve
```

Aplikacija je dostupna na:
- http://localhost:4200

## Struktura projekta
Frontend je organizovan po feature-based arhitekturi (svaka funkcionalna celina ima svoj feature modul/folder sa svojim pages/components logikom).
Backend je organizovan kao 3-slojna arhitektura:
- Presentation sloj: Controllers
- Business sloj: Services
- Data Access sloj: Repositories + Data (EF Core)

```text
bookwise/
  backend/
    Controllers/      # Presentation sloj
    Services/         # Business sloj
    Repositories/     # Data Access sloj
    Data/             # Data Access sloj (DbContext, EF)
    Models/
    Hubs/
    Program.cs
    backend.csproj
  frontend/
    src/
      app/
        features/
          auth/
          home/
          providers/
          provider/
          appointments/
          admin/
          profile/
          chat/
          layout/
          errors/
        shared/
          models/
          services/
          guards/
          config/
        app.routes.ts
        app.config.ts
      main.ts
      styles.css
    package.json
    angular.json
```

## Testiranje

Frontend:
```bash
cd frontend
npm run test
```

Backend:
```bash
cd backend
dotnet test
```

