using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Countries.Services;
using DeliveryCostCalculator.Server.Features.Deliveries.Services;
using DeliveryCostCalculator.Server.Features.DeliveryServices.Contracts;
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
        public ICollection<DeliveryServicePropertyDto>? DeliveryServiceProperties { get; set; }
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
        private readonly IDeliveryServiceService _deliveryServiceService;
        private readonly IValidator<Command> _validator;

        public Handler(IDeliveryServiceService deliveryServiceService, IValidator<Command> validator)
        {
            _deliveryServiceService = deliveryServiceService;
            _validator = validator;
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

            var deliveryService =  _deliveryServiceService.CreateDeliveryService(request);

            return deliveryService;
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