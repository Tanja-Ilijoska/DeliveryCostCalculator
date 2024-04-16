using Carter;
using DeliveryCostCalculator.Server.Features.Deliveries.Contracts;
using DeliveryCostCalculator.Server.Features.Deliveries.Services;
using DeliveryCostCalculator.Server.Shared;
using MediatR;

namespace DeliveryCostCalculator.Server.Features.Deliveries;

public static class GetDeliveries
{

    public class Query : IRequest<Result<List<DeliveryDto>>>
    {
    }

    internal sealed class Handler : IRequestHandler<Query, Result<List<DeliveryDto>>>
    {
        private readonly IDeliveriesService _deliveriesService;

        public Handler(IDeliveriesService deliveriesService)
        {
            _deliveriesService = deliveriesService;
        }


        public async Task<Result<List<DeliveryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var deliveryResponse = await _deliveriesService.GetDeliveriesAsync();

            if (deliveryResponse is null)
            {
                return Result.Failure<List<DeliveryDto>>(new Error(
                    "GetDeliveryResponse.Null",
                    "The Delivery Response was not found"));
            }

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
