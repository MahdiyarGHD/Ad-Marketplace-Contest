# Copilot Instructions: Ad Marketplace Contest

## Project Overview

This is a Telegram-based ad marketplace application built with:
- **Backend**: .NET 10.0 with FastEndpoints, Entity Framework Core, PostgreSQL
- **Frontend**: React 19 + TypeScript + Vite, using Telegram Mini App SDK
- **Bot**: Telegram.Bot integration for channel connections and user management

The project structure uses a clean architecture approach with domain-driven design principles.

## Build & Run Commands

### Backend (.NET)

```bash
# Working directory: src/Back-End/
dotnet restore
dotnet build
dotnet run --project AdMarketplace.WebApi/AdMarketplace.WebApi.csproj

# Docker Compose (full stack with PostgreSQL and Telegram Bot API)
docker compose up --build

# Database migrations (from Back-End directory)
dotnet ef migrations add <MigrationName> --project AdMarketplace.Database --startup-project AdMarketplace.WebApi
dotnet ef database update --project AdMarketplace.Database --startup-project AdMarketplace.WebApi
```

### Frontend (React + Vite)

```bash
# Working directory: src/Front-End/
npm install
npm run dev          # Development server
npm run build        # Production build
npm run lint         # ESLint
npx biome check .    # Biome linter/formatter
npx biome format --write .  # Auto-format with Biome
```

**Note**: The frontend uses **Biome** for linting/formatting (configured in `biome.json`), not just ESLint.

## Architecture

### Backend Project Structure

```
AdMarketplace.Domain/      # Core domain entities, contracts, types (ErrorOr pattern)
AdMarketplace.Database/    # EF Core DbContext, migrations, entity configurations
AdMarketplace.Infra/       # Services, interfaces, helpers (Serilog, Mapster)
AdMarketplace.Bot/         # Telegram bot handlers and endpoints
AdMarketplace.WebApi/      # FastEndpoints HTTP API entry point
```

**Dependency Flow**: `WebApi` → `Bot` → `Infra` → `Database` → `Domain`

### FastEndpoints Pattern

All HTTP endpoints follow this structure:
- Location: `AdMarketplace.WebApi/Endpoints/{Feature}/{Action}/Endpoint.cs`
- Each endpoint is a class inheriting from `Endpoint<TRequest, TResponse>`
- Use `Configure()` to define routes and policies
- Use `ExecuteAsync()` for handler logic

Example:
```csharp
public class Endpoint(IChannelService channelService)
    : Endpoint<Request, ErrorOr<Response>>
{
    public override void Configure()
    {
        Get("/api/channels/{Id}");
        AllowAnonymous();
    }

    public override async Task<ErrorOr<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        // Implementation
    }
}
```

### Service Layer Pattern

- Services in `AdMarketplace.Infra/Services/` implement interfaces from `AdMarketplace.Infra/Interfaces/`
- All services return `ErrorOr<T>` from the `ErrorOr` package (not exceptions for business logic failures)
- Services are registered via `ConfigureServices()` extension in `ServiceCollectionExtensions.cs`
- Constructor injection with primary constructors (C# 12)

### Database Layer

- `AdMarketDbContext` uses EF Core 10 with PostgreSQL
- Entity configurations in `AdMarketplace.Database/Configurations/`
- All entities implement `IDateTimeSchema` (CreatedAt, UpdatedAt)
- Migrations stored in `AdMarketplace.Database/Migrations/`
- Database seeding via `MigrateAndSeedAsync()` on app start

### Telegram Bot Integration

- `UpdateHandler` is the main entry point for Telegram updates
- Handler classes in `AdMarketplace.Bot/Handlers/` process specific update types
- Bot endpoints in `AdMarketplace.Bot/Endpoints/` handle bot-specific HTTP routes
- Webhook configuration in `compose.yaml` with local Telegram Bot API server

## Key Conventions

### .NET Backend

- **Primary constructors** used throughout (C# 12 feature)
- **ErrorOr pattern** for result types instead of throwing exceptions for business logic
- **Nullable reference types** enabled (`<Nullable>enable</Nullable>`)
- **Implicit usings** enabled globally
- **Namespace follows folder structure** (no nested namespaces)
- **Dependency injection** via Scrutor for automatic registration
- **Configuration** via extension methods in `Extensions/` folders

### Frontend (React)

- **Tab indentation** (configured in biome.json)
- **Double quotes** for strings (JS/CSS)
- **Trailing commas** everywhere
- **Semicolons** always required
- **Zustand** for state management
- **React Router DOM** v7 for routing
- **Telegram Mini App SDK** (`@tma.js/sdk-react`) for Telegram integration
- **Lucide React** for icons
- **SASS** for styling

### Docker & Deployment

- Backend Dockerfile in `AdMarketplace.WebApi/Dockerfile`
- Frontend deploys to GitHub Pages via `.github/workflows/fe-deploy.yml`
- Environment variables for Telegram bot token and connection strings in `compose.yaml`

## Configuration Files

- `appsettings.json` / `appsettings.Development.json` - Backend configuration
- `compose.yaml` - Docker services (app, PostgreSQL, Telegram Bot API)
- `nuget.config` - Custom NuGet sources
- `biome.json` - Frontend code formatting and linting rules
- `vite.config.ts` - Frontend build config with base path `/Ad-Marketplace-Contest`

## Development Workflow

1. **Backend changes**: Make changes → Run migrations if schema changed → Test with Swagger (`/swagger`)
2. **Frontend changes**: Use `npm run dev` with hot reload
3. **Database changes**: Create migration → Update database → Verify in PostgreSQL
4. **Bot changes**: Test with local Telegram Bot API server (port 8081)

## Common Tasks

### Adding a new endpoint
1. Create folder in `AdMarketplace.WebApi/Endpoints/{Feature}/{Action}/`
2. Add `Endpoint.cs`, `Request.cs`, `Response.cs`
3. Implement service logic in `AdMarketplace.Infra/Services/`
4. FastEndpoints auto-discovers endpoints (no manual registration needed)

### Adding a new entity
1. Create model in `AdMarketplace.Database/Models/`
2. Add DbSet in `AdMarketDbContext.cs`
3. Create configuration in `AdMarketplace.Database/Configurations/`
4. Generate and apply migration

### Adding a new bot handler
1. Create handler class in `AdMarketplace.Bot/Handlers/`
2. Inject into `UpdateHandler.cs`
3. Call from `HandleUpdate()` method


project description:

Goal
Build an MVP Telegram Mini App for an ads marketplace that connects channel owners and advertisers, using an escrow-style deal flow.

MVP Scope
1) Marketplace model (both sides must be supported)

Channel owner listings:
Channel owner lists their channel, sets pricing, and adds a bot as an admin (for stats verification and future auto-posting).

Extra: think about PR manager flow and ability to add 1+ users to manage channel, for example fetch admins of the channel with selected rights. You must re-check if user still an admin on financial and other important operations.

Advertiser requests:
Advertiser creates a request/campaign brief; channel owners (influencers) can apply.

Both entry points must converge into a single unified workflow for negotiation, approvals, escrow, and auto-posting. For messaging, use a text bot; don't create a chat in a mini-app.

Implement practical filters for both offer types(pricing, subscribers, views, etc.)

2) Verified channel stats (from Telegram)
Automatically fetch and display verified channel stats available via Telegram, including (at minimum):
— subscribers
— average views / reach
— language charts
— Telegram Premium stats
— any other metrics exposed by Telegram channel analytics

3) Ad formats and pricing
Support setting prices for different ad formats within a single channel, eg.: post, forward/repost, story, and other formats you deem suitable, but only post is OK for MVP. This should be a free format rather than a strict ad type.

4) Escrow deal flow based on TON
Implement an escrow-style flow:
payment by advertiser → funds held by us → auto-posting confirms delivery → release or refund
This should be secure; it is recommended to use a new address/wallet for each deal or for each user, except for a hot wallet.
Include basic lifecycle controls such as:
— auto-cancel / timeout if the deal stalls (no activity for X time)
— clear deal statuses and transitions

5) Creative approval workflow
A clear approval loop must exist:
advertiser submits preferences / brief → channel owner accepts or rejects → if accepted, channel owner drafts the post and submits it for review → advertiser approves or requests edits → once approved, the post is auto-published at the agreed time

6) Auto-posting
Auto-post the approved creative to the channel and verify it's not deleted, edited, etc. — verify that creative is done and stay in channel for enough time before releasing funds to channel owner.

Stack
We do not restrict the tech stack. We want to see your product thinking, engineering decisions, and system design. Your code should be clean and ready to opensource.
Backend is the main focus. If you’re short on frontend capacity, a lightweight UI is acceptable, prioritize working flows over visual polish, it can be vibecoded, for reference use other tools on tools.tg

Prize

The grant is a paid build budget. The winner will receive milestone-based compensation to continue development with us while we remain the product owner and handle everything except engineering; rev-share is negotiable.
