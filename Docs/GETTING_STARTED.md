# Getting Started - Big Job Hunter Pro

## Overview

This guide will help you start building Big Job Hunter Pro, a full-stack gamified job tracking application designed to help you master the following technologies for technical interviews:

- **Microsoft SQL Server** - Database
- **.NET 8 ASP.NET Core** - Backend API
- **C# OOP & SOLID Principles** - Code architecture
- **React & TypeScript** - Frontend
- **Docker** - Containerization
- **CI/CD** - GitHub Actions
- **Testing** - xUnit, Vitest, React Testing Library
- **Git/GitHub** - Version control
- **LINQ** - Data querying

## Architecture Overview

- **Backend**: Clean Architecture (Onion) with CQRS pattern using MediatR
- **Frontend**: React 18 + TypeScript with Redux Toolkit
- **Database**: MS SQL Server with EF Core Code-First
- **Authentication**: ASP.NET Core Identity + JWT tokens
- **Deployment**: Azure Free Tier (<$10/month)

## Quick Start

### Prerequisites

Before you begin, ensure you have installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)
- A code editor (VS Code, Visual Studio, or Rider)

### Step 1: Set Up Backend (.NET Solution)

```bash
# Navigate to project root
cd C:\Users\Inferno\Dev\BigJobHunterPro

# Create solution and projects
dotnet new sln -n BigJobHunterPro
dotnet new classlib -n Domain -o src/Domain
dotnet new classlib -n Application -o src/Application
dotnet new classlib -n Infrastructure -o src/Infrastructure
dotnet new webapi -n WebAPI -o src/WebAPI

# Add projects to solution
dotnet sln add src/Domain/Domain.csproj
dotnet sln add src/Application/Application.csproj
dotnet sln add src/Infrastructure/Infrastructure.csproj
dotnet sln add src/WebAPI/WebAPI.csproj

# Add project references (establishing Clean Architecture dependencies)
cd src/Application
dotnet add reference ../Domain/Domain.csproj
cd ../Infrastructure
dotnet add reference ../Application/Application.csproj
cd ../WebAPI
dotnet add reference ../Infrastructure/Infrastructure.csproj
cd ../../..
```

### Step 2: Install Backend NuGet Packages

```bash
# Domain layer (no dependencies - pure business logic)
cd src/Domain
# No packages needed initially

# Application layer
cd ../Application
dotnet add package MediatR
dotnet add package FluentValidation
dotnet add package AutoMapper
dotnet add package Microsoft.Extensions.Logging.Abstractions

# Infrastructure layer
cd ../Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# WebAPI layer
cd ../WebAPI
dotnet add package MediatR
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

cd ../..
```

### Step 3: Set Up Frontend (React + TypeScript)

```bash
# Create Vite React TypeScript project
npm create vite@latest bigjobhunterpro-web -- --template react-ts

# Navigate to frontend directory
cd bigjobhunterpro-web

# Install core dependencies
npm install

# Install state management and API libraries
npm install @reduxjs/toolkit react-redux axios

# Install UI and styling libraries
npm install react-router-dom

# Install development dependencies
npm install -D @types/node vitest @testing-library/react @testing-library/jest-dom
```

### Step 4: Create Test Projects

```bash
# Return to project root
cd ..

# Create test projects
dotnet new xunit -n Domain.UnitTests -o tests/Domain.UnitTests
dotnet new xunit -n Application.IntegrationTests -o tests/Application.IntegrationTests
dotnet new xunit -n WebAPI.FunctionalTests -o tests/WebAPI.FunctionalTests

# Add test projects to solution
dotnet sln add tests/Domain.UnitTests/Domain.UnitTests.csproj
dotnet sln add tests/Application.IntegrationTests/Application.IntegrationTests.csproj
dotnet sln add tests/WebAPI.FunctionalTests/WebAPI.FunctionalTests.csproj

# Add project references for tests
cd tests/Domain.UnitTests
dotnet add reference ../../src/Domain/Domain.csproj
dotnet add package FluentAssertions

cd ../Application.IntegrationTests
dotnet add reference ../../src/Application/Application.csproj
dotnet add reference ../../src/Infrastructure/Infrastructure.csproj
dotnet add package FluentAssertions
dotnet add package Microsoft.EntityFrameworkCore.InMemory

cd ../WebAPI.FunctionalTests
dotnet add reference ../../src/WebAPI/WebAPI.csproj
dotnet add package FluentAssertions
dotnet add package Microsoft.AspNetCore.Mvc.Testing

cd ../..
```

### Step 5: Set Up Docker Compose

Create `docker-compose.yml` in the project root:

```yaml
version: '3.8'

services:
  # SQL Server Database
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: bigjobhunter-db
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: YourStrong!Passw0rd
      MSSQL_PID: Developer
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
    healthcheck:
      test: /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd" -Q "SELECT 1"
      interval: 10s
      timeout: 5s
      retries: 5

  # Backend API (uncomment after Phase 1 implementation)
  # api:
  #   build:
  #     context: .
  #     dockerfile: src/WebAPI/Dockerfile
  #   container_name: bigjobhunter-api
  #   ports:
  #     - "5000:80"
  #   environment:
  #     ASPNETCORE_ENVIRONMENT: Development
  #     ConnectionStrings__DefaultConnection: "Server=db;Database=BigJobHunterPro;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
  #   depends_on:
  #     db:
  #       condition: service_healthy

  # Frontend (uncomment after React setup)
  # web:
  #   build:
  #     context: ./bigjobhunterpro-web
  #     dockerfile: Dockerfile
  #   container_name: bigjobhunter-web
  #   ports:
  #     - "3000:80"
  #   environment:
  #     VITE_API_BASE_URL: http://localhost:5000/api
  #   depends_on:
  #     - api

volumes:
  sqlserver-data:
```

### Step 6: Start SQL Server

```bash
# Start just the database for now
docker-compose up -d db

# Verify it's running
docker-compose ps
```

### Step 7: Verify Setup

```bash
# Build the .NET solution
dotnet build

# Run tests (they should all pass initially - placeholder tests)
dotnet test

# Start frontend dev server
cd bigjobhunterpro-web
npm run dev
```

## Implementation Phases

Follow these phases incrementally to build the application while learning:

### Phase 1: Foundation (Weeks 1-3)
**Goal**: Working authentication + basic job application tracking

**What You'll Build**:
- User registration and login with JWT
- Create/Read/Update/Delete job applications
- Basic scoring (+1 point per application)
- Simple dashboard showing applications list
- Quick Capture modal (15-second promise)

**What You'll Learn**:
- Clean Architecture structure
- CQRS with MediatR
- EF Core migrations
- React hooks and Redux Toolkit
- Docker basics
- Git workflow

### Phase 2: Gamification Core (Weeks 4-6)
**Goal**: Full scoring system + streaks + achievements

**What You'll Build**:
- Stage progression (Applied → Interview → Offer)
- Complete point system (all stages)
- Daily streak tracking
- Achievement system
- Activity feed
- Enhanced dashboard with stats

**What You'll Learn**:
- Domain events
- Complex business logic
- State management patterns
- Data visualization

### Phase 3: Social Features (Weeks 7-9)
**Goal**: Hunting Parties + Leaderboards

**What You'll Build**:
- Create and join hunting parties
- Group leaderboards
- Rivalry comparisons
- Invite system
- Privacy settings

**What You'll Learn**:
- Complex LINQ queries
- Performance optimization
- Social feature architecture
- Advanced React patterns

### Phase 4: Polish & Production (Weeks 10-12)
**Goal**: Production-ready deployment

**What You'll Build**:
- CI/CD pipeline
- Production deployment to Azure
- Monitoring and logging
- PWA capabilities
- Accessibility improvements

**What You'll Learn**:
- DevOps practices
- Cloud deployment
- Observability
- Production best practices

## Key Files to Reference

Your detailed implementation plan is located at:
`C:\Users\Inferno\.claude\plans\generic-roaming-crystal.md`

This plan includes:
- Complete project structure
- Database schema
- API endpoint definitions
- Code examples for SOLID principles
- LINQ usage examples
- Testing strategies
- Learning resources

## Development Workflow

### Daily Development Loop

1. **Pull latest changes**
   ```bash
   git pull origin main
   ```

2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Start services**
   ```bash
   docker-compose up -d db
   cd src/WebAPI && dotnet run
   # In another terminal:
   cd bigjobhunterpro-web && npm run dev
   ```

4. **Make changes and test**
   ```bash
   dotnet test
   cd bigjobhunterpro-web && npm test
   ```

5. **Commit your work**
   ```bash
   git add .
   git commit -m "feat: add feature description"
   git push origin feature/your-feature-name
   ```

6. **Create pull request** on GitHub

## Useful Commands

### Backend (.NET)

```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Run API
cd src/WebAPI && dotnet run

# Create migration
dotnet ef migrations add MigrationName -p src/Infrastructure -s src/WebAPI

# Update database
dotnet ef database update -p src/Infrastructure -s src/WebAPI

# Clean build artifacts
dotnet clean
```

### Frontend (React)

```bash
# Install dependencies
npm install

# Start dev server
npm run dev

# Run tests
npm test

# Run linter
npm run lint

# Build for production
npm run build

# Type check
npm run type-check
```

### Docker

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Rebuild and restart
docker-compose up -d --build

# Remove volumes (clean database)
docker-compose down -v
```

## Next Steps

1. **Review the detailed plan**: Open `C:\Users\Inferno\.claude\plans\generic-roaming-crystal.md`

2. **Start with Phase 1**: Begin implementing the foundation
   - Create domain entities (User, Application, ScoreEvent)
   - Set up EF Core DbContext
   - Implement first API endpoints
   - Create basic React components

3. **Set up version control**:
   ```bash
   git add .
   git commit -m "chore: initial project structure setup"
   git push origin main
   ```

4. **Ask for help**: When you're ready to implement specific features, I can help you:
   - Write domain entities with proper OOP
   - Create CQRS command/query handlers
   - Set up EF Core configurations
   - Build React components
   - Configure CI/CD pipelines
   - Write tests

## Learning Resources

- **Clean Architecture**: [Jason Taylor's CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)
- **EF Core**: [Microsoft Learn - EF Core](https://learn.microsoft.com/en-us/ef/core/)
- **React**: [Official React Docs](https://react.dev)
- **TypeScript**: [TypeScript Handbook](https://www.typescriptlang.org/docs/handbook/intro.html)
- **Redux Toolkit**: [RTK Docs](https://redux-toolkit.js.org/)
- **Docker**: [Docker Getting Started](https://docs.docker.com/get-started/)
- **Azure**: [Azure Free Account](https://azure.microsoft.com/free/)

## Budget Tracking

**Target**: <$10/month

| Service | Tier | Cost |
|---------|------|------|
| Azure App Service | F1 Free | $0 |
| Azure SQL Database | Free (32MB) | $0 |
| Azure Static Web Apps | Free | $0 |
| **Total (Initial)** | | **$0** |

When you need more database capacity:
| Azure SQL Database | Basic (2GB) | $5/month |
| **Total (Scaled)** | | **$5/month** |

## Interview Preparation

As you build this project, document your learning for interviews:

1. **Architecture Decisions**: Keep notes on why you chose Clean Architecture, CQRS, etc.
2. **SOLID Examples**: Be ready to explain how your code demonstrates each principle
3. **LINQ Queries**: Understand and explain your complex queries (leaderboards, aggregations)
4. **Testing Strategy**: Know your coverage percentages and test types
5. **DevOps Pipeline**: Be able to walk through your CI/CD workflow
6. **Challenges Faced**: Document problems you solved and how

## Support

If you need help at any point:
- Review the detailed plan in `.claude/plans/`
- Check the concept document in `concept.md`
- Ask me for guidance on specific implementations
- Reference the learning resources above

Good luck building Big Job Hunter Pro! 🎯
