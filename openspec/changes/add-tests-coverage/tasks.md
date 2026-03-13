## 1. Test Infrastructure Setup

- [x] 1.1 Add `Moq` NuGet package to `KingsAspireSpec.Tests`
- [x] 1.2 Create `KingsAspireSpec.Tests/Unit/` and `KingsAspireSpec.Tests/Integration/` folders
- [x] 1.3 Move existing `WebTests.cs` into `KingsAspireSpec.Tests/Integration/`
- [x] 1.4 Create `KingsAspireSpec.Tests/Unit/Helpers/FakeHttpMessageHandler.cs` with call-count tracking
- [x] 1.5 Add `coverlet.runsettings` to `KingsAspireSpec.Tests/` with 80% line-coverage threshold

## 2. Unit Tests — WeatherForecast Model

- [x] 2.1 Create `KingsAspireSpec.Tests/Unit/CustomerService/WeatherForecastTests.cs`
- [x] 2.2 Add test: `TemperatureF` returns 32 when `TemperatureC` is 0
- [x] 2.3 Add test: `TemperatureF` is within 1 degree of 212 when `TemperatureC` is 100
- [x] 2.4 Add test: `TemperatureF` equals -40 when `TemperatureC` is -40

## 3. Unit Tests — WeatherApiClient

- [x] 3.1 Create `KingsAspireSpec.Tests/Unit/Web/WeatherApiClientTests.cs`
- [x] 3.2 Add test: `GetWeatherAsync` returns all items when response has fewer than `maxItems`
- [x] 3.3 Add test: `GetWeatherAsync` respects `maxItems` limit and returns exactly that count
- [x] 3.4 Add test: `GetWeatherAsync` returns empty array when API returns `[]`

## 4. Integration Tests — CustomerService Endpoints

- [x] 4.1 Create `KingsAspireSpec.Tests/Integration/CustomerServiceTests.cs`
- [x] 4.2 Add test: `GET /` on `customerservice` returns HTTP 200 and body contains `"API service is running"`
- [x] 4.3 Add test: `GET /weatherforecast` on `customerservice` returns HTTP 200
- [x] 4.4 Add test: `/weatherforecast` response deserializes to a non-empty `WeatherForecast[]`
- [x] 4.5 Add test: Each forecast element has valid `Date`, non-null `Summary`, and `TemperatureC` in range -20..55
- [x] 4.6 Add test: `TemperatureF` is consistent with `TemperatureC` for each returned forecast

## 5. Integration Tests — Web Frontend

- [x] 5.1 Add test to `WebTests.cs` (or new class): `GET /weather` on `webfrontend` returns HTTP 200
- [x] 5.2 Decorate all new integration tests with `[Trait("Category", "Integration")]`
- [x] 5.3 Verify all integration tests call `WaitForResourceHealthyAsync` before HTTP assertions

## 6. Coverage Configuration & Validation

- [x] 6.1 Verify `dotnet test KingsAspireSpec.Tests` runs without errors
- [ ] 6.2 Run `dotnet test --collect:"XPlat Code Coverage"` and confirm `coverage.cobertura.xml` is generated
- [ ] 6.3 Confirm overall line coverage is ≥ 80% (check `coverlet.runsettings` thresholds kick in)
- [ ] 6.4 Confirm `WeatherApiClient` and `WeatherForecast` paths show ≥ 90% coverage in the report

