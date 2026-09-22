# Omnesoft Challenge

A small product catalogue: a .NET 10 FastEndpoints API over PostgreSQL, a React + MUI frontend,
integration tests on Testcontainers, and an Aspire AppHost that wires them all together and runs them locally.

## Quick start

Default Login Credentials (also shown on the login page)

| User | Password | Access |
|---|---|---|
| `admin` | `admin123` | Full access |
| `guest` | `guest123` | Read-only |

You must have NodeJS 22, .NET 10 SDK + Docker running on your machine

Solution located at backend/OmnesoftChallenge.slnx

In Visual Studio or Rider, set OmnesoftChallenge.AppHost (Aspire) as the startup project and run it. 
Aspire handles everything: it starts PostgreSQL, runs the API (which applies the migrations and seed data on startup), installs the npm dependencies and runs the frontend at http://localhost:5173.

## Backend, Architecture and Design

Three layers, with the projects at the solution root:

| Solution folder | Project | Responsibility |
|---|---|---|
| `1 - UI` | `OmnesoftChallenge.AppHost` | Aspire orchestration |
| `1 - UI` | `OmnesoftChallenge.Api` | FastEndpoints endpoints; all DI and middleware in `Program.cs`; Aspire service defaults in `Extensions/` |
| `2 - BLL` | `OmnesoftChallenge.BLL` | Services, security (JWT, hashing, authentication), request validation, view models, Mapperly mapper |
| `3 - DAL` | `OmnesoftChallenge.DAL` | `EntityFrameworkContext` and `DapperContext`, entities, `EntitiyMapping`, repositories, Unit of Work, migrations |
| `4 - Tests` | `OmnesoftChallenge.IntegrationTests` | API integration tests against PostgreSQL in Testcontainers |

References: `AppHost → Api` (as an Aspire resource) · `Api → BLL` · `BLL → DAL` · `IntegrationTests → Api`.

## Frontend

React 19 + MUI single-page app built with Vite, in `frontend/`.

- **Pages.** `/login` and `/products`. The products table is visible to both roles; only admins
  see the add, edit and delete actions.
- **Auth.** Tokens are kept in `localStorage`. An Axios interceptor adds the access token to every
  request and, on a 401, refreshes it once with the refresh token before logging the user out.
- **API calls.** Requests go to `/api`, which the Vite dev server proxies to the API address
  Aspire provides, so no extra configuration is needed.

## Integration tests

Docker must be running. From the repository root:

```bash
dotnet test --solution backend/OmnesoftChallenge.slnx
```

- **Real database.** Each run starts a throwaway `postgres:18.3` container through
  Testcontainers (`Infrastructure/OmnesoftChallengeApiFactory.cs`), the same image Aspire uses,
  and removes it when the run ends. The database starts empty, so the API's startup check applies
  the real migrations, seed data included, on every run.
- **Real API.** `WebApplicationFactory<Program>` hosts the actual API in memory, so every
  request goes through the real JWT authentication, authorization policies, FastEndpoints
  binding, BLL validation, Dapper and EF Core.

| Area | Covered |
|---|---|
| Login | valid admin and guest logins, wrong password, unknown user, empty credentials (validation) |
| Refresh tokens | rotation, rejection of a reused token, refreshed access token accepted, unknown and empty tokens |
| Products, reads | missing and tampered tokens (401), list, by id, 404, invalid id (400) |
| Products, writes | create, update, delete round-trips, null description, validation failures, name length limit, 404s, guest gets 403 on every write |