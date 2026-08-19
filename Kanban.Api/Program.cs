using Kanban.Api.Modules.Projects;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kanban.Api.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

builder.Services.AddProjectsModule();

builder.Services.AddOpenApis();

var app = builder.Build();

app.UseExceptionHandler();

app.UseOpenApi();

// Map module endpoints
app.MapProjectsModule();

app.MapDefaultEndpoints();

app.Run();
