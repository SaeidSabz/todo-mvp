using System.ComponentModel.DataAnnotations;

namespace TodoMvp.Application.Tasks.Models
{
    public record CreateTaskRequest(
        [Required(AllowEmptyStrings = false)]
        [MaxLength(200)]
        string Title,
        [MaxLength(2000)]
        string? Description,
        DateTime? DueDate);
}
