using CodeChavez.Repository.Pattern.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodeChavez.Repository.Pattern;

public record class Document : IAudit
{
    [BsonId]
    public ObjectId Id { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
    public string? ModifiedBy { get; set; }
}
