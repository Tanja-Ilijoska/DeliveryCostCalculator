using Carter;
using DeliveryCostCalculator.Server.Contracts;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Shared;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.Countries;


public static class UpdateCountry
{
    public class Command : IRequest<Result<CountryResponse>>
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

    internal sealed class Handler : IRequestHandler<Command, Result<CountryResponse>>
    {
        private readonly DataContext _dbContext;
        private readonly IValidator<Command> _validator;
        //   private readonly IMapper _mapper;

        public Handler(DataContext dbContext, IValidator<Command> validator)//, IMapper mapper)
        {
            _dbContext = dbContext;
            _validator = validator;
            //  _mapper = mapper;
        }


        public async Task<Result<CountryResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var command = new UpdateCountry.Command
            {
                Id = request.Id,
                Name = request.Name,
                CostCorrectionPercentage = request.CostCorrectionPercentage,
                CountryType = request.CountryType
            };

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<CountryResponse>(new Error(
                    "CreateCountry.Validation",
                    validationResult.ToString()));
            }

            var country = await _dbContext.Country.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (country is null)
            {
                return Result.Failure<CountryResponse>(new Error(
                    "GetCountryResponse.Null",
                    "The Country with the specified ID was not found"));
            }
            else
            {
                country.Name = command.Name;
                country.CostCorrectionPercentage = command.CostCorrectionPercentage;
                country.CountryType = command.CountryType.ToString();  
            }

            // = _mapper.Map<DeliveryService>(request);

            _dbContext.Update(country);
            await _dbContext.SaveChangesAsync(cancellationToken);

            CountryResponse countryResponse = new CountryResponse()
            {
                Id = country.Id,
                Name = country.Name,
                CountryType = country.CountryType,
                CostCorrectionPercentage = country.CostCorrectionPercentage
            };

            return countryResponse;
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
