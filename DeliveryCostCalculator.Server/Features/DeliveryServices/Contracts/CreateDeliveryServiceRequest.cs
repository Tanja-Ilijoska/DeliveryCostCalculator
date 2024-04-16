using DeliveryCostCalculator.Server.Entities;

namespace DeliveryCostCalculator.Server.Features.DeliveryServices.Contracts
{
    public class CreateDeliveryServiceRequest
    {
        public required string Name { get; set; }
        public string Formula { get; set; } = string.Empty;
        public ICollection<DeliveryServicePropertyDto> DeliveryServiceProperties { get; set; } = [];
    }
}
