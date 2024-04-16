namespace DeliveryCostCalculator.Server.Features.Countries.Contracts
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CountryType { get; set; } = string.Empty;
        public decimal CostCorrectionPercentage { get; set; }
    }
}
