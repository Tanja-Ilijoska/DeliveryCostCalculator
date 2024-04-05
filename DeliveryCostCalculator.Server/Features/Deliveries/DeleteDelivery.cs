using Carter;
using DeliveryCostCalculator.Server.Data;
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
        private readonly DataContext _dbContext;

        public Handler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<bool>> Handle(Query request, CancellationToken cancellationToken)
        {
            var delivery = await _dbContext.Deliveries.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (delivery is null)
            {
                return Result.Failure<bool>(new Error(
                    "GetDeliveryResponse.Null",
                    "The Delivery with the specified ID was not found"));
            }

            _dbContext.Remove(delivery);
            _dbContext.SaveChangesAsync(cancellationToken).Wait();

            return true;
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
