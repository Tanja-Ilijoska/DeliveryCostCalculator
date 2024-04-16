using Carter;
using DeliveryCostCalculator.Server.Contracts;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Shared;
using FluentValidation;
using MediatR;
using System.Diagnostics.Contracts;

namespace DeliveryCostCalculator.Server.Features.DeliveryServices;

public static class CreateDeliveryService
{
    public class Command : IRequest<Result<int>>
    {
        public required string Name { get; set; }
        public string? Formula { get; set; }
        public ICollection<DeliveryServiceProperty>? DeliveryServiceProperties { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Name).NotNull();
            RuleFor(c => c.Formula).NotNull();
            RuleFor(c=>c.DeliveryServiceProperties).NotNull();
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
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<int>(new Error(
                    "CreateDeliveryService.Validation",
                    validationResult.ToString()));
            }

            var deliveryService = new DeliveryService()
            {
                Name = request.Name,
                Formula = request.Formula ?? string.Empty,
            }; 
            
            // = _mapper.Map<DeliveryService>(request);

            _dbContext.Add(deliveryService);
            await _dbContext.SaveChangesAsync(cancellationToken);

            foreach (var ds in request.DeliveryServiceProperties)
            {
                var dsProperty = new DeliveryServiceProperties()
                {
                    DeliveryServiceId = deliveryService.Id,
                    Name = ds.Name,
                    Operation = ds.Operation,
                    Value = ds.Value,
                    Order = ds.Order,
                };
                _dbContext.Add(dsProperty);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            return deliveryService.Id;
        }
    }    
}
public class CreateDeliveryServiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/deliveryServices", async (CreateDeliveryServiceRequest request, ISender sender) =>
        {
            var command = new DeliveryServices.CreateDeliveryService.Command
            {
                Name = request.Name,
                Formula = request.Formula,
                DeliveryServiceProperties = request.DeliveryServiceProperties,
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