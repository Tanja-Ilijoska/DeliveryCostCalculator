using Carter;
using DeliveryCostCalculator.Server.Data;
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
        private readonly DataContext _dbContext;

        public Handler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<bool>> Handle(Query request, CancellationToken cancellationToken)
        {
            var deliveryService = await _dbContext.DeliveryServices.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (deliveryService is null)
            {
                return Result.Failure<bool>(new Error(
                    "GetDeliveryServiceResponse.Null",
                    "The Delivery Service with the specified ID was not found"));
            }

            _dbContext.Remove(deliveryService);
            _dbContext.SaveChangesAsync(cancellationToken).Wait();

            return true;
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
        });
    }
}
