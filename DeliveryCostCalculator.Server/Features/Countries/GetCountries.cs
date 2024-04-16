using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Features.Countries.Contracts;
using DeliveryCostCalculator.Server.Features.Countries.Services;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DeliveryCostCalculator.Server.Features.Countries;


public static class GetCountries
{

    public class Query : IRequest<Result<List<CountryDto>>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CountryType CountryType { get; set; }
        public decimal CostCorrectionPercentage { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Result<List<CountryDto>>>
    {
        private readonly ICountriesService _countriesService;

        public Handler(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task<Result<List<CountryDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var countryResponse = _countriesService.GetCountriesAsync();

            if (countryResponse is null)
            {
                return Result.Failure<List<CountryDto>> (new Error(
                    "GetCountryResponse.Null",
                    "The Country Response was not found"));
            }

            return await countryResponse;
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
