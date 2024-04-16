using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Features.Deliveries.Contracts;
using DeliveryCostCalculator.Server.Shared;
using MediatR;

namespace DeliveryCostCalculator.Server.Features.Deliveries
{
    public static class GetDeliveryCost
    {
        public class Query : IRequest<Result<decimal>>
        {
            public decimal Distance { get; set; }
            public decimal Weight { get; set; }
            public int CountryId { get; set; }
            public int DeliveryServiceId { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<decimal>>
        {
            private readonly DataContext _dbContext;

            public Handler(DataContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result<decimal>> Handle(Query request, CancellationToken cancellationToken)
            {
                var cost = request.Weight * request.Distance;
                var formula = "Weight * Disance";

                var service = _dbContext.DeliveryServices.FirstOrDefault(x => x.Id == request.DeliveryServiceId);
                if (service != null)
                {
                    foreach (var property in service.DeliveryServiceProperties.OrderBy(x => x.Order))
                    {
                        switch (property.Operation)
                        {
                            case "+":
                                cost += property.Value;
                                formula = $"{formula} + {property.Name}";
                                break;
                            case "-":
                                cost -= property.Value;
                                formula = $"{formula} - {property.Name}";
                                break;
                            case "/":
                                cost /= property.Value;
                                formula = $"{formula} / {property.Name}";
                                break;
                            case "*":
                                cost *= property.Value;
                                formula = $"{formula} * {property.Name}";
                                break;
                        }
                    }
                }
                var country = _dbContext.Country.SingleOrDefault(x => x.Id == request.CountryId);
                if (country != null)
                    cost *= (1m + country.CostCorrectionPercentage / 100m);

                return cost;
            }

        }
    }

    public class GetDeliveryCostEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/deliveryCost", async (DeliveryCostRequest request, ISender sender) =>
            {
                var query = new GetDeliveryCost.Query
                {
                    CountryId = request.CountryId,
                    DeliveryServiceId = request.DeliveryServiceId,
                    Distance = request.Distance,
                    Weight = request.Weight 
                };

                var result = await sender.Send(query);

                if (result.IsFailure)
                {
                    return Results.NotFound(result.Error);
                }

                return Results.Ok(result.Value);
            }).RequireCors("AllowAll"); 
        }
    }
}
