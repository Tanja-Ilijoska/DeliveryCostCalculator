namespace DeliveryCostCalculator.Server.Entities
{
    public class DeliveryService
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Formula { get;set; }  = string.Empty;
        public ICollection<DeliveryServiceProperties> DeliveryServiceProperties { get; set; } = [];
        public ICollection<Delivery> Delivery { get; set; } = [];
    }
}
