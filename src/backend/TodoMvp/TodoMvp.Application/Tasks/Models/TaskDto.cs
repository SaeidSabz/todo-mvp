namespace TodoMvp.Application.Tasks.Models
{
    public record TaskDto(
        int Id,
        string Title,
        string? Description,
        bool IsCompleted,
        DateTime? DueDate,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}
