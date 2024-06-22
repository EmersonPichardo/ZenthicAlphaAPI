namespace Domain._Common.Entities.Abstractions;

public interface IAuditableEntity : IEntity
{
    public DateTime CreationDate { get; set; }
    public Guid? CreatedBy { get; set; }

    public DateTime LastModificationDate { get; set; }
    public Guid? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletionDate { get; set; }
    public Guid? DeletedBy { get; set; }
}