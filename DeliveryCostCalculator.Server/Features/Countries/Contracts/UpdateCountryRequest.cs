namespace DeliveryCostCalculator.Server.Features.Countries.Contracts
{
    public class UpdateCountryRequest : CreateCountryRequest
    {
        public int Id { get; set; }
    }
}
