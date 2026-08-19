using System.ComponentModel.DataAnnotations;
using Kanban.Web.Models;
using Kanban.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Kanban.Web.Components.Pages;

public partial class Home
{
    private List<Project> projects = [];
    private bool isLoading = true;
    private string? feedbackMessage;
    private string? errorMessage;

    // drag state
    private Guid? dragSourceColumnId;
    private Guid dragTaskId;
    private int? dragSourceIndex;

    // drop target state
    private Guid? dropTargetColumnId;
    private int? dropIndex;
    private bool isOverTask = false;

    // modal state
    private BoardActionMode activeAction = BoardActionMode.None;
    private BoardActionModel boardAction = new();
    private string selectedProjectIdString = string.Empty;
    private string selectedColumnIdString = string.Empty;

    [Inject]
    private ProjectRepository ProjectRepository { get; set; } = null!;

    private Project? CurrentProject => projects.FirstOrDefault(p => p.Id.ToString() == selectedProjectIdString) ?? projects.FirstOrDefault();

    protected override async System.Threading.Tasks.Task OnInitializedAsync()
    {
        await LoadProjectsAsync();
    }

    private async System.Threading.Tasks.Task LoadProjectsAsync()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var loadedProjects = await ProjectRepository.GetAllProjectsAsync();
            projects = loadedProjects ?? [];

            if (projects.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(selectedProjectIdString) || projects.All(p => p.Id.ToString() != selectedProjectIdString))
                {
                    selectedProjectIdString = projects[0].Id.ToString();
                }
            }
            else
            {
                selectedProjectIdString = string.Empty;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error loading projects: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    void OnDragStart(DragEventArgs e, Guid sourceColumnId, Guid taskId, int index)
    {
        dragSourceColumnId = sourceColumnId;
        dragTaskId = taskId;
        dragSourceIndex = index;
        isOverTask = false;
        dropIndex = null;
    }

    void OnColumnDragEnter(DragEventArgs e, Guid columnId)
    {
        dropTargetColumnId = columnId;
        StateHasChanged();
    }

    void OnDragEnter(DragEventArgs e, Guid targetColumnId, int index)
    {
        dropTargetColumnId = targetColumnId;
        dropIndex = index;
        isOverTask = true;
        StateHasChanged();
    }

    void OnTaskDragOver(DragEventArgs e, Guid targetColumnId, int index)
    {
        if (dropIndex != index || dropTargetColumnId != targetColumnId)
        {
            dropTargetColumnId = targetColumnId;
            dropIndex = index;
            isOverTask = true;
            StateHasChanged();
        }
    }

    void OnTaskDragLeave(DragEventArgs e)
    {
        isOverTask = false;
    }

    void OnContainerDragEnter(DragEventArgs e, Guid columnId)
    {
        var project = CurrentProject;
        if (project == null)
        {
            return;
        }

        if (dragSourceColumnId != columnId)
        {
            dropTargetColumnId = columnId;
            isOverTask = false;
            StateHasChanged();
        }
        else if (!isOverTask && dragSourceColumnId == columnId)
        {
            var column = project.Columns.FirstOrDefault(c => c.Id == columnId);
            if (column != null)
            {
                dropTargetColumnId = columnId;
                dropIndex = column.Tasks.Count;
                StateHasChanged();
            }
        }
    }

    void OnContainerDragLeave(DragEventArgs e, Guid columnId)
    {
        if (dragSourceColumnId != columnId)
        {
            dropTargetColumnId = null;
            dropIndex = null;
            isOverTask = false;
        }
    }

    void OnDrop(DragEventArgs e, Guid targetColumnId)
    {
        try
        {
            if (dragSourceColumnId == null)
            {
                return;
            }

            var project = CurrentProject;
            if (project == null)
            {
                return;
            }

            var sourceColumn = project.Columns.FirstOrDefault(c => c.Id == dragSourceColumnId);
            if (sourceColumn == null)
            {
                return;
            }

            var task = sourceColumn.Tasks.FirstOrDefault(t => t.Id == dragTaskId);
            if (task == null)
            {
                return;
            }

            int insertIndex;
            if (dropIndex.HasValue)
            {
                insertIndex = dropIndex.Value;
            }
            else
            {
                var targetColumn = project.Columns.FirstOrDefault(c => c.Id == targetColumnId);
                insertIndex = targetColumn?.Tasks.Count ?? 0;
            }

            if (dragSourceColumnId == targetColumnId)
            {
                var srcIdx = dragSourceIndex.GetValueOrDefault(-1);
                if (srcIdx < 0)
                {
                    srcIdx = sourceColumn.Tasks.FindIndex(t => t.Id == dragTaskId);
                }

                if (srcIdx >= 0 && srcIdx < sourceColumn.Tasks.Count)
                {
                    var adjustedInsertIndex = insertIndex;
                    if (srcIdx < insertIndex)
                    {
                        adjustedInsertIndex--;
                    }

                    if (adjustedInsertIndex < 0)
                    {
                        adjustedInsertIndex = 0;
                    }

                    if (adjustedInsertIndex > sourceColumn.Tasks.Count)
                    {
                        adjustedInsertIndex = sourceColumn.Tasks.Count;
                    }

                    if (srcIdx != adjustedInsertIndex)
                    {
                        sourceColumn.Tasks.RemoveAt(srcIdx);
                        sourceColumn.Tasks.Insert(adjustedInsertIndex, task);
                    }
                }
            }
            else
            {
                sourceColumn.Tasks.Remove(task);

                var targetColumn = project.Columns.FirstOrDefault(c => c.Id == targetColumnId);
                if (targetColumn == null)
                {
                    return;
                }

                var idx = insertIndex;
                if (idx < 0)
                {
                    idx = 0;
                }

                if (idx > targetColumn.Tasks.Count)
                {
                    idx = targetColumn.Tasks.Count;
                }

                targetColumn.Tasks.Insert(idx, task);
            }

            StateHasChanged();
        }
        finally
        {
            dragSourceColumnId = null;
            dragTaskId = Guid.Empty;
            dragSourceIndex = null;
            dropTargetColumnId = null;
            dropIndex = null;
            isOverTask = false;
        }
    }

    bool IsDragOver(Guid columnId) => dropTargetColumnId == columnId;

    private enum BoardActionMode
    {
        None,
        CreateProject,
        CreateColumn,
        EditColumn,
        CreateTask,
        EditTask
    }

    public class BoardActionModel
    {
        public Guid ProjectId { get; set; }
        public Guid ColumnId { get; set; }
        public Guid TaskId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public int Order { get; set; }
    }

    private string ModalTitle => activeAction switch
    {
        BoardActionMode.CreateProject => "Add New Project",
        BoardActionMode.CreateColumn => "Add New Column",
        BoardActionMode.EditColumn => "Edit Column",
        BoardActionMode.CreateTask => "Add New Task",
        BoardActionMode.EditTask => "Edit Task",
        _ => "Board Item"
    };

    private string NameLabel => activeAction switch
    {
        BoardActionMode.CreateProject => "Project Name",
        BoardActionMode.CreateColumn or BoardActionMode.EditColumn => "Column Name",
        _ => "Task Title"
    };

    private bool ShowOrderField => activeAction is BoardActionMode.CreateColumn or BoardActionMode.EditColumn;
    private bool ShowDescriptionField => activeAction is BoardActionMode.CreateTask or BoardActionMode.EditTask;
    private bool ShowColumnSelector => activeAction is BoardActionMode.CreateTask;
    private bool IsEditMode => activeAction is BoardActionMode.EditColumn or BoardActionMode.EditTask;

    private void ResetBoardAction(BoardActionMode action)
    {
        activeAction = action;
        boardAction = new BoardActionModel();
        selectedColumnIdString = string.Empty;
        errorMessage = null;
        feedbackMessage = null;
    }

    private void OpenAddProjectModal()
    {
        ResetBoardAction(BoardActionMode.CreateProject);
    }

    private void OpenAddColumnModal()
    {
        var project = CurrentProject;
        if (project == null)
        {
            errorMessage = "Select a project first.";
            return;
        }

        ResetBoardAction(BoardActionMode.CreateColumn);
        boardAction.Order = project.Columns.Count;
    }

    private void OpenEditColumnModal(Column column)
    {
        ResetBoardAction(BoardActionMode.EditColumn);
        boardAction.ColumnId = column.Id;
        boardAction.Name = column.Name;
        boardAction.Order = column.Order;
    }

    private void OpenAddTaskModal()
    {
        var project = CurrentProject;
        if (project == null || project.Columns.Count == 0)
        {
            errorMessage = "Select a project with at least one column first.";
            return;
        }

        ResetBoardAction(BoardActionMode.CreateTask);
        selectedColumnIdString = project.Columns[0].Id.ToString();
        boardAction.ColumnId = project.Columns[0].Id;
    }

    private void OpenEditTaskModal(Column column, KanbanTask task)
    {
        ResetBoardAction(BoardActionMode.EditTask);
        boardAction.ColumnId = column.Id;
        boardAction.TaskId = task.Id;
        boardAction.Name = task.Name;
        boardAction.Description = task.Description;
        selectedColumnIdString = column.Id.ToString();
    }

    private void CloseBoardActionModalAsync()
    {
        activeAction = BoardActionMode.None;
        boardAction = new BoardActionModel();
        selectedColumnIdString = string.Empty;
    }

    private async Task HandleDeleteAsync()
    {
        var project = CurrentProject;
        if (project == null)
        {
            errorMessage = "Project not found.";
            return;
        }

        switch (activeAction)
        {
            case BoardActionMode.EditColumn:
            {
                var deleted = await ProjectRepository.DeleteColumnAsync(project.Id, boardAction.ColumnId);
                if (!deleted)
                {
                    errorMessage = "Unable to delete the column.";
                    return;
                }
                feedbackMessage = "Column deleted successfully.";
                break;
            }
            case BoardActionMode.EditTask:
            {
                var deleted = await ProjectRepository.DeleteTaskAsync(project.Id, boardAction.ColumnId, boardAction.TaskId);
                if (!deleted)
                {
                    errorMessage = "Unable to delete the task.";
                    return;
                }
                feedbackMessage = "Task deleted successfully.";
                break;
            }
            default:
                return;
        }

        CloseBoardActionModalAsync();
        await LoadProjectsAsync();
    }

    private async Task HandleBoardActionAsync()
    {
        feedbackMessage = null;
        errorMessage = null;

        var action = activeAction;
        var project = CurrentProject;

        switch (action)
        {
            case BoardActionMode.CreateProject:
            {
                var createdProject = await ProjectRepository.CreateProjectAsync(new CreateProjectRequest
                {
                    Name = boardAction.Name
                });

                if (createdProject == null)
                {
                    errorMessage = "Unable to create the project.";
                    return;
                }

                selectedProjectIdString = createdProject.Id.ToString();
                feedbackMessage = $"Project '{createdProject.Name}' created.";
                break;
            }
            case BoardActionMode.CreateColumn:
            {
                if (project == null)
                {
                    errorMessage = "Select a project first.";
                    return;
                }

                var createdColumn = await ProjectRepository.AddColumnAsync(project.Id, new CreateColumnRequest
                {
                    Name = boardAction.Name,
                    Order = boardAction.Order
                });

                if (createdColumn == null)
                {
                    errorMessage = "Unable to create the column.";
                    return;
                }

                feedbackMessage = $"Column '{createdColumn.Name}' created.";
                break;
            }
            case BoardActionMode.EditColumn:
            {
                if (project == null)
                {
                    errorMessage = "Select a project first.";
                    return;
                }

                var updatedColumn = await ProjectRepository.UpdateColumnAsync(project.Id, boardAction.ColumnId, new UpdateColumnRequest
                {
                    Name = boardAction.Name,
                    Order = boardAction.Order
                });

                if (updatedColumn == null)
                {
                    errorMessage = "Unable to update the column.";
                    return;
                }

                feedbackMessage = $"Column '{updatedColumn.Name}' updated.";
                break;
            }
            case BoardActionMode.CreateTask:
            {
                if (project == null)
                {
                    errorMessage = "Select a project first.";
                    return;
                }

                if (!Guid.TryParse(selectedColumnIdString, out var columnId))
                {
                    errorMessage = "Select a column first.";
                    return;
                }

                var createdTask = await ProjectRepository.AddTaskAsync(project.Id, columnId, new CreateTaskRequest
                {
                    Name = boardAction.Name,
                    Description = boardAction.Description
                });

                if (createdTask == null)
                {
                    errorMessage = "Unable to create the task.";
                    return;
                }

                feedbackMessage = $"Task '{createdTask.Name}' created.";
                break;
            }
            case BoardActionMode.EditTask:
            {
                if (project == null)
                {
                    errorMessage = "Select a project first.";
                    return;
                }

                var updatedTask = await ProjectRepository.UpdateTaskAsync(project.Id, boardAction.ColumnId, boardAction.TaskId, new UpdateTaskRequest
                {
                    Name = boardAction.Name,
                    Description = boardAction.Description
                });

                if (updatedTask == null)
                {
                    errorMessage = "Unable to update the task.";
                    return;
                }

                feedbackMessage = $"Task '{updatedTask.Name}' updated.";
                break;
            }
            default:
                return;
        }

        CloseBoardActionModalAsync();
        await LoadProjectsAsync();
    }
}
