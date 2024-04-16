using DeliveryCostCalculator.Server.Features.DeliveryServices;
using DeliveryCostCalculator.Server.Features.DeliveryServices.Contracts;

namespace DeliveryCostCalculator.Server.Features.Countries.Services
{
    public interface IDeliveryServiceService
    {
        public Task<DeliveryServiceDto?> GetDeliveryService(int id);

        public Task<List<DeliveryServiceDto>> GetDeliveryServicesAsync();

        public Task<bool> DeleteDeliveryService(int id);

        public int CreateDeliveryService(CreateDeliveryService.Command request);

        public Task<DeliveryServiceDto> UpdateDeliveryService(UpdateDeliveryService.Command request);
    }
}
