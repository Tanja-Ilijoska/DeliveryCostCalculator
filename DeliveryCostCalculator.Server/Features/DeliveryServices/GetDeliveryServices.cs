using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Features.DeliveryServices.Contracts;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.DeliveryServices;


public static class GetDeliveryServices
{

    public class Query : IRequest<Result<List<DeliveryServiceDto>>>
    {
        public int Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<List<DeliveryServiceDto>>>
    {
        private readonly DataContext _dbContext;

        public Handler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<DeliveryServiceDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var deliveryServiceResponse = await _dbContext.DeliveryServices.Include(x=>x.DeliveryServiceProperties).Select(x => new DeliveryServiceDto
            {
                Id = x.Id,
                Name = x.Name,
                DeliveryServiceProperties = x.DeliveryServiceProperties.ToList()
            }).ToListAsync(cancellationToken);

            if (deliveryServiceResponse is null)
            {
                return Result.Failure<List<DeliveryServiceDto>>(new Error(
                    "GetDeliveryServiceResponse.Null",
                    "The Delivery Service Response with the specified ID was not found"));
            }

            return deliveryServiceResponse;
        }

    }
}

public class GetDeliveryServicesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/deliveryServices", async (ISender sender) =>
        {
            var query = new GetDeliveryServices.Query {};

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