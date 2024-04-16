using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Countries.Contracts;
using DeliveryCostCalculator.Server.Shared;
using FluentValidation;
using MediatR;

namespace DeliveryCostCalculator.Server.Features.Countries;

public static class CreateCountry
{
    public class Command : IRequest<Result<int>>
    {
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

    internal sealed class Handler : IRequestHandler<Command, Result<int>>
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


        public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            var command = new CreateCountry.Command
            {
                Name = request.Name,
                CostCorrectionPercentage = request.CostCorrectionPercentage,
                CountryType = request.CountryType
            };

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<int>(new Error(
                    "CreateCountry.Validation",
                    validationResult.ToString()));
            }

            var country = new Country()
            {
                Name = request.Name,
                CountryType = request.CountryType.ToString(),
                CostCorrectionPercentage = request.CostCorrectionPercentage
            };

            // = _mapper.Map<DeliveryService>(request);

            _dbContext.Add(country);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return country.Id;
        }
    }


}
public class CreateCountryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/country", async (CreateCountryRequest request, ISender sender) =>
        {
            var command = new Countries.CreateCountry.Command
            {
                Name = request.Name,
                CountryType = request.CountryType,
                CostCorrectionPercentage = request.CostCorrectionPercentage
            };

            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        })
            .RequireCors("AllowAll");
    }
}