using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Countries.Services;
using DeliveryCostCalculator.Server.Features.DeliveryServices.Contracts;
using DeliveryCostCalculator.Server.Shared;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Features.DeliveryServices;


public static class UpdateDeliveryService
{
    public class Command : IRequest<Result<DeliveryServiceDto>>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Formula { get; set; } = string.Empty;
        public ICollection<DeliveryServicePropertyDto> DeliveryServiceProperties { get; set; } = [];
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Name).NotNull();
            RuleFor(c => c.Formula).NotNull();
            RuleFor(c => c.DeliveryServiceProperties).NotNull();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<DeliveryServiceDto>>
    {
        private readonly IDeliveryServiceService _deliveryServiceService;
        private readonly IValidator<Command> _validator;

        public Handler(IDeliveryServiceService deliveryServiceService, IValidator<Command> validator)
        {
            _deliveryServiceService = deliveryServiceService;
            _validator = validator;
        }



        public async Task<Result<DeliveryServiceDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<DeliveryServiceDto>(new Error(
                    "CreateDeliveryService.Validation",
                    validationResult.ToString()));
            }

            var deliveryResponse = await _deliveryServiceService.UpdateDeliveryService(request);

            return deliveryResponse;
        }
    }
}
public class UpdateDeliveryServiceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/deliveryService", async (UpdateDeliveryServiceRequest request, ISender sender) =>
        {
            var command = new DeliveryServices.UpdateDeliveryService.Command
            {
                Id = request.Id,
                Name = request.Name,
                Formula = request.Formula,
                DeliveryServiceProperties = request.DeliveryServiceProperties
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
