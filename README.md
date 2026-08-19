# Kanban Board

A .NET 10 modular monolith Kanban board application with a Blazor Server frontend and minimal API backend. Features full CRUD operations on columns and tasks with drag-and-drop support and comprehensive unit tests.

## Features

### Core Functionality
- ✅ **Create, Read, Update, Delete (CRUD)** - Full management of projects, columns, and tasks
- ✅ **Drag & Drop** - Move tasks between columns with visual feedback
- ✅ **Responsive Design** - Works on desktop, tablet, and mobile
- ✅ **Modern UI** - Dark theme with smooth animations and transitions
- ✅ **Project Management** - Organize work into multiple projects
- ✅ **Task Management** - Add descriptions to tasks and manage status across columns

### Technical Features
- ✅ **Unit Tests** - Comprehensive xUnit test suite covering all CRUD operations
- ✅ **Modular Architecture** - Well-organized project structure with clear separation of concerns
- ✅ **State Management** - Component-based state with modal-driven editing
- ✅ **RESTful API** - Minimal APIs with proper HTTP status codes (201, 204, 404, etc.)

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
   dotnet run --project Kanban.Api

   # Terminal 2: Start Web
   dotnet run --project Kanban.Web
   ```

## Project Structure

```
Kanban/
├── Kanban.AppHost/              # Aspire orchestrator (run this to start everything)
├── Kanban.ServiceDefaults/      # Shared service configuration
├── Kanban.Api/           # Minimal API backend
│   └── Modules/
│       └── Projects/            # Projects module (owns data models)
│           ├── Models/          # Domain models (Project, Column, KanbanTask)
│           ├── Dtos/            # Public API contracts
│           ├── Services/        # Business logic
│           └── Endpoints.cs     # Route handlers (GET, POST, PUT, DELETE)
├── Kanban.Web/                  # Blazor Server frontend
│   ├── Components/
│   │   └── Pages/               # Page components (Home.razor)
│   └── Services/                # Repository & API client (ProjectRepository)
├── Kanban.Tests/                # xUnit tests for ProjectService
└── Kanban.slnx                  # Solution file (use this, not .sln)
```

## API Endpoints

### Projects
- `GET /api/projects` - Get all projects
- `GET /api/projects/{id}` - Get a specific project
- `POST /api/projects` - Create a new project
- `DELETE /api/projects/{id}` - Delete a project (not yet exposed in UI)

### Columns
- `POST /api/projects/{projectId}/columns` - Add a column
- `PUT /api/projects/{projectId}/columns/{columnId}` - Update a column
- `DELETE /api/projects/{projectId}/columns/{columnId}` - Delete a column

### Tasks
- `POST /api/projects/{projectId}/columns/{columnId}/tasks` - Add a task
- `PUT /api/projects/{projectId}/columns/{columnId}/tasks/{taskId}` - Update a task
- `DELETE /api/projects/{projectId}/columns/{columnId}/tasks/{taskId}` - Delete a task

## Architecture Notes

### Modular Monolith Design
- **API module ownership**: The `Projects` module in `Kanban.Api` owns its data models (`Project`, `Column`, `KanbanTask`)
- **Public contracts via DTOs**: Models are published as DTOs in the API module's public interface
- **Web consumption**: The `Kanban.Web` project consumes API DTOs via HTTP, no direct model sharing
- **Extraction-ready**: When the Web project is extracted to a separate solution, it only needs a reference to the API package

### Key Technologies
- **Aspire**: Cloud-native orchestration and dashboard
- **Blazor Server**: Interactive web components with stateful event handling
- **Minimal APIs**: Lightweight HTTP endpoints with proper OpenAPI documentation
- **xUnit**: Unit testing framework for service layer testing
- **Entity models with GUIDs**: Type-safe identification (columns, tasks)

## Development Workflow

### Adding Features
1. Add/modify models in `Kanban.Api/Modules/Projects/Models/`
2. Update DTOs in `Kanban.Api/Modules/Projects/Dtos/` to match
3. Add/update endpoints in `Kanban.Api/Modules/Projects/Endpoints.cs`
4. Add service methods in `Kanban.Api/Modules/Projects/Services/ProjectService.cs`
5. Update `Kanban.Web` component models and repository calls
6. Write tests in `Kanban.Tests/ProjectServiceTests.cs`

### Running Tests
```powershell
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=ProjectServiceTests"

# Run with verbose output
dotnet test --logger:"console;verbosity=detailed"
```

## UI Features

### Modal Dialog System
- Edit modals for columns and tasks appear as overlays
- Delete buttons visible only in edit mode
- Form validation with required field indicators
- Smooth animations on modal open/close

### Drag & Drop
- Visual drop placeholders while dragging
- Reorder tasks within columns or move to different columns
- Real-time visual feedback with hover states

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

### Tests not discovered
- Ensure `Kanban.Tests.csproj` references `Kanban.Api.csproj`
- Run `dotnet build` to ensure test project compiles
- In Visual Studio, go to Test > Test Explorer and refresh

## Future Enhancements

- Database persistence (replace in-memory seeding with Entity Framework)
- Authentication & authorization (identity management)
- Real-time updates (SignalR for multi-user sync)
- Extracting Web to separate solution/deployment
- Task filtering by status or assignee
- Project-level permissions and sharing
- Bulk operations (multi-select, bulk delete)
