## Context

The solution has one integration test (`WebTests.cs`) that performs a single HTTP smoke-check against the Blazor frontend. There is no unit test coverage for `WeatherApiClient`, `WeatherForecast` model logic, or any service layer. Code coverage tooling is not configured.

The test project (`KingsAspireSpec.Tests`) already has the Aspire testing infrastructure (`DistributedApplicationTestingBuilder`, xUnit v3) wired up. The goal is to expand coverage without breaking existing patterns.

## Goals / Non-Goals

**Goals:**
- Achieve ≥ 80% overall line/branch coverage across `CustomerService` and `Web`
- Achieve ≥ 90% coverage on critical paths (API endpoints, typed HTTP client, model computed properties)
- Add unit tests that run fast and in isolation (no Aspire/Docker required)
- Add integration tests that exercise live HTTP interactions via the Aspire test host
- Configure Coverlet to measure and enforce coverage thresholds on every `dotnet test` run

**Non-Goals:**
- UI/browser-level tests with Playwright (deferred; no UI changes in scope)
- Changing any production code behavior
- Adding a separate test project — all tests stay in `KingsAspireSpec.Tests`
- Coverage for `AppHost` or `ServiceDefaults` (orchestration/infrastructure, not business logic)

## Decisions

### Decision 1 — Test organization within a single project

**Chosen:** Organize `KingsAspireSpec.Tests` into `Unit/` and `Integration/` subfolders. No new `.csproj` is added.

**Alternatives considered:**
- *Separate `KingsAspireSpec.Tests.Unit` project* — cleaner isolation but adds project scaffolding complexity and means two `dotnet test` invocations to get full coverage.

**Rationale:** A single `dotnet test KingsAspireSpec.Tests` command produces a unified coverage report. xUnit v3 `[Trait("Category", "Unit")]` and `[Trait("Category", "Integration")]` decorators allow filtering when needed.

---

### Decision 2 — HttpClient mocking strategy for unit tests

**Chosen:** A hand-rolled `FakeHttpMessageHandler : HttpMessageHandler` that returns pre-configured `HttpResponseMessage` objects.

**Alternatives considered:**
- *`RichardSzalay.MockHttp`* — full-featured but adds a dependency.
- *Full Moq mock of `HttpMessageHandler`* — possible but verbose for a protected `SendAsync` override.

**Rationale:** `Moq` is already listed as a project dependency. A small `FakeHttpMessageHandler` test-helper class covers all `WeatherApiClient` scenarios without new NuGet packages, keeping the dependency surface minimal per the coding guidelines.

---

### Decision 3 — Coverage tooling

**Chosen:** `coverlet.collector` (already bundled with the .NET SDK test templates) with thresholds enforced via `dotnet test` arguments:

```
dotnet test --collect:"XPlat Code Coverage" \
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Threshold=80 \
     DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.ThresholdType=line
```

A `coverlet.runsettings` file is added to `KingsAspireSpec.Tests/` to centralize threshold configuration.

**Alternatives considered:**
- *`coverlet.msbuild`* — integrates into build but gives less flexible reporting.

**Rationale:** `coverlet.collector` is the recommended approach for `dotnet test` integration; no additional packages required since the SDK already includes it for test projects.

---

### Decision 4 — Integration test scope

**Chosen:** Extend the existing `DistributedApplicationTestingBuilder` pattern to cover:
1. `GET /weatherforecast` on `CustomerService` — verifies response shape, count, and temperature logic
2. `GET /` on `CustomerService` — verifies plain-text response
3. `GET /` on `webfrontend` — existing smoke test (already passes)
4. `GET /weather` on `webfrontend` — verifies the Blazor weather page loads (HTTP 200)

**Rationale:** These cover the full data path (Browser → Web → CustomerService) exercised via real HTTP over the Aspire test host, satisfying integration test requirements at the API boundary level without Playwright.

## Risks / Trade-offs

| Risk | Mitigation |
|---|---|
| Integration tests require Docker for Redis | Document prerequisite; tests are already Docker-dependent. Mark integration tests with `[Trait("Category","Integration")]` so they can be skipped in environments without Docker |
| `WeatherForecast` data is random — assertions on exact values impossible | Assert structural properties (count, field types, valid date ranges) rather than exact values |
| `Aspire` test startup is slow (~10–30 s) | Unit tests are fast and isolated; integration tests are tagged so CI can run them separately |
| Coverage thresholds may block CI on first run before tests are complete | Apply thresholds incrementally: 60% in first PR, raise to 80% once all tests land |
