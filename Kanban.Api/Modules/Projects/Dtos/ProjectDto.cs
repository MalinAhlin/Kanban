namespace Kanban.Api.Modules.Projects.Dtos;

public class ProjectDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public List<ColumnDto> Columns { get; set; } = [];
}
