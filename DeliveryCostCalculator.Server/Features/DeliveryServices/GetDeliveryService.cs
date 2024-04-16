using Carter;
using DeliveryCostCalculator.Server.Contracts;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.DeliveryServices;

public static class GetDeliveryService
{

    public class Query : IRequest<Result<DeliveryServiceResponse>>
    {
        public int Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<DeliveryServiceResponse>>
    {
        private readonly DataContext _dbContext;

        public Handler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<DeliveryServiceResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var deliveryServiceResponse = await _dbContext.DeliveryServices
                                                    .Include(x => x.DeliveryServiceProperties)
                                                    .Where(x => x.Id == request.Id).Select(x => new DeliveryServiceResponse
                                                        {
                                                            Id = request.Id,
                                                            Name = x.Name,
                                                            DeliveryServiceProperties = x.DeliveryServiceProperties.ToList()
                                                        }).FirstOrDefaultAsync(cancellationToken);

            if (deliveryServiceResponse is null)
            {
                return Result.Failure<DeliveryServiceResponse>(new Error(
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