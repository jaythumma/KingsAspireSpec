# Copilot Instructions — KingsAspireSpec


# Requirements

- Write clear, self-documenting code
- Keep abstractions simple and focused
- Minimize dependencies and coupling
- Use modern C# features appropriately
- Any code you commit MUST compile, and new and existing tests related to the change MUST pass.
- MUST make your best effort to ensure any code changes satisfy those criteria before committing. If for any reason you were unable to build or test code changes, you MUST report that.
- must NOT claim success unless all builds and tests pass as described above.
- Before completing, use the code-review skill to review your code changes. Any issues flagged as errors or warnings should be addressed before completing.
- Run tests locally before committing. Do not rely solely on CI to catch build or test failures.
- If you are unsure about any of the requirements or how to implement a change, ask for clarification before proceeding. Do not make assumptions that could lead to code that does not meet the standards outlined above.
- Ensure code coverage stays above 80% for the entire solution, and above 90% for critical paths, when running Unit and Integration tests. 
- Follow TTD practices: write failing tests first (unit and integration), then implement code to pass those tests. Red Green refactor cycles should be followed to ensure code quality and maintainability.

## Architecture Overview

This is a **.NET 10 Aspire** solution with four runnable/referenced projects:

| Project | Role |
|---|---|
| `KingsAspireSpec.AppHost` | Aspire orchestrator — defines the distributed app topology |
| `KingsAspireSpec.CustomerService` | ASP.NET Core minimal-API backend |
| `KingsAspireSpec.Web` | Blazor Server frontend (Interactive Server render mode) |
| `KingsAspireSpec.ServiceDefaults` | Shared cross-cutting library (OpenTelemetry, health checks, resilience, service discovery) |
| `KingsAspireSpec.Tests` | Integration tests that boot the full Aspire AppHost |

**Data flow:** Browser → `Web` (Blazor Server) → `WeatherApiClient` (typed `HttpClient`) → `CustomerService` (minimal API). Redis output-caching sits between `Web` and the browser.

## Critical Patterns

### ServiceDefaults — always call `AddServiceDefaults()`
Every service project (`CustomerService`, `Web`) must call `builder.AddServiceDefaults()` in `Program.cs`.

### Service Discovery — use `https+http://` scheme
HTTP clients that call other Aspire-registered services resolve by **logical name**, not a hardcoded URL:
```csharp
client.BaseAddress = new("https+http://customerservice");
```
The resource name `"customerservice"` must match the name passed to `builder.AddProject<...>("customerservice")` in `AppHost.cs`.

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
Domain types are declared as top-level `record` types at the bottom of `Program.cs` (see `WeatherForecast`). Shared types used by both `Web` and `CustomerService` are duplicated as records in each project (no shared DTO library currently).

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
- `KingsAspireSpec.CustomerService/Program.cs` — minimal API endpoints and top-level record types
- `KingsAspireSpec.Tests/WebTests.cs` — integration test pattern using `Aspire.Hosting.Testing`
