namespace Kanban.Api.Modules.Projects.Dtos;

public class ColumnDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<TaskDto> Tasks { get; set; } = [];
}
