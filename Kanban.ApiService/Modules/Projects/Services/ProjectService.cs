using Kanban.Api.Modules.Projects.Dtos;
using Kanban.Api.Modules.Projects.Models;

namespace Kanban.Api.Modules.Projects.Services;

public class ProjectService
{
    private static readonly List<Project> Projects = [];

    public ProjectService()
    {
        // Replace with database initialization logic.
        // For now, we will seed some initial data for demonstration purposes.
        if (Projects.Count == 0)
        {
            SeedData();
        }
    }

    public Task<List<Project>> GetAllProjectsAsync()
    {
        return Task.FromResult(Projects);
    }

    public Task<Project?> GetProjectByIdAsync(Guid id)
    {
        var project = Projects.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(project);
    }

    public Task<Project> CreateProjectAsync(CreateProjectRequest request)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Columns = []
        };

        Projects.Add(project);
        return Task.FromResult(project);
    }

    public Task<Column?> AddColumnAsync(Guid projectId, CreateColumnRequest request)
    {
        var project = Projects.FirstOrDefault(p => p.Id == projectId);
        if (project == null)
        {
            return Task.FromResult<Column?>(null);
        }

        var column = new Column
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Order = request.Order ?? project.Columns.Count,
            Tasks = []
        };

        project.Columns.Add(column);
        NormalizeColumnOrder(project);

        return Task.FromResult<Column?>(column);
    }

    public Task<Column?> UpdateColumnAsync(Guid projectId, Guid columnId, UpdateColumnRequest request)
    {
        var project = Projects.FirstOrDefault(p => p.Id == projectId);
        if (project == null)
        {
            return Task.FromResult<Column?>(null);
        }

        var column = project.Columns.FirstOrDefault(c => c.Id == columnId);
        if (column == null)
        {
            return Task.FromResult<Column?>(null);
        }

        column.Name = request.Name.Trim();

        var currentIndex = project.Columns.FindIndex(c => c.Id == columnId);
        var targetIndex = Math.Clamp(request.Order, 0, project.Columns.Count - 1);

        if (currentIndex != targetIndex)
        {
            project.Columns.RemoveAt(currentIndex);
            project.Columns.Insert(targetIndex, column);
        }

        NormalizeColumnOrder(project);
        return Task.FromResult<Column?>(column);
    }

    public Task<KanbanTask?> AddTaskAsync(Guid projectId, Guid columnId, CreateTaskRequest request)
    {
        var project = Projects.FirstOrDefault(p => p.Id == projectId);
        if (project == null)
        {
            return Task.FromResult<KanbanTask?>(null);
        }

        var column = project.Columns.FirstOrDefault(c => c.Id == columnId);
        if (column == null)
        {
            return Task.FromResult<KanbanTask?>(null);
        }

        var task = new KanbanTask
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty
        };

        column.Tasks.Add(task);
        return Task.FromResult<KanbanTask?>(task);
    }

    public Task<KanbanTask?> UpdateTaskAsync(Guid projectId, Guid columnId, Guid taskId, UpdateTaskRequest request)
    {
        var project = Projects.FirstOrDefault(p => p.Id == projectId);
        if (project == null)
        {
            return Task.FromResult<KanbanTask?>(null);
        }

        var column = project.Columns.FirstOrDefault(c => c.Id == columnId);
        if (column == null)
        {
            return Task.FromResult<KanbanTask?>(null);
        }

        var task = column.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null)
        {
            return Task.FromResult<KanbanTask?>(null);
        }

        task.Name = request.Name.Trim();
        task.Description = request.Description?.Trim() ?? string.Empty;
        return Task.FromResult<KanbanTask?>(task);
    }

    public Task<bool> DeleteColumnAsync(Guid projectId, Guid columnId)
    {
        var project = Projects.FirstOrDefault(p => p.Id == projectId);
        if (project == null)
        {
            return Task.FromResult(false);
        }

        var column = project.Columns.FirstOrDefault(c => c.Id == columnId);
        if (column == null)
        {
            return Task.FromResult(false);
        }

        project.Columns.Remove(column);
        NormalizeColumnOrder(project);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteTaskAsync(Guid projectId, Guid columnId, Guid taskId)
    {
        var project = Projects.FirstOrDefault(p => p.Id == projectId);
        if (project == null)
        {
            return Task.FromResult(false);
        }

        var column = project.Columns.FirstOrDefault(c => c.Id == columnId);
        if (column == null)
        {
            return Task.FromResult(false);
        }

        var task = column.Tasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null)
        {
            return Task.FromResult(false);
        }

        column.Tasks.Remove(task);
        return Task.FromResult(true);
    }

    private static void NormalizeColumnOrder(Project project)
    {
        for (var i = 0; i < project.Columns.Count; i++)
        {
            project.Columns[i].Order = i;
        }
    }

    private static void SeedData()
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Project 1",
            Columns =
            [
                new Column
                {
                    Id = Guid.NewGuid(),
                    Name = "To Do",
                    Order = 0,
                    Tasks =
                    [
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Task 1",
                            Description = "Description for Task 1"
                        }
                    ]
                },
                new Column
                {
                    Id = Guid.NewGuid(),
                    Name = "In Progress",
                    Order = 1,
                    Tasks =
                    [
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Task 2",
                            Description = "Description for Task 2"
                        }
                    ]
                },
                new Column
                {
                    Id = Guid.NewGuid(),
                    Name = "Done",
                    Order = 2,
                    Tasks =
                    [
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = "Task 3",
                            Description = "Description for Task 3"
                        }
                    ]
                }
            ]
        };

        Projects.Add(project);
    }
}
