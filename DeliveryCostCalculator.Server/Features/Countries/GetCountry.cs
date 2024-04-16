using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Countries.Contracts;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace DeliveryCostCalculator.Server.Features.Countries;

public static class GetCountry
{

    public class Query : IRequest<Result<CountryDto>>
    {
        public int Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<CountryDto>>
    {
        private readonly DataContext _dbContext;

        public Handler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<CountryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var country = await _dbContext.Country.Where(x => x.Id == request.Id).Select(x => new CountryDto
            {
                Id = request.Id,
                Name = x.Name,
                CountryType = x.CountryType,
                CostCorrectionPercentage = x.CostCorrectionPercentage,
            }).FirstOrDefaultAsync(cancellationToken);

            if (country is null)
            {
                return Result.Failure<CountryDto>(new Error(
                    "GetCountryResponse.Null",
                    "The Country with the specified ID was not found"));
            }

            return country;
        }

    }
}

public class GetCountryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/country/{id}", async (int id, ISender sender) =>
        {
            var query = new GetCountry.Query { Id = id };

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