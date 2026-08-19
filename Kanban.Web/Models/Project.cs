namespace Kanban.Web.Models;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Column> Columns { get; set; } = [];
}
