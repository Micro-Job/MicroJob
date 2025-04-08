using SharedLibrary.Enums;


namespace JobPayment.Business.Dtos.PriceDtos
{
    public class PriceListDto
    {
        public Guid Id { get; set; }
        public InformationType InformationType { get; set; }
        public double Coin { get; set; }

        public ICollection<OldPriceDto>? OldPrices { get; set; }
    }
}
