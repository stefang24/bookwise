# BookWise - Service Booking Platform

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/stefang24/bookwise)
[![Tests Status](https://img.shields.io/badge/tests-passing-brightgreen)](https://github.com/stefang24/bookwise)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Angular Version](https://img.shields.io/badge/Angular-21-DD0031)](https://angular.io/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14+-316192)](https://www.postgresql.org/)

A modern full-stack application for managing personal and professional services with real-time communication, appointment scheduling, and a service provider catalog.

## Table of Contents
- [Overview](#overview)
- [Key Features](#key-features)
- [Visual Overview](#visual-overview)
- [Architecture](#architecture)
- [Technologies](#technologies)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Testing](#testing)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [Authors](#authors)
- [License](#license)
- [Project Status](#project-status)

## Overview

BookWise is a modern platform for managing professional services that enables secure and efficient interaction between service providers and customers. The platform provides complete appointment scheduling management with real-time communication, making the entire process transparent and straightforward for all participants.

**Ideal for:** hair salons, fitness centers, therapists, consultants, and all types of service businesses.

### Key Features

- **Role-based Authentication** - Admin, Provider, User with JWT security standards
- **Advanced Scheduling System** - Intelligent conflict detection, available slots, calendar view
- **Real-time Chat Communication** - Messages via SignalR WebSocket with HTTP fallback
- **Service Catalog** - Search, filter, and browse services with detailed information
- **Admin Dashboard** - Statistics, user management, platform monitoring
- **Schedule Management** - Flexible work schedule with daily availability hours
- **Push Notifications** - Alerts for scheduled appointments and messages
- **Security** - JWT tokens, BCrypt hashing, CORS protection, SQL injection prevention

## Visual Overview

### Key User Flows

**Customer Registration and Booking Flow**
```
Registration → Service Search → Provider Selection → View Available Slots 
→ Book Appointment → Confirmation → Chat with Provider
```

**Provider Management Flow**
```
Registration → Define Schedule → Add Services → Await Requests 
→ Chat with Client → Successful Service
```

**Admin Flow**
```
Login → Statistics Dashboard → User Management 
→ Transaction Overview → System Monitoring
```

## Architecture
```text
Browser (Angular)
    |
    | HTTP/JSON + WebSocket
    v
.NET API (ASP.NET Core)
    |
    +-- Controllers
    +-- JWT Authentication & Authorization
    +-- SignalR Hub (/hubs/chat)
    |
    v
Services (Business Logic)
    |
    v
Repositories + EF Core
    |
    v
PostgreSQL Database
```

## Technologies

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
- SignalR JS Client
- Vitest

## Prerequisites
- .NET SDK 8+
- Node.js 20+
- PostgreSQL 14+
- Git

## Installation

### 1. Clone the Repository
```bash
git clone https://github.com/stefang24/bookwise.git
cd bookwise
```

### 2. Backend Setup
Create the database in PostgreSQL:
```sql
CREATE DATABASE bookwise_db;
```

Configure the connection in backend/appsettings.Development.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bookwise_db;Username=bookwise_user;Password=your_secure_password"
  }
}
```

Install backend dependencies:
```bash
cd backend
dotnet restore
dotnet build
```

> **Note:** When the application starts, database tables will be created automatically (migrations run automatically). Default test accounts will also be created:
> - **User:** `user@gmail.com` / `password`
> - **Provider:** `provider@gmail.com` / `password`
> - **Admin:** `admin@gmail.com` / `password`

### 3. Frontend Setup
```bash
cd frontend
npm install
npm run build
```

## Running the Application

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

The application is available at:
- http://localhost:4200

## Project Structure
The frontend is organized using a feature-based architecture (each functional unit has its own feature module/folder with its own pages/components logic).
The backend is organized as a 3-layer architecture:
- Presentation Layer: Controllers
- Business Layer: Services
- Data Access Layer: Repositories + Data (EF Core)

```text
bookwise/
  backend/
    Controllers/      # Presentation Layer
    Services/         # Business Layer
    Repositories/     # Data Access Layer
    Data/             # Data Access Layer (DbContext, EF)
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

## Testing

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

## Usage

### For Users Looking for Services:

1. **Register** with an email and password
2. **Find a service** using search and filters (city, category, price)
3. **Select a provider** and view detailed information
4. **Choose a time slot** from the available slots in the calendar
5. **Book** and wait for confirmation
6. **Communicate** with the provider via chat before the service takes place

### For Service Providers:

1. **Register** as a provider
2. **Complete your profile** and add service descriptions
3. **Set your work schedule** for each day of the week
4. **Add services** with prices and duration
5. **Adjust** your schedule as requests come in
6. **Communicate** with customers via real-time chat

### For Administrators:

1. **Log in** with an admin account
2. **View the dashboard** with key metrics
3. **Manage users** - block, activate
4. **Review all appointments** and transactions
5. **Monitor the system** and performance

## Support

If you encounter a problem or have questions:

- **GitHub Issues:** [Open an issue](https://github.com/stefang24/bookwise/issues) with a detailed description of the problem
- **Discussions:** Use [GitHub Discussions](https://github.com/stefang24/bookwise/discussions) for general questions

## Roadmap

### Version 1.0 (Current)
- [x] Complete scheduling system
- [x] Real-time chat communication
- [x] Role-based authentication
- [x] Admin dashboard
- [x] Schedule management

### Version 1.1 (Planned)
- [ ] Mobile application (React Native)
- [ ] Online payments (Stripe integration)
- [ ] SMS notifications
- [ ] Calendar sync (Google, Outlook)
- [ ] Ratings and reviews

### Version 1.2 (Future)
- [ ] AI-powered recommendations
- [ ] Video consultations
- [ ] Multi-language support
- [ ] Billing and invoicing system
- [ ] Analytics and insights

## Contributing

Thank you for your interest in contributing to the BookWise project!

### How to Contribute:

1. **Fork** the repository
2. **Create a branch** for your feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** your changes (`git commit -m 'Add some AmazingFeature'`)
4. **Push** to the branch (`git push origin feature/AmazingFeature`)
5. **Open a Pull Request**

### Pull Request Requirements:
- Code must be consistent with the project style
- All tests must pass (`npm run test` for frontend, `dotnet test` for backend)
- Add tests for new features
- Update documentation if necessary
- Write clear commit messages

## Authors

- **Stefan Grujičić** - Initial work - [GitHub](https://github.com/stefang24)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

The MIT License allows you to freely use, modify, and distribute the software as long as you retain the original credit and license.

## Project Status

**Active**: The project is under active development.

Version: **1.0.0**  
Last updated: April 2026

- All systems are operational
- Tests are passing
- Documentation is up to date

If you would like to become a collaborator or maintainer, please contact us as described in the Support section.

---

**Thank you for using BookWise!**

For faster updates and news, follow us on [GitHub](https://github.com/stefang24/bookwise).
