namespace TodoMvp.Application.Tasks.Models
{
    public record UpdateTaskRequest(
        string Title,
        string? Description,
        bool IsCompleted,
        DateTime? DueDate);
}