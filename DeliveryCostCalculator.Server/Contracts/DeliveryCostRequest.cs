namespace DeliveryCostCalculator.Server.Contracts
{
    public class DeliveryCostRequest
    {
        public decimal Distance { get; set; }
        public decimal Weight { get; set; }
        public int CountryId { get; set; }
        public int DeliveryServiceId { get; set; }
    }
}
