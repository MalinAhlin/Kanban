# Kanban Board

A .NET 10 modular monolith Kanban board application with a Blazor Web frontend and minimal API backend.

## Prerequisites

- **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
- **Aspire Workload** - Required for running the AppHost orchestrator
  ```powershell
  dotnet workload install aspire
  ```
- **Visual Studio 2022+** or **Visual Studio Code** with C# extension

## Getting Started

### Running the Solution

1. **Clone/open the solution**
   ```powershell
   cd path/to/Kanban
   ```

2. **Build the solution**
   ```powershell
   dotnet build Kanban.slnx
   ```

3. **Run with Aspire orchestration** (recommended)

   **Command line:**
   ```powershell
   dotnet run --project Kanban.AppHost
   ```
   The Aspire dashboard will open automatically, showing:
   - API Service (http://localhost:5000)
   - Web Project (http://localhost:5001)

   **Or in Visual Studio:**
   - Set `Kanban.AppHost` as the startup project
   - Press F5 or click Run

4. **Or run projects individually**
   ```powershell
   # Terminal 1: Start API
   dotnet run --project Kanban.ApiService

   # Terminal 2: Start Web
   dotnet run --project Kanban.Web
   ```

## Project Structure

```
Kanban/
├── Kanban.AppHost/              # Aspire orchestrator (run this to start everything)
├── Kanban.ServiceDefaults/      # Shared service configuration
├── Kanban.ApiService/           # Minimal API backend
│   └── Modules/
│       └── Projects/            # Projects module (owns data models)
│           ├── Models/          # Domain models (Project, Column, KanbanTask)
│           ├── Dtos/            # Public API contracts
│           ├── Services/        # Business logic
│           └── Endpoints.cs     # Route handlers
├── Kanban.Web/                  # Blazor Server frontend
│   └── Components/
│       └── Pages/               # Page components (Home.razor)
└── Kanban.slnx                  # Solution file (use this, not .sln)
```

## Architecture Notes

### Modular Monolith Design
- **API module ownership**: The `Projects` module in `Kanban.ApiService` owns its data models (`Project`, `Column`, `KanbanTask`)
- **Public contracts via DTOs**: Models are published as DTOs in the API module's public interface
- **Web consumption**: The `Kanban.Web` project consumes API DTOs via HTTP, no direct model sharing
- **Extraction-ready**: When the Web project is extracted to a separate solution, it only needs a reference to the API package

### Key Technologies
- **Aspire**: Cloud-native orchestration and dashboard
- **Blazor Server**: Interactive web components
- **Minimal APIs**: Lightweight HTTP endpoints
- **Entity models with GUIDs**: Type-safe identification (columns, tasks)

## Development Workflow

### Adding Features
1. Add/modify models in `Kanban.ApiService/Modules/Projects/Models/`
2. Update DTOs in `Kanban.ApiService/Modules/Projects/Dtos/` to match
3. Create endpoints in `Kanban.ApiService/Modules/Projects/Endpoints.cs`
4. Update `Kanban.Web` component models and calls to match API DTOs

### Running Tests
```powershell
dotnet test
```

## Troubleshooting

### "Aspire workload not installed"
```powershell
dotnet workload install aspire
```

### Port conflicts (5000/5001 already in use)
- Change ports in `Kanban.AppHost/Program.cs`
- Or stop other services using those ports

### Solution won't open
- Use `Kanban.slnx` (not `Kanban.sln`)
- Requires Visual Studio 2022 or later

## Future Enhancements

- Database persistence (replace in-memory seeding)
- Authentication & authorization
- Real-time updates (SignalR)
- Extracting Web to separate solution/deployment
