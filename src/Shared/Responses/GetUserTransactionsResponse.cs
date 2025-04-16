namespace SharedLibrary.Responses;

public class GetUserTransactionsResponse
{
    public Guid Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public byte TransactionType { get; set; }
    public byte TransactionStatus { get; set; }
    public double? Coin { get; set; }

    public Guid InformationId { get; set; }
    public byte InformationType { get; set; }
}
