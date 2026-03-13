## ADDED Requirements

### Requirement: CustomerService weatherforecast endpoint returns valid data
The system SHALL have integration tests (using `DistributedApplicationTestingBuilder`) that verify `GET /weatherforecast` on `CustomerService` returns an HTTP 200 with a JSON array of weather forecast objects.

#### Scenario: Endpoint returns HTTP 200
- **WHEN** a GET request is made to `/weatherforecast` on the `customerservice` resource
- **THEN** the response status SHALL be `200 OK`

#### Scenario: Response body is a non-empty JSON array
- **WHEN** a GET request is made to `/weatherforecast`
- **THEN** the response body SHALL deserialize to a `WeatherForecast[]` with at least 1 element

#### Scenario: Each forecast has valid structure
- **WHEN** the `/weatherforecast` response is deserialized
- **THEN** each element SHALL have a non-default `Date`, a `Summary` that is not null or empty, and a `TemperatureC` within the range -20 to 55

#### Scenario: TemperatureF is consistent with TemperatureC
- **WHEN** the `/weatherforecast` response is deserialized
- **THEN** each element's `TemperatureF` SHALL equal `32 + (int)(TemperatureC / 0.5556)`

### Requirement: CustomerService root endpoint returns plain text
The system SHALL have an integration test verifying `GET /` on `CustomerService` returns HTTP 200 with a plain-text body.

#### Scenario: Root endpoint returns HTTP 200
- **WHEN** a GET request is made to `/` on the `customerservice` resource
- **THEN** the response status SHALL be `200 OK`

#### Scenario: Root response contains expected message
- **WHEN** a GET request is made to `/` on the `customerservice` resource
- **THEN** the response body SHALL contain the text `"API service is running"`

### Requirement: Web frontend weather page loads successfully
The system SHALL have an integration test verifying the Blazor weather page (`/weather`) on the `webfrontend` resource returns HTTP 200, confirming end-to-end data flow from browser to `CustomerService`.

#### Scenario: Weather page returns HTTP 200
- **WHEN** a GET request is made to `/weather` on the `webfrontend` resource
- **THEN** the response status SHALL be `200 OK`

### Requirement: Integration tests are tagged and wait for resource health
The system SHALL ensure all Aspire integration tests use `[Trait("Category", "Integration")]` and call `WaitForResourceHealthyAsync` before making HTTP assertions.

#### Scenario: Tests use Category trait
- **WHEN** an integration test class is defined
- **THEN** each test method SHALL be decorated with `[Trait("Category", "Integration")]`

#### Scenario: Tests wait for resource health before HTTP calls
- **WHEN** an Aspire app is started in a test
- **THEN** the test SHALL call `WaitForResourceHealthyAsync` for the target resource before any HTTP assertion

### Requirement: Code coverage thresholds are enforced
The system SHALL configure Coverlet in `KingsAspireSpec.Tests` to enforce a minimum of 80% line coverage overall, failing the test run if the threshold is not met.

#### Scenario: Coverage report is generated on every test run
- **WHEN** `dotnet test` is executed on `KingsAspireSpec.Tests`
- **THEN** a code coverage report SHALL be generated in the `TestResults/` directory

#### Scenario: Test run fails below 80% coverage
- **WHEN** code coverage drops below 80% overall
- **THEN** the `dotnet test` run SHALL exit with a non-zero status code
