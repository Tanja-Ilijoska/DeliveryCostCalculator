using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Features.Countries.Services;
using DeliveryCostCalculator.Server.Features.Deliveries.Services;
using DeliveryCostCalculator.Server.Features.DeliveryServices.Contracts;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.DeliveryServices;

public static class GetDeliveryService
{

    public class Query : IRequest<Result<DeliveryServiceDto>>
    {
        public int Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<DeliveryServiceDto>>
    {
        private readonly IDeliveryServiceService _deliveryServiceService;

        public Handler(IDeliveryServiceService deliveryServiceService)
        {
            _deliveryServiceService = deliveryServiceService;
        }

        public async Task<Result<DeliveryServiceDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var deliveryServiceResponse = await _deliveryServiceService.GetDeliveryService(request.Id);

            if (deliveryServiceResponse is null)
            {
                return Result.Failure<DeliveryServiceDto>(new Error(
                    "GetDeliveryServiceResponse.Null",
                    "The Delivery Service Response with the specified ID was not found"));
            }

            return deliveryServiceResponse;
        }

    }
}

public class GetDeliveryServiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/deliveryService/{id}", async (int id, ISender sender) =>
        {
            var query = new GetDeliveryService.Query { Id = id };

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        }).RequireCors("AllowAll");
    }
}