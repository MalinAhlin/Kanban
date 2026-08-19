using Kanban.Api.Modules.Projects.Services;

namespace Kanban.Api.Modules.Projects;

public static class ModuleRegistration
{
    public static IServiceCollection AddProjectsModule(this IServiceCollection services)
    {
        services.AddSingleton<ProjectService>();
        return services;
    }

    public static WebApplication MapProjectsModule(this WebApplication app)
    {
        var projectService = app.Services.GetRequiredService<ProjectService>();
        app.MapProjectsEndpoints(projectService);
        return app;
    }
}
