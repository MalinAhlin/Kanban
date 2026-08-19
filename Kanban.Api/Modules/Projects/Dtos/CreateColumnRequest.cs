namespace Kanban.Api.Modules.Projects.Dtos;

public class CreateColumnRequest
{
    public string Name { get; set; } = string.Empty;
    public int? Order { get; set; }
}
