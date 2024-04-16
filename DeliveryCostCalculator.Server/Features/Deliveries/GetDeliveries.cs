using Carter;
using DeliveryCostCalculator.Server.Contracts;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.Deliveries;

public static class GetDeliveries
{

    public class Query : IRequest<Result<List<DeliveryResponse>>>
    {
    }

    internal sealed class Handler : IRequestHandler<Query, Result<List<DeliveryResponse>>>
    {
        private readonly DataContext _dbContext;

        public Handler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<DeliveryResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var deliveryResponse = await _dbContext.Deliveries.Select(x => new DeliveryResponse
            {
                Id = x.Id,
                Weight = x.Weight,
                CountryId = x.CountryId,
                DeliveryServiceId = x.DeliveryServiceId,
                Cost = x.Cost,
                Recipient = x.Recipient,
                Distance = x.Distance,
            }).ToListAsync(cancellationToken);

            if (deliveryResponse is null)
            {
                return Result.Failure<List<DeliveryResponse>>(new Error(
                    "GetDeliveryResponse.Null",
                    "The Delivery Response was not found"));
            }

            //todo
            return deliveryResponse;
        }

    }
}

public class GetDeliveriesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/deliveries", async (ISender sender) =>
        {
            var query = new GetDeliveries.Query();

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        })
            .RequireCors("AllowAll");
    }
}
