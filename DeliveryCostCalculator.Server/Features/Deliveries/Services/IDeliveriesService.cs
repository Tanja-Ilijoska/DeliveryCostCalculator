using DeliveryCostCalculator.Server.Features.Countries.Contracts;
using DeliveryCostCalculator.Server.Features.Deliveries.Contracts;

namespace DeliveryCostCalculator.Server.Features.Deliveries.Services
{
    public interface IDeliveriesService
    {
        public Task<DeliveryDto?> GetDelivery(int id);

        public Task<List<DeliveryDto>> GetDeliveriesAsync();

        public Task<bool> DeleteDelivery(int id);

        public Task<int> CreateDeliveryAsync(CreateDelivery.Command request);

        public Task<DeliveryDto> UpdateDelivery(UpdateDelivery.Command request);
    }
}
