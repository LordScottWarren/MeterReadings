# 📈 MeterReadings

A test project demonstrating a cleanly structured API and frontend experience for uploading and validating CSV files containing meter readings.

This solution showcases a full-stack architecture using:
- **ASP.NET Core 8 Web API** for backend processing
- **Blazor WebAssembly (WASM)** for the client interface
- **CsvHelper** for robust CSV parsing
- **MudBlazor** for rich UI components

---

## 🧩 Features

### ✅ API Functionality
- `POST /meter-reading-upload` endpoint for receiving a CSV file via `multipart/form-data`
- Parses the CSV using `CsvHelper`
- Validates each row based on business rules:
  - Must be exactly 5 numeric digits
  - Must not be a duplicate reading
  - Must be newer than existing readings for the same customer
  - Must have a valid customer account
- Saves valid meter readings to an in-memory database
- Returns a structured result summarizing accepted and rejected record counts

### ✅ Service Layer
- Encapsulates all domain logic and validation
- Separates parsing, validation, and persistence concerns
- Fully unit-testable

### ✅ Data Access Layer (DAL)
- Abstracted via repository interfaces
- Uses Entity Framework Core with an InMemory database
- Provides limited CRUD logic for customer accounts and meter readings
- Uses an embedded resource file and reflection to seed the database with client accounts

### ✅ Blazor Client (WASM)
- File upload UI using MudBlazor
- Sends CSV to API and shows snackbar notifications on success/failure

---

## 🚫 What This Project Does **Not** Include

This project intentionally skips certain concerns to keep the focus clear:

- ❌ **Authentication / Authorization**
- ❌ **Persistent Database** (uses InMemory EF Core)
- ❌ **CSV Preview or Parsing in the Client**
- ❌ **Granular error display to the frontend**
- ❌ **File size or type enforcement beyond frontend hinting**

---

## 🧪 How to Run

### Prerequisites
- .NET 8 SDK
- A modern browser

### Run the API
```bash
cd MeterReadings
dotnet run
```

The API will run at `https://localhost:7079`.

Swagger: `https://localhost:7079/swagger/index.html`.

### Run the Blazor Client
```bash
cd Client
dotnet run
```

The Blazor UI will run at `https://localhost:7046`.

---

## 🔬 Testing

Unit tests cover:
- CSV parsing
- Validation logic
- Insert rules (duplicate prevention, timestamp validation)

To run the tests:
```bash
dotnet test
```

---

## 🧠 Architecture Overview

- **Layered design**:
  - `Client`: Blazor WASM frontend
  - `API`: ASP.NET Core Web API
  - `ServiceLayer`: CSV parsing, validation, and coordination
  - `DAL`: Repositories and EF Core DbContext
  - `Shared`: DTOs used across layers
- **Built for testability** with full use of DI and mocking
- **Clean separation of concerns**

---

## 📂 Project Structure

```
/Client                       - Blazor WASM UI
/MeterReadingsAPI             - ASP.NET Core API
/MeterReadingsDAL            - Repositories and EF Core
/MeterReadingsDAL.Tests      - Unit tests for DAL logic
/MeterReadingsServiceLayer   - Service logic and validation
/MeterReadingsServiceLayer.Test   - Unit tests for service logic and validation
/MeterReadings.Shared        - Shared DTOs
```

---

## ✍️ Author

Scott Warren
