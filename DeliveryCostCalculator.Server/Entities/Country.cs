namespace DeliveryCostCalculator.Server.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CountryType { get; set; } = string.Empty;
        public decimal CostCorrectionPercentage { get; set; }
        public ICollection<Delivery> Delivery { get; set;} = [];
    }
}
