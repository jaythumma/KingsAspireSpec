## ADDED Requirements

### Requirement: WeatherApiClient returns forecasts from API
The system SHALL have unit tests that verify `WeatherApiClient.GetWeatherAsync` correctly deserializes a JSON array from the HTTP response and returns typed `WeatherForecast` objects, using a mocked `HttpMessageHandler`.

#### Scenario: Returns all forecasts within max limit
- **WHEN** the API responds with a JSON array of 5 weather forecasts
- **THEN** `GetWeatherAsync(maxItems: 10)` SHALL return an array of 5 `WeatherForecast` records

#### Scenario: Respects maxItems limit
- **WHEN** the API responds with a JSON array of 10 weather forecasts
- **THEN** `GetWeatherAsync(maxItems: 3)` SHALL return an array of exactly 3 `WeatherForecast` records

#### Scenario: Returns empty array on empty API response
- **WHEN** the API responds with an empty JSON array `[]`
- **THEN** `GetWeatherAsync()` SHALL return an empty array (not null)

### Requirement: WeatherForecast temperature conversion is correct
The system SHALL have unit tests verifying that the `TemperatureF` computed property on the `WeatherForecast` record returns the correct Fahrenheit value.

#### Scenario: Zero degrees Celsius converts to 32 Fahrenheit
- **WHEN** a `WeatherForecast` is created with `TemperatureC = 0`
- **THEN** `TemperatureF` SHALL equal `32`

#### Scenario: 100 degrees Celsius converts approximately to 212 Fahrenheit
- **WHEN** a `WeatherForecast` is created with `TemperatureC = 100`
- **THEN** `TemperatureF` SHALL be within 1 degree of `212`

#### Scenario: Negative temperature converts correctly
- **WHEN** a `WeatherForecast` is created with `TemperatureC = -40`
- **THEN** `TemperatureF` SHALL equal `-40` (the crossover point)

### Requirement: FakeHttpMessageHandler test helper exists
The system SHALL provide a reusable `FakeHttpMessageHandler` test helper class in the test project that allows configuring a canned `HttpResponseMessage` and tracking call count.

#### Scenario: Handler returns configured response
- **WHEN** a `FakeHttpMessageHandler` is constructed with a specific `HttpResponseMessage`
- **THEN** any `HttpClient` using it SHALL receive that response on `SendAsync`

#### Scenario: Handler tracks call count
- **WHEN** an `HttpClient` backed by `FakeHttpMessageHandler` makes multiple requests
- **THEN** `FakeHttpMessageHandler.CallCount` SHALL reflect the number of requests made
