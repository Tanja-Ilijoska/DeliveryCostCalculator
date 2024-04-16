using Carter;
using DeliveryCostCalculator.Server.Contracts;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Shared;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace DeliveryCostCalculator.Server.Features.Deliveries;


public static class UpdateDelivery
{
    public class Command : IRequest<Result<DeliveryResponse>>
    {
        public int Id { get; set; }
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

    internal sealed class Handler : IRequestHandler<Command, Result<DeliveryResponse>>
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


        public async Task<Result<DeliveryResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var command = new UpdateDelivery.Command
            {
                Id = request.Id,
                Weight = request.Weight,
                CountryId = request.CountryId,
                DeliveryServiceId = request.DeliveryServiceId,
                Cost = request.Cost,
                Recipient = request.Recipient,
                Distance = request.Distance
            };

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<DeliveryResponse>(new Error(
                    "CreateDelivery.Validation",
                    validationResult.ToString()));
            }

            var delivery = await _dbContext.Deliveries.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (delivery is null)
            {
                return Result.Failure<DeliveryResponse>(new Error(
                    "GetDeliveryResponse.Null",
                    "The Delivery with the specified ID was not found"));
            }
            else
            {
                delivery.Distance = request.Distance;
                delivery.Recipient = request.Recipient;
                delivery.Weight = request.Weight;
                delivery.CountryId = request.CountryId;
                delivery.DeliveryServiceId = request.DeliveryServiceId;
            }

            // = _mapper.Map<DeliveryService>(request);

            _dbContext.Update(delivery);
            await _dbContext.SaveChangesAsync(cancellationToken);

            DeliveryResponse deliveryResponse = new DeliveryResponse()
            {
                Id = delivery.Id,
                Distance = delivery.Distance,
                Recipient = delivery.Recipient,
                Weight = delivery.Weight,
                CountryId = delivery.CountryId,
                DeliveryServiceId = delivery.DeliveryServiceId,
                Cost = delivery.Cost
        };

            return deliveryResponse;
        }

    }
}
public class UpdateDeliveryEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/delivery", async (UpdateDeliveryRequest request, ISender sender) =>
        {
            var command = new Deliveries.UpdateDelivery.Command
            {
                Id = request.Id,
                Distance = request.Distance,
                Recipient = request.Recipient,
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
        }).RequireCors("AllowAll");
    }
}
