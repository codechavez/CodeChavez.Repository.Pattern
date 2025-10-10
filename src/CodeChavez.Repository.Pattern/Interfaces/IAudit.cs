namespace CodeChavez.Repository.Pattern.Interfaces;

public interface IAudit
{
    bool IsActive { get; set; }
    bool IsDeleted { get; set; }
    DateTime CreatedDate { get; set; }
    string? CreatedBy { get; set; }
    DateTime? ModifiedDate { get; set; }
    string? ModifiedBy { get; set; }
}