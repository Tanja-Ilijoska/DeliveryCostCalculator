using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Features.Deliveries.Services;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.Deliveries;


public static class DeleteDelivery
{

    public class Query : IRequest<Result<bool>>
    {
        public int Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<bool>>
    {
        private readonly IDeliveriesService _deliveriesService;

        public Handler(IDeliveriesService deliveriesService)
        {
            _deliveriesService = deliveriesService;
        }

        public async Task<Result<bool>> Handle(Query request, CancellationToken cancellationToken)
        {            
            return await _deliveriesService.DeleteDelivery(request.Id);
        }

    }
}

public class DeleteDeliveryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/delivery/{id}", async (int id, ISender sender) =>
        {
            var query = new DeleteDelivery.Query { Id = id };

            var result = await sender.Send(query);

            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }

            return Results.Ok(result.Value);
        }).RequireCors("AllowAll");
    }
}
