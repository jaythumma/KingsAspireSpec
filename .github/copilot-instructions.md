# Copilot Instructions — KingsAspireSpec

## Architecture Overview

This is a **.NET 10 Aspire** solution with four runnable/referenced projects:

| Project | Role |
|---|---|
| `KingsAspireSpec.AppHost` | Aspire orchestrator — defines the distributed app topology |
| `KingsAspireSpec.ApiService` | ASP.NET Core minimal-API backend |
| `KingsAspireSpec.Web` | Blazor Server frontend (Interactive Server render mode) |
| `KingsAspireSpec.ServiceDefaults` | Shared cross-cutting library (OpenTelemetry, health checks, resilience, service discovery) |
| `KingsAspireSpec.Tests` | Integration tests that boot the full Aspire AppHost |

**Data flow:** Browser → `Web` (Blazor Server) → `WeatherApiClient` (typed `HttpClient`) → `ApiService` (minimal API). Redis output-caching sits between `Web` and the browser.

## Critical Patterns

### ServiceDefaults — always call `AddServiceDefaults()`
Every service project (`ApiService`, `Web`) must call `builder.AddServiceDefaults()` in `Program.cs`. This single call wires up OpenTelemetry (OTLP exporter), health checks at `/health` and `/alive`, standard HTTP resilience, and Aspire service discovery. See `KingsAspireSpec.ServiceDefaults/Extensions.cs`.

### Service Discovery — use `https+http://` scheme
HTTP clients that call other Aspire-registered services resolve by **logical name**, not a hardcoded URL:
```csharp
client.BaseAddress = new("https+http://apiservice");
```
The resource name `"apiservice"` must match the name passed to `builder.AddProject<...>("apiservice")` in `AppHost.cs`.

### AppHost topology — `AppHost/AppHost.cs`
New resources (services, databases, caches) are registered here. Redis is provisioned and injected into `webfrontend` via `.WithReference(cache).WaitFor(cache)`. Adding a new backing service follows the same pattern.

### Blazor pages — `Web/Components/Pages/`
- Use `@attribute [StreamRendering(true)]` for pages that load async data.
- Use `@attribute [OutputCache(Duration = N)]` to opt into Redis-backed output caching.
- Inject API clients with `@inject WeatherApiClient WeatherApi`.
- Component-scoped CSS lives in a `.razor.css` sibling file (e.g., `NavMenu.razor.css`).

### Typed API clients — primary constructor injection
```csharp
public class WeatherApiClient(HttpClient httpClient) { ... }
```
Clients are registered in `Web/Program.cs` via `builder.Services.AddHttpClient<T>()`. Use `GetFromJsonAsAsyncEnumerable` for streaming JSON responses.

### Minimal API endpoints — `ApiService/Program.cs`
All routes are added with `app.MapGet(...)` / `app.MapPost(...)` inline. OpenAPI docs are enabled in development (`app.MapOpenApi()`). Always call `app.MapDefaultEndpoints()` last.

### API response types — top-level records
Domain types are declared as top-level `record` types at the bottom of `Program.cs` (see `WeatherForecast`). Shared types used by both `Web` and `ApiService` are duplicated as records in each project (no shared DTO library currently).

## Developer Workflows

**Run the full stack (preferred):** Set `KingsAspireSpec.AppHost` as startup project and press F5. The Aspire dashboard launches automatically.

**Run tests:**
```
dotnet test KingsAspireSpec.Tests
```
Tests use `DistributedApplicationTestingBuilder.CreateAsync<Projects.KingsAspireSpec_AppHost>()` to start the real AppHost — they require Docker (for Redis) or a local Redis instance.

**Build only:**
```
dotnet build KingsAspireSpec.sln
```

## Key Files

- `KingsAspireSpec.AppHost/AppHost.cs` — single source of truth for the service graph
- `KingsAspireSpec.ServiceDefaults/Extensions.cs` — shared observability & resilience setup
- `KingsAspireSpec.Web/WeatherApiClient.cs` — canonical example of a typed HTTP client
- `KingsAspireSpec.Web/Components/Pages/Weather.razor` — canonical Blazor page with streaming + caching
- `KingsAspireSpec.Tests/WebTests.cs` — integration test pattern using `Aspire.Hosting.Testing`
