using DeliveryCostCalculator.Server.Features.Countries.Contracts;
using DeliveryCostCalculator.Server.Features.Deliveries.Contracts;

namespace DeliveryCostCalculator.Server.Features.Deliveries.Services
{
    public interface ICalculationService
    {
        public Task<decimal> CalculateCost(GetDeliveryCost.Query request);
    }
}
