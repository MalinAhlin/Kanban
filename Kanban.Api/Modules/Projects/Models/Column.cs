namespace Kanban.Api.Modules.Projects.Models;

public class Column
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<KanbanTask> Tasks { get; set; } = [];
}
