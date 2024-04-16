namespace DeliveryCostCalculator.Server.Features.Deliveries.Contracts
{
    public class DeliveryDto
    {
        public int Id { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public decimal Weight { get; set; }
        public int CountryId { get; set; }
        public int DeliveryServiceId { get; set; }
        public decimal Cost { get; set; }
    }
}
