# BookWise - Platforma za zakazivanje usluga

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/stefang24/bookwise)
[![Tests Status](https://img.shields.io/badge/tests-passing-brightgreen)](https://github.com/stefang24/bookwise)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Angular Version](https://img.shields.io/badge/Angular-21-DD0031)](https://angular.io/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14+-316192)](https://www.postgresql.org/)

Moderna full-stack aplikacija za upravljanje ličnim i profesionalnim uslugama sa real-time komunikacijom, zakazivanjem termina i katalogom dobavljača usluga.

## Sadržaj
- [Pregled](#pregled)
- [Glavne mogućnosti](#glavne-mogućnosti)
- [Vizuelni prikaz](#vizuelni-prikaz)
- [Arhitektura](#arhitektura)
- [Tehnologije](#tehnologije)
- [Preduslovi](#preduslovi)
- [Instalacija](#instalacija)
- [Pokretanje aplikacije](#pokretanje-aplikacije)
- [Korišćenje](#korišćenje)
- [Struktura projekta](#struktura-projekta)
- [Testiranje](#testiranje)
- [Roadmap](#roadmap)
- [Kontribucije](#kontribucije)
- [Autori](#autori)
- [Licenca](#licenca)
- [Status projekta](#status-projekta)

## Pregled

BookWise je moderna platforma za upravljanje profesionalnim uslugama koja omogućava bezbedan i efikasan rad između pružalaca usluga i korisnika. Platforma omogućava kompletno upravljanje zakazivanjem termina sa real-time komunikacijom, što čini ceo proces transparentnim i jednostavnim za sve učesnike.

**Idealni za:** frizerske salone, fitnes centre, terapeute, konsultante, i sve vrste uslužnih delatnosti.

### Glavne mogućnosti

- **Autentifikacija po ulogama** - Admin, Dobavljač, Korisnik sa JWT bezbednosnim standardima
- **Napredni sistem zakazivanja** - Inteligentna detekcija konflikta, dostupni slotovi, kalendarski pregled
- **Real-time chat komunikacija** - Poruke preko SignalR WebSocket sa HTTP fallback-om
- **Katalog usluga** - Pretraga, filtriranje i pregled usluga sa detaljnim informacijama
- **Administratorski dashboard** - Statistika, upravljanje korisnicima, nadzor platforme
- **Upravljanje rasporedima** - Fleksibilan raspored rada sa dnevnim satima dostupnosti
- **Push notifikacije** - Obaveštenja o zakazanim terminima i porukama
- **Sigurnost** - JWT tokeni, BCrypt haširanje, CORS zaštita, SQL injection zaštita

## Vizuelni prikaz

### Key User Flows

**Korisnikov tok registracije i zakazivanja**
```
Registracija → Pretraga Usluga → Izbor Dobavljača → Prikaz Dostupnih Slotova 
→ Zakazivanje → Potvrda → Chat sa Dobavljačem
```

**Dobavljačev tok upravljanja**
```
Registracija → Definisanje Rasporeda → Dodavanje Usluga → Čekanje Zahteva 
→ Chat sa Klijentom → Uspešna Usluga
```

**Administratorski tok**
```
Login → Dashboard sa Statistikom → Upravljanje Korisnicima 
→ Pregled Transakcija → Sistem Monitoring
```

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

## Korišćenje

### Za korisnike koji traže usluge:

1. **Registrujte se** sa email-om i šifrom
2. **Pronađite uslugu** koristeći search i filtere (grad, kategorija, cena)
3. **Odaberite dobavljača** i vidite detaljne informacije
4. **Izaberite termin** iz dostupnih slotova u kalendaru
5. **Zakazite** i pričekajte potvrdu
6. **Komunicirajte** sa dobavljačem preko chat-a pre nego što dođe do usluge


### Za dobavljače usluga:

1. **Registrujte se** kao dobavljač
2. **Popunite profil** i dodajte opis usluga
3. **Postavite raspored** rada za svaki dan nedelje
4. **Dodajte usluge** sa cenama i trajanjem
5. **Prilagođavajte** raspored kako se zahtevi primaju
6. **Komunicirajte** sa korisnicima preko real-time chat-a

### Za administratore:

1. **Prijavite se** admin nalogom
2. **Pregledajte dashboard** sa ključnim metrikama
3. **Upravljajte korisnicima** - blokiranje, aktivacija
4. **Pregledajte sve termine** i transakcije
5. **Pratite sistem** i performanse

## Support

Ako naiđete na problem ili imate pitanja:

- **📋 GitHub Issues:** [Otvorite issue](https://github.com/stefang24/bookwise/issues) sa detaljnim opisom problema
- **💬 Diskusije:** Koristite [GitHub Discussions](https://github.com/stefang24/bookwise/discussions) za opšta pitanja

## Roadmap

### Verzija 1.0 (Trenutna)
- [x] Kompletan sistem zakazivanja
- [x] Real-time chat komunikacija
- [x] Role-based autentifikacija
- [x] Administratorski dashboard
- [x] Upravljanje rasporedima

### Verzija 1.1 (Planirana)
- [ ] Mobilna aplikacija (React Native)
- [ ] Plaćanje online (Stripe integracija)
- [ ] SMS notifikacije
- [ ] Kalendar sinhronizacija (Google, Outlook)
- [ ] Ocenjivanje i recenzije

### Verzija 1.2 (Buduće)
- [ ] AI-powered preporuke
- [ ] Video konsultacije
- [ ] Multi-language podrška
- [ ] Billing i invoicing sistem
- [ ] Analitika i insights

## Kontribucije

Zahvaljujemo vam na interesovanju da doprinesete BookWise projektu! 

### Kako da doprinesete:

1. **Fork** repozitorijum
2. **Kreirajte branch** za vašu feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** vaše promene (`git commit -m 'Add some AmazingFeature'`)
4. **Push** na branch (`git push origin feature/AmazingFeature`)
5. **Otvorite Pull Request**

### Zahtevi za Pull Request:
- Kod mora biti konzistentan sa stilom projekta
- Sve testove moraju da prođe (`npm run test` za frontend, `dotnet test` za backend)
- Dodajte testove za nove funkcionalnosti
- Ažurirajte dokumentaciju ako je potrebno
- Pišite jasne commit poruke


## Autori

- **Stefan Grujičić** - Inicijalni rad - [GitHub](https://github.com/stefang24)

## Licenca

Ovaj projekat je licenciran pod MIT licencom - pogledajte [LICENSE](LICENSE) datoteku za detalje.

MIT License dozvoljava vam da slobodno koristite, modifikujete i distribuirate softvare sve dok zadržite originalni kredit i licencu.

## Status projekta

**Aktivno**: Projekat je u aktivnom razvoju. 

Verzija: **1.0.0**  
Poslednja ažuriranja: april 2026

- Svi sistemi su operativni
- Testovi su prolazili
- Dokumentacija je ažurna

Ako želite da postanete saradnik ili održavač, molimo vas da nas kontaktirate kao što je navedeno u sekciji Support.

---

**Hvala što koristite BookWise!**

Za brže ažuriranja i vesti, pratite nas na [GitHub](https://github.com/stefang24/bookwise).

