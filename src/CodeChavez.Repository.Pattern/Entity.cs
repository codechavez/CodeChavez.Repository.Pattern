using CodeChavez.Repository.Pattern.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CodeChavez.Repository.Pattern;

public record class Entity : IAudit
{
    [Key]
    public Guid Id { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
    public string? ModifiedBy { get; set; }
}
