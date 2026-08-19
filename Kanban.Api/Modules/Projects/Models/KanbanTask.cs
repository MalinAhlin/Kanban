namespace Kanban.Api.Modules.Projects.Models;

public class KanbanTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
