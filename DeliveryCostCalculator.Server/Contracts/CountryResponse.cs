namespace DeliveryCostCalculator.Server.Contracts
{
    public class CountryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CountryType { get; set; } = string.Empty;
        public decimal CostCorrectionPercentage { get; set; }
    }
}
