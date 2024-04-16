using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Deliveries.Contracts;
using DeliveryCostCalculator.Server.Features.Deliveries.Services;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.Deliveries;


public static class GetDelivery
{

    public class Query : IRequest<Result<DeliveryDto>>
    {
        public int Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<DeliveryDto>>
    {
        private readonly IDeliveriesService _deliveriesService;

        public Handler(IDeliveriesService deliveriesService)
        {
            _deliveriesService = deliveriesService;
        }


        public async Task<Result<DeliveryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var delivery = _deliveriesService.GetDelivery(request.Id);

            if (delivery is null)
            {
                return Result.Failure<DeliveryDto>(new Error(
                    "GetDeliveryResponse.Null",
                    "The Delivery with the specified ID was not found"));
            }

            return await delivery;
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
