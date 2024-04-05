using DeliveryCostCalculator.Server.Entities;

namespace DeliveryCostCalculator.Server.Contracts
{
    public class CreateDeliveryServiceRequest
    {
        public required string Name { get; set; }
        public string Formula { get; set; } = string.Empty;
        public ICollection<DeliveryServiceProperty> DeliveryServiceProperties { get; set; } = [];
    }
}
