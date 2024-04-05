using Carter;
using DeliveryCostCalculator.Server.Contracts;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.Countries;

public static class DeleteCountry
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
            var country = await _dbContext.Country.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (country is null)
            {
                return Result.Failure<bool>(new Error(
                    "GetCountryResponse.Null",
                    "The Country with the specified ID was not found"));
            }

            _dbContext.Remove(country);
            _dbContext.SaveChangesAsync(cancellationToken).Wait();

            return true;
        }

    }
}

public class DeleteCountryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/country/{id}", async (int id, ISender sender) =>
        {
            var query = new DeleteCountry.Query { Id = id };

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
