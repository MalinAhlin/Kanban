namespace Kanban.Web.Models;

public class Column
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<KanbanTask> Tasks { get; set; } = [];
}
