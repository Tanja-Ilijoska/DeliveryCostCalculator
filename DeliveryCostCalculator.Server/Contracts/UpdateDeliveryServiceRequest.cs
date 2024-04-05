namespace DeliveryCostCalculator.Server.Contracts
{
    public class UpdateDeliveryServiceRequest : CreateDeliveryServiceRequest
    {
        public int Id { get; set; }
    }
}
