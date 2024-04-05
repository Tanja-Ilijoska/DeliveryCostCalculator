namespace DeliveryCostCalculator.Server.Entities
{
    public class DeliveryServiceProperties
    {
        public int Id { get; set; }
        public int DeliveryServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public int Order {  get; set; }
        public string Operation { get; set; } = string.Empty;

        public DeliveryService DeliveryService = null!;
    }
}
