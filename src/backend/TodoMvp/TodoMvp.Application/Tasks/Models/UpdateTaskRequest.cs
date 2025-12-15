using System.ComponentModel.DataAnnotations;

namespace TodoMvp.Application.Tasks.Models
{
    public record UpdateTaskRequest(
        [Required(AllowEmptyStrings = false)]
        [MaxLength(200)]
        string Title,
        [MaxLength(2000)]
        string? Description,
        bool IsCompleted,
        DateTime? DueDate);
}