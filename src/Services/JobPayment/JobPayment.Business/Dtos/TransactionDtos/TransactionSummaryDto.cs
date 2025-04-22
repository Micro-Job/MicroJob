using JobPayment.Core.Enums;
using SharedLibrary.Enums;

namespace JobPayment.Business.Dtos.TransactionDtos;

public class TransactionSummaryDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public double? Coin { get; set; }

    public Guid InformationId { get; set; }
    public InformationType InformationType { get; set; }
}
