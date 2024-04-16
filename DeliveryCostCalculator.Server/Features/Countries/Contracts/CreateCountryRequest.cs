namespace DeliveryCostCalculator.Server.Features.Countries.Contracts
{
    public class CreateCountryRequest
    {
        public string Name { get; set; } = string.Empty;
        public CountryType CountryType { get; set; }
        public decimal CostCorrectionPercentage { get; set; }
    }
}
