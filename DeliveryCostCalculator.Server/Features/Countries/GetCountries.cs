using Carter;
using DeliveryCostCalculator.Server.Contracts;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DeliveryCostCalculator.Server.Features.Countries;


public static class GetCountries
{

    public class Query : IRequest<Result<List<CountryResponse>>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CountryType CountryType { get; set; }
        public decimal CostCorrectionPercentage { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<List<CountryResponse>>>
    {
        private readonly DataContext _dbContext;

        public Handler(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<List<CountryResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {            
            var countryResponse = await _dbContext.Country.Select(x => new CountryResponse
            {
                Id = request.Id,
                Name = x.Name,
                CostCorrectionPercentage = x.CostCorrectionPercentage,
                CountryType = x.CountryType,
            }).ToListAsync(cancellationToken);

            if (countryResponse is null)
            {
                return Result.Failure<List<CountryResponse>> (new Error(
                    "GetCountryResponse.Null",
                    "The Country Response was not found"));
            }

            //todo
            return countryResponse;
        }

    }
}

public class GetCountriesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/countries", async (ISender sender) =>
        {
            var query = new GetCountries.Query();

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
