using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Features.Countries.Services;
using DeliveryCostCalculator.Server.Features.Deliveries.Contracts;
using DeliveryCostCalculator.Server.Features.Deliveries.Services;
using DeliveryCostCalculator.Server.Shared;
using FluentValidation;
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
            private readonly ICalculationService _calculationService;
            private readonly DataContext _dbContext;

            public Handler(ICalculationService calculationService, DataContext dbContext)
            {
                _calculationService = calculationService;
                _dbContext = dbContext;
            }

            public async Task<Result<decimal>> Handle(Query request, CancellationToken cancellationToken)
            {
                var cost = await _calculationService.CalculateCost(request);

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
