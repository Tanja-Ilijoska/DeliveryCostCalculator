namespace DeliveryCostCalculator.Server.Entities
{
    public class Delivery
    {
        public int Id { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public decimal Weight { get; set; }
        public int CountryId { get; set; }
        public int DeliveryServiceId { get; set; }
        public decimal Cost { get; set; }
        public DeliveryService DeliveryService { get; set; } = null!;
        public Country Country { get; set; } = null!;
    }
}