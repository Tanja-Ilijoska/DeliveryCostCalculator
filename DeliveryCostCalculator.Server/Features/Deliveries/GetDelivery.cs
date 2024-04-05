using Carter;
using DeliveryCostCalculator.Server.Contracts;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.Deliveries;


public static class GetDelivery
{

    public class Query : IRequest<Result<DeliveryResponse>>
    {
        public int Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<DeliveryResponse>>
    {
        private readonly DataContext _dbContext;

        public Handler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<DeliveryResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var delivery = await _dbContext.Deliveries.Where(x => x.Id == request.Id).Select(x => new DeliveryResponse
            {
                Id = x.Id,
                Recipient = x.Recipient,
                Cost = x.Cost,
                CountryId = x.CountryId,
                DeliveryServiceId = x.DeliveryServiceId,
                Distance = x.Distance,
                Weight = x.Weight
            }).FirstOrDefaultAsync(cancellationToken);

            if (delivery is null)
            {
                return Result.Failure<DeliveryResponse>(new Error(
                    "GetDeliveryResponse.Null",
                    "The Delivery with the specified ID was not found"));
            }

            return delivery;
        }

    }
}

public class GetDeliveryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/delivery/{id}", async (int id, ISender sender) =>
        {
            var query = new GetDelivery.Query { Id = id };

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
