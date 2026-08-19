namespace Kanban.Api.Modules.Projects.Models;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Column> Columns { get; set; } = [];
}
