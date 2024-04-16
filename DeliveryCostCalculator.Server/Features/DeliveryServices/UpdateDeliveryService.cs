using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
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
        private readonly DataContext _dbContext;
        private readonly IValidator<Command> _validator;
        //   private readonly IMapper _mapper;

        public Handler(DataContext dbContext, IValidator<Command> validator)//, IMapper mapper)
        {
            _dbContext = dbContext;
            _validator = validator;
            //  _mapper = mapper;
        }


        public async Task<Result<DeliveryServiceDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var command = new UpdateDeliveryService.Command
            {
                Id = request.Id,
                Name = request.Name,
                Formula = request.Formula,
                DeliveryServiceProperties = request.DeliveryServiceProperties,
            };

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<DeliveryServiceDto>(new Error(
                    "CreateDeliveryService.Validation",
                    validationResult.ToString()));
            }

            var delivery = await _dbContext.DeliveryServices.Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);

            if (delivery is null)
            {
                return Result.Failure<DeliveryServiceDto>(new Error(
                    "GetDeliveryResponse.Null",
                    "The Delivery with the specified ID was not found"));
            }
            else
            {
                delivery.Formula = request.Formula;
                delivery.Name = request.Name;
                delivery.DeliveryServiceProperties.Clear();
                foreach(var property in request.DeliveryServiceProperties) {

                    delivery.DeliveryServiceProperties.Add(new DeliveryServiceProperties()
                    {
                        Id = property.Id,
                        Name = property.Name,
                        Operation = property.Operation,
                        Order = property.Order
                    });
                };
            }

            // = _mapper.Map<DeliveryService>(request);

            _dbContext.Update(delivery);
            await _dbContext.SaveChangesAsync(cancellationToken);

            DeliveryServiceDto deliveryResponse = new DeliveryServiceDto()
            {
                Id = delivery.Id,
                Name = delivery.Name,
                Formula = delivery.Formula,
                
            };

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
