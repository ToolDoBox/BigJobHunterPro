# WebAPI Integration Tests

## Overview
Integration tests for BigJobHunterPro authentication endpoints using xUnit and ASP.NET Core WebApplicationFactory.

## Test Coverage
- **Register Endpoint (4 tests)**
  - Valid registration returns 201 + JWT token
  - Duplicate email returns 400
  - Invalid email format returns 400
  - Weak password returns 400

- **Login Endpoint (3 tests)**
  - Valid credentials return 200 + JWT token
  - Invalid password returns 401
  - Non-existent email returns 401

- **Protected Endpoints (3 tests)**
  - No token returns 401
  - Invalid token returns 401
  - Valid token returns 200 + user data

## Running Tests

```bash
# From repository root
dotnet test tests/WebAPI.IntegrationTests/WebAPI.IntegrationTests.csproj

# With detailed output
dotnet test tests/WebAPI.IntegrationTests/WebAPI.IntegrationTests.csproj --verbosity detailed

# Run all solution tests
dotnet test
```

## Known Issues

### Dependency Resolution (deps.json)
The test project currently encounters a runtime dependency resolution issue where VSTest looks for `lib/net6.0/Azure.Core.dll` instead of the correct net8.0 path. This is a known issue with WebApplicationFactory and transitive dependencies from Entity Framework Core.

**Workaround:**
Run tests using Visual Studio Test Explorer or Rider instead of `dotnet test` CLI, as IDEs handle dependency resolution differently.

**Permanent Fix (Future):**
- Upgrade to .NET 9 when available (improved dependency resolution)
- Use SQLite in-memory instead of SQL Server packages (fewer transitive dependencies)
- Wait for Microsoft.AspNetCore.Mvc.Testing package update

## Test Infrastructure

- **CustomWebApplicationFactory**: Creates in-memory test server with test database
- **TestDataHelper**: Generates unique test data to prevent conflicts
- **In-Memory Database**: Fast, isolated tests using EF Core InMemory provider

## Test Principles

1. **Test Isolation**: Each test generates unique data (emails, users)
2. **AAA Pattern**: Arrange, Act, Assert structure for clarity
3. **No Mocking**: Tests use real services with in-memory database
4. **Fast Execution**: Full test suite runs in <5 seconds
5. **Parallel Safe**: Tests can run in parallel without conflicts
