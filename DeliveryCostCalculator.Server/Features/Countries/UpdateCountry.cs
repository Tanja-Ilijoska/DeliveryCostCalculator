using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Countries.Contracts;
using DeliveryCostCalculator.Server.Features.Countries.Services;
using DeliveryCostCalculator.Server.Shared;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.Countries;


public static class UpdateCountry
{
    public class Command : IRequest<Result<CountryDto>>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public CountryType CountryType { get; set; }
        public decimal CostCorrectionPercentage { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Name).NotNull();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<CountryDto>>
    {
        private readonly IValidator<Command> _validator; 
        private readonly ICountriesService _countriesService;

        public Handler(DataContext dbContext, IValidator<Command> validator, ICountriesService countriesService)
        {
            _validator = validator;
            _countriesService = countriesService;
        }


        public async Task<Result<CountryDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<CountryDto>(new Error(
                    "CreateCountry.Validation",
                    validationResult.ToString()));
            }

            var result = await _countriesService.UpdateCountry(request);

            return result;
        }
    }


}
public class UpdateCountryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/country", async (UpdateCountryRequest request, ISender sender) =>
        {
            var command = new Countries.UpdateCountry.Command
            {
                Id = request.Id,
                Name = request.Name,
                
            };

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        }).RequireCors("AllowAll");
    }
}
