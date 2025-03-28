namespace Job.Business.Dtos.PositionDtos;

public class PositionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentPositionId { get; set; }
    public List<PositionDto>? SubPositions { get; set; }
}