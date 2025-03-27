namespace Job.Core.Entities;

public class Position : BaseEntity
{
    public string Name { get; set; }
    public bool IsActive { get; set; } = false;

    public Guid? ParentPositionId { get; set; }
    public Position? ParentPosition { get; set; }

    public ICollection<Position>? SubPositions { get; set; }
    public ICollection<Resume> Resumes { get; set; }
}