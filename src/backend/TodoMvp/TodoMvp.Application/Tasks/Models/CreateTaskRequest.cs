namespace TodoMvp.Application.Tasks.Models
{
    public record CreateTaskRequest(
        string Title,
        string? Description,
        DateTime? DueDate);
}
