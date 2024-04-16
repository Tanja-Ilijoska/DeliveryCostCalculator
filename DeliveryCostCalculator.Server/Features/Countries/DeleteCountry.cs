using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Features.Countries.Services;
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
        private readonly ICountriesService _countriesService;

        public Handler(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        public async Task<Result<bool>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _countriesService.DeleteCountry(request.Id);
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
