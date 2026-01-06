# Big Job Hunter Pro

Gamify your job search at **BigJobHunter.Pro**. A retro-arcade job tracker styled after 90s hunting classics. Log applications in 15 seconds, earn points for interviews, maintain daily streaks, and climb social leaderboards. Turn the "Corporate Wilderness" into your high-score hunting ground.

## Tech Stack

### Backend
- **Framework:** ASP.NET Core 8 Web API
- **Architecture:** Clean Architecture (Domain → Application → Infrastructure → API)
- **ORM:** Entity Framework Core
- **Database:** MS SQL Server (Azure SQL)
- **Auth:** ASP.NET Core Identity + JWT tokens

### Frontend
- **Framework:** React 18 + TypeScript + Vite
- **Styling:** Tailwind CSS with retro-arcade theme
- **State:** React Context API
- **Routing:** React Router v6

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (includes npm)
- [SQL Server](https://www.microsoft.com/sql-server) or Azure SQL Database

### 1. Database Setup

**Option A: Azure SQL (Recommended for development)**
1. Create a free Azure SQL Database at [portal.azure.com](https://portal.azure.com)
2. Configure firewall rules to allow your local IP
3. Note your connection string

**Option B: Local SQL Server**
1. Install SQL Server Express or Developer Edition
2. Use connection string: `Server=localhost;Database=BigJobHunterPro;Trusted_Connection=True;TrustServerCertificate=True;`

### 2. Backend Setup

```bash
# Navigate to backend directory
cd src/WebAPI

# Configure user secrets (REQUIRED)
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING_HERE"
dotnet user-secrets set "JwtSettings:Secret" "your-256-bit-secret-key-make-it-long-enough-for-HS256"
dotnet user-secrets set "JwtSettings:Issuer" "BigJobHunterPro"
dotnet user-secrets set "JwtSettings:Audience" "BigJobHunterPro"

# Run database migrations (creates tables and seeds dev data)
dotnet ef database update

# Run the API (HTTP for local development)
dotnet run --launch-profile http
```

**Backend will start at:** `http://localhost:5074`
**Swagger UI:** `http://localhost:5074/swagger`

**Seeded test users:**
- `hunter@test.com` / `Hunter123!` (150 points)
- `newbie@test.com` / `Newbie123!` (25 points)
- `pro@test.com` / `ProHunter123!` (500 points)

### 3. Frontend Setup

```bash
# Navigate to frontend directory
cd bigjobhunterpro-web

# Install dependencies
npm install

# Create .env file (if it doesn't exist)
echo "VITE_API_URL=http://localhost:5074" > .env
echo "VITE_APP_NAME=Big Job Hunter Pro" >> .env

# Start development server
npm run dev
```

**Frontend will start at:** `http://localhost:5173` (or `http://localhost:5174` if 5173 is in use)

### 4. Access the Application

1. Open browser to `http://localhost:5173`
2. Login with a seeded user or create a new account
3. Start tracking your job hunt! 🎯

## Project Structure

```
BigJobHunterPro/
├── src/                        # Backend (.NET)
│   ├── Domain/                 # Domain entities (User, Application, etc.)
│   ├── Application/            # DTOs, interfaces, business logic
│   ├── Infrastructure/         # EF Core, services, data access
│   └── WebAPI/                 # Controllers, API endpoints
│
├── bigjobhunterpro-web/        # Frontend (React + TypeScript)
│   ├── src/
│   │   ├── components/         # Reusable UI components
│   │   ├── context/            # React context (auth, etc.)
│   │   ├── hooks/              # Custom React hooks
│   │   ├── pages/              # Route pages (Login, Dashboard, etc.)
│   │   ├── router/             # React Router configuration
│   │   └── services/           # API client, auth service
│   └── public/                 # Static assets
│
├── tests/                      # Backend integration tests
│   └── WebAPI.IntegrationTests/
│
├── Meta/                       # Project documentation
│   ├── Docs/                   # Strategy, architecture, scoping
│   ├── Sprints/                # Sprint plans, backlogs, validation
│   └── Ref/                    # Reference artifacts
│
├── index.html                  # Landing page
├── styles.css                  # Landing page styles
└── README.md                   # This file
```

## Available Commands

### Backend
```bash
cd src/WebAPI

# Development
dotnet run --launch-profile http    # Run API (HTTP)
dotnet run --launch-profile https   # Run API (HTTPS)
dotnet build                        # Build solution
dotnet test                         # Run tests

# Database migrations
dotnet ef migrations add <Name> -p ../Infrastructure -s .
dotnet ef database update
```

### Frontend
```bash
cd bigjobhunterpro-web

npm run dev         # Start dev server
npm run build       # Production build
npm run preview     # Preview production build
npm run test        # Run tests (Vitest)
npm run lint        # Run ESLint
```

### VS Code Tasks
From the Command Palette, run `Tasks: Run Task` and select:
- `Full Stack: Run API + Web` (runs `dotnet run --launch-profile http` and `npm run dev`)
- `Backend: Run API (http)` (runs `dotnet run --launch-profile http`)
- `Frontend: Run Dev Server` (runs `npm run dev`)

## Development Notes

### CORS Configuration
The backend is configured to allow requests from:
- `http://localhost:5173` (default Vite port)
- `http://localhost:5174` (alternate Vite port)

If your frontend runs on a different port, update `src/WebAPI/Program.cs`.

### Environment Variables

**Backend** (via user secrets):
- `ConnectionStrings:DefaultConnection` - Database connection string
- `JwtSettings:Secret` - JWT signing key (min 256 bits)
- `JwtSettings:Issuer` - JWT issuer claim
- `JwtSettings:Audience` - JWT audience claim

**Frontend** (`.env` file):
- `VITE_API_URL` - Backend API base URL (default: `http://localhost:5074`)
- `VITE_APP_NAME` - Application name

### Troubleshooting

**"Connection string is empty" error:**
- Ensure user secrets are configured: `dotnet user-secrets list`
- Backend must run with `--launch-profile http` (sets environment to Development)

**CORS errors in browser:**
- Verify backend is running and CORS is configured in `Program.cs`
- Check frontend `.env` has correct `VITE_API_URL`

**Port conflicts:**
- Backend: Edit `src/WebAPI/Properties/launchSettings.json`
- Frontend: Vite will auto-increment port (5173 → 5174 → 5175...)

## Current Sprint Status

**Sprint 1:** Authenticated App Shell + Quick Capture (In Progress)

See `Meta/Sprints/Sprint-1/` for detailed sprint planning and validation results.

## Documentation

- **Architecture:** `Meta/Docs/Project-Structure.md`
- **Strategy:** `Meta/Docs/Project-Strategy.md`
- **Scoping:** `Meta/Docs/Project-Scoping.md`
- **Sprint System:** `Meta/Sprints/README.md`
- **Validation Guide:** `Meta/Sprints/Sprint-1/Validation.md`

## Contributing

This is currently a private project. Sprint planning follows trunk-based development with feature branches.
