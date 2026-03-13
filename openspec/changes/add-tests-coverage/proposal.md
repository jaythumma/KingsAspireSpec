## Why

The solution currently has a single smoke-test that only verifies the web frontend returns HTTP 200. There is no unit test coverage for service logic or API clients, and no integration tests that exercise the `CustomerService` API endpoints directly. This leaves critical paths unvalidated and makes future refactoring risky.

## What Changes

- Add **unit tests** for `WeatherApiClient` (typed HTTP client) in `KingsAspireSpec.Web`
- Add **unit tests** for the `WeatherForecast` record and temperature conversion logic in `KingsAspireSpec.CustomerService`
- Add **integration tests** for `CustomerService` API endpoints (`/weatherforecast`, `/`) using `DistributedApplicationTestingBuilder`
- Add **integration tests** for `Web` frontend pages that consume the API (via Blazor component testing or HTTP-level checks)
- Configure code coverage tooling (Coverlet) to enforce ≥ 80% overall and ≥ 90% on critical paths
- Organize `KingsAspireSpec.Tests` project to separate unit and integration test classes clearly

## Capabilities

### New Capabilities

- `unit-tests`: Unit-level tests for `WeatherApiClient`, `WeatherForecast` model logic, and any service methods — fully isolated with mocked dependencies using Moq
- `integration-tests`: Full-stack Aspire integration tests exercising `CustomerService` HTTP endpoints and verifying `Web` frontend responses against a live service graph

### Modified Capabilities

<!-- No existing spec-level capabilities are changing requirements -->

## Impact

- `KingsAspireSpec.Tests` — primary target; test classes added, Coverlet configured
- `KingsAspireSpec.CustomerService` — no production code changes expected; may add `internal` visibility for testability
- `KingsAspireSpec.Web` — no production code changes expected; `WeatherApiClient` tested via mocked `HttpClient`
- CI pipeline — coverage thresholds enforced via `dotnet test --collect:"XPlat Code Coverage"`
