using DeliveryCostCalculator.Server.Entities;

namespace DeliveryCostCalculator.Server.Features.DeliveryServices.Contracts
{
    public class DeliveryServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Formula { get; set; } = string.Empty;

        public List<DeliveryServiceProperties> DeliveryServiceProperties { get; set; }
    }
}
