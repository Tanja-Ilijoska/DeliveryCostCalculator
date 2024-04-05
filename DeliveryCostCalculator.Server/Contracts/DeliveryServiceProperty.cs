using Microsoft.Identity.Client;

namespace DeliveryCostCalculator.Server.Contracts
{
    public class DeliveryServiceProperty
    {
        public int Id { get; set; }
        public int DeliveryServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public int Order { get; set; }
    }
}
