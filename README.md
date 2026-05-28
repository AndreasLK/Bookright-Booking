# Bookright Booking

Welcome to the **Bookright Booking** project. This is a comprehensive, modular booking management system built using **.NET** (Blazor) and designed with Clean Architecture principles.

## Project Architecture

The solution is organized into distinct layers to ensure separation of concerns and maintainability:

* **`Domain`**: The core of the system. Contains business entities (Booking, Clinic, Practitioner), value objects, domain logic, interfaces, and specifications.
* **`Infrastructure`**: Implementation details for persistence and external services. Features In-Memory repositories, `BookrightDbContext`, and implementation of infrastructure interfaces.
* **`Use Case`**: Contains application-specific business processes, including best-discount strategies and registration workflows.
* **`Facade`**: Acts as an API/Service layer that exposes services and DTOs (Data Transfer Objects) to the UI, bridging the gap between business logic and the frontend.
* **`UI`**: The frontend layer, consisting of a Blazor Web App (`UI` project) and client-side components (`UI.Client`).
* **`Tests`**: Comprehensive unit and integration tests to ensure system stability.

Follow these steps to set up the project on your local machine.

### Prerequisites

* [.NET 10.0 SDK](https://dotnet.microsoft.com/download) (or higher)
* An IDE (Visual Studio 2022, JetBrains Rider, or VS Code with C# Dev Kit)

### Installation

1.  **Clone the repository:**
    ```bash
    git clone [repository-url]
    cd bookright-booking
    ```

2.  **Open the solution:**
    Open the `Bookright Booking.sln` file in your preferred IDE.

3.  **Restore dependencies:**
    If your IDE doesn't do this automatically, run:
    ```bash
    dotnet restore
    ```

4.  **Run the application:**
    Navigate to the `UI/UI` directory and run:
    ```bash
    dotnet run
    ```

## Testing

The solution includes a dedicated `Tests` project. To run the test suite and ensure all components are working as expected:

```bash
dotnet test
