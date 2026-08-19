namespace Kanban.Api.Modules.Projects.Dtos;

public class UpdateColumnRequest
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}
