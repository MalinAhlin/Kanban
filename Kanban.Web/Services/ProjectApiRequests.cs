namespace Kanban.Web.Services;

public class CreateProjectRequest
{
    public string Name { get; set; } = string.Empty;
}

public class CreateColumnRequest
{
    public string Name { get; set; } = string.Empty;
    public int? Order { get; set; }
}

public class UpdateColumnRequest
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class CreateTaskRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateTaskRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
