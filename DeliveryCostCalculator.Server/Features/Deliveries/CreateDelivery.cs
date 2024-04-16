using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Deliveries.Contracts;
using DeliveryCostCalculator.Server.Features.Deliveries.Services;
using DeliveryCostCalculator.Server.Shared;
using FluentValidation;
using MediatR;

namespace DeliveryCostCalculator.Server.Features.Deliveries;

public static class CreateDelivery
{
    public class Command : IRequest<Result<int>>
    {
        public string Recipient { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public decimal Weight { get; set; }
        public int CountryId { get; set; }
        public int DeliveryServiceId { get; set; }
        public decimal Cost { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Recipient).NotNull();
            RuleFor(c => c.Distance).NotNull();
            RuleFor(c => c.Weight).NotNull();
            RuleFor(c => c.CountryId).NotNull();
            RuleFor(c => c.DeliveryServiceId).NotNull();
            RuleFor(c => c.Cost).NotNull();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Result<int>>
    {
        private readonly IDeliveriesService _deliveriesService;
        private readonly IValidator<Command> _validator;
        //   private readonly IMapper _mapper;

        public Handler(IDeliveriesService deliveriesService, IValidator<Command> validator)
        {
            _deliveriesService = deliveriesService;
            _validator = validator;
        }

        public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<int>(new Error(
                    "CreateDelivery.Validation",
                    validationResult.ToString()));
            }           
            var delivery = await _deliveriesService.CreateDeliveryAsync(request);

            return delivery;
        }
    }
}
public class CreateDeliveryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/delivery", async (CreateDeliveryRequest request, ISender sender) =>
        {
            var command = new Deliveries.CreateDelivery.Command
            {
               Recipient = request.Recipient,
               Distance = request.Distance,
               Weight = request.Weight,
               CountryId = request.CountryId,
               DeliveryServiceId = request.DeliveryServiceId,
               Cost = request.Cost
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
