using System.Net.Http.Json;
using Kanban.Web.Models;

namespace Kanban.Web.Services;

public class ProjectRepository(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<Project>?> GetAllProjectsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Project>>("/api/projects");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching projects: {ex.Message}");
            return null;
        }
    }

    public async Task<Project?> GetProjectByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Project>($"/api/projects/{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching project {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Project?> CreateProjectAsync(CreateProjectRequest request)
    {
        return await SendAsync<Project>(HttpMethod.Post, "/api/projects", request, "project");
    }

    public async Task<Column?> AddColumnAsync(Guid projectId, CreateColumnRequest request)
    {
        return await SendAsync<Column>(HttpMethod.Post, $"/api/projects/{projectId}/columns", request, "column");
    }

    public async Task<Column?> UpdateColumnAsync(Guid projectId, Guid columnId, UpdateColumnRequest request)
    {
        return await SendAsync<Column>(HttpMethod.Put, $"/api/projects/{projectId}/columns/{columnId}", request, "column");
    }

    public async Task<KanbanTask?> AddTaskAsync(Guid projectId, Guid columnId, CreateTaskRequest request)
    {
        return await SendAsync<KanbanTask>(HttpMethod.Post, $"/api/projects/{projectId}/columns/{columnId}/tasks", request, "task");
    }

    public async Task<KanbanTask?> UpdateTaskAsync(Guid projectId, Guid columnId, Guid taskId, UpdateTaskRequest request)
    {
        return await SendAsync<KanbanTask>(HttpMethod.Put, $"/api/projects/{projectId}/columns/{columnId}/tasks/{taskId}", request, "task");
    }

    public async Task<bool> DeleteColumnAsync(Guid projectId, Guid columnId)
    {
        return await SendDeleteAsync($"/api/projects/{projectId}/columns/{columnId}", "column");
    }

    public async Task<bool> DeleteTaskAsync(Guid projectId, Guid columnId, Guid taskId)
    {
        return await SendDeleteAsync($"/api/projects/{projectId}/columns/{columnId}/tasks/{taskId}", "task");
    }

    private async Task<bool> SendDeleteAsync(string requestUri, string resourceName)
    {
        try
        {
            using var response = await _httpClient.DeleteAsync(requestUri);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error deleting {resourceName} at {requestUri}: {(int)response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting {resourceName} at {requestUri}: {ex.Message}");
            return false;
        }
    }

    private async Task<T?> SendAsync<T>(HttpMethod method, string requestUri, object payload, string resourceName)
    {
        try
        {
            using var request = new HttpRequestMessage(method, requestUri)
            {
                Content = JsonContent.Create(payload)
            };

            using var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error {resourceName} request to {requestUri}: {(int)response.StatusCode} {response.ReasonPhrase}");
                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {resourceName} request to {requestUri}: {ex.Message}");
            return default;
        }
    }
}

