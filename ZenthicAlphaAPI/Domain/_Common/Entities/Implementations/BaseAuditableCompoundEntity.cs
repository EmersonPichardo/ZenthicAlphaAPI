namespace Domain._Common.Entities.Implementations;

public abstract class BaseAuditableCompoundEntity
    : BaseCompoundEntity, IAuditableEntity
{
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    public DateTime LastModificationDate { get; set; } = DateTime.UtcNow;
    public Guid? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletionDate { get; set; }
    public Guid? DeletedBy { get; set; }
}