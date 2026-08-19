namespace Kanban.Api.Modules.Projects.Dtos;

public class UpdateTaskRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
