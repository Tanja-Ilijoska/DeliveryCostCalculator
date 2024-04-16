using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Features.Countries.Services;
using DeliveryCostCalculator.Server.Features.Deliveries.Services;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.DeliveryServices;


public static class DeleteDeliveryService
{

    public class Query : IRequest<Result<bool>>
    {
        public int Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<bool>>
    {
        private readonly IDeliveryServiceService _deliveryServiceService;

        public Handler(IDeliveryServiceService deliveryServiceService)
        {
            _deliveryServiceService = deliveryServiceService;
        }

        public async Task<Result<bool>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _deliveryServiceService.DeleteDeliveryService(request.Id);
        }

    }
}

public class DeleteDeliveryServiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/deliveryService/{id}", async (int id, ISender sender) =>
        {
            var query = new DeleteDeliveryService.Query { Id = id };

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        }).RequireCors("AllowAll");
    }
}
