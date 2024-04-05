namespace DeliveryCostCalculator.Server.Contracts
{
    public class CreateCountryRequest
    {
        public string Name { get; set; } = string.Empty;
        public CountryType CountryType { get; set; } 
        public decimal CostCorrectionPercentage { get; set; }
    }
}
