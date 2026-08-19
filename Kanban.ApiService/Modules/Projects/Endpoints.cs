using Kanban.Api.Modules.Projects.Dtos;
using Kanban.Api.Modules.Projects.Models;
using Kanban.Api.Modules.Projects.Services;

namespace Kanban.Api.Modules.Projects;

public static class Endpoints
{
    public static void MapProjectsEndpoints(this WebApplication app, ProjectService projectService)
    {
        var group = app.MapGroup("/api/projects")
            .WithName("Projects");

        group.MapGet("/", GetAllProjects)
            .WithName("GetAllProjects")
            .WithDescription("Get all projects with their columns and tasks")
            .Produces<List<ProjectDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id}", GetProjectById)
            .WithName("GetProjectById")
            .WithDescription("Get a specific project by ID")
            .Produces<ProjectDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateProject)
            .WithName("CreateProject")
            .WithDescription("Create a new project")
            .Accepts<CreateProjectRequest>("application/json")
            .Produces<ProjectDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPost("/{projectId}/columns", AddColumn)
            .WithName("AddColumn")
            .WithDescription("Add a new column to a project")
            .Accepts<CreateColumnRequest>("application/json")
            .Produces<ColumnDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapPut("/{projectId}/columns/{columnId}", UpdateColumn)
            .WithName("UpdateColumn")
            .WithDescription("Update a column within a project")
            .Accepts<UpdateColumnRequest>("application/json")
            .Produces<ColumnDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapPost("/{projectId}/columns/{columnId}/tasks", AddTask)
            .WithName("AddTask")
            .WithDescription("Add a new task to a column")
            .Accepts<CreateTaskRequest>("application/json")
            .Produces<TaskDto>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapPut("/{projectId}/columns/{columnId}/tasks/{taskId}", UpdateTask)
            .WithName("UpdateTask")
            .WithDescription("Update a task within a column")
            .Accepts<UpdateTaskRequest>("application/json")
            .Produces<TaskDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem();

        group.MapDelete("/{projectId}/columns/{columnId}", DeleteColumn)
            .WithName("DeleteColumn")
            .WithDescription("Delete a column from a project")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{projectId}/columns/{columnId}/tasks/{taskId}", DeleteTask)
            .WithName("DeleteTask")
            .WithDescription("Delete a task from a column")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        async Task<IResult> GetAllProjects()
        {
            var projects = await projectService.GetAllProjectsAsync();
            var dtos = projects.Select(MapToDto).ToList();
            return Results.Ok(dtos);
        }

        async Task<IResult> GetProjectById(Guid id)
        {
            var project = await projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(MapToDto(project));
        }

        async Task<IResult> CreateProject(CreateProjectRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Name)] = ["Project name is required."]
                });
            }

            var project = await projectService.CreateProjectAsync(request);
            return Results.Created($"/api/projects/{project.Id}", MapToDto(project));
        }

        async Task<IResult> AddColumn(Guid projectId, CreateColumnRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Name)] = ["Column name is required."]
                });
            }

            var column = await projectService.AddColumnAsync(projectId, request);
            if (column == null)
            {
                return Results.NotFound();
            }

            return Results.Created($"/api/projects/{projectId}/columns/{column.Id}", MapToDto(column));
        }

        async Task<IResult> UpdateColumn(Guid projectId, Guid columnId, UpdateColumnRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Name)] = ["Column name is required."]
                });
            }

            var column = await projectService.UpdateColumnAsync(projectId, columnId, request);
            if (column == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(MapToDto(column));
        }

        async Task<IResult> AddTask(Guid projectId, Guid columnId, CreateTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Name)] = ["Task name is required."]
                });
            }

            var task = await projectService.AddTaskAsync(projectId, columnId, request);
            if (task == null)
            {
                return Results.NotFound();
            }

            return Results.Created($"/api/projects/{projectId}/columns/{columnId}/tasks/{task.Id}", MapToDto(task));
        }

        async Task<IResult> UpdateTask(Guid projectId, Guid columnId, Guid taskId, UpdateTaskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    [nameof(request.Name)] = ["Task name is required."]
                });
            }

            var task = await projectService.UpdateTaskAsync(projectId, columnId, taskId, request);
            if (task == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(MapToDto(task));
        }

        async Task<IResult> DeleteColumn(Guid projectId, Guid columnId)
        {
            var deleted = await projectService.DeleteColumnAsync(projectId, columnId);
            if (!deleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }

        async Task<IResult> DeleteTask(Guid projectId, Guid columnId, Guid taskId)
        {
            var deleted = await projectService.DeleteTaskAsync(projectId, columnId, taskId);
            if (!deleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }
    }

    private static ProjectDto MapToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Columns = project.Columns.Select(MapToDto).ToList()
        };
    }

    private static ColumnDto MapToDto(Column column)
    {
        return new ColumnDto
        {
            Id = column.Id,
            Name = column.Name,
            Order = column.Order,
            Tasks = column.Tasks.Select(MapToDto).ToList()
        };
    }

    private static TaskDto MapToDto(KanbanTask task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description
        };
    }
}
