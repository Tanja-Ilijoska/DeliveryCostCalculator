﻿using Carter;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Deliveries.Contracts;
using DeliveryCostCalculator.Server.Features.Deliveries.Services;
using DeliveryCostCalculator.Server.Shared;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace DeliveryCostCalculator.Server.Features.Deliveries;


public static class UpdateDelivery
{
    public class Command : IRequest<Result<DeliveryDto>>
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

    internal sealed class Handler : IRequestHandler<Command, Result<DeliveryDto>>
    {
        private readonly IDeliveriesService _deliveriesService;
        private readonly IValidator<Command> _validator;

        public Handler(IDeliveriesService deliveriesService, IValidator<Command> validator)
        {
            _deliveriesService = deliveriesService;
            _validator = validator;
        }


        public async Task<Result<DeliveryDto>> Handle(Command request, CancellationToken cancellationToken)
        {
          
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure<DeliveryDto>(new Error(
                    "CreateDelivery.Validation",
                    validationResult.ToString()));
            }

            var deliveryResponse = await _deliveriesService.UpdateDelivery(request);

            return  deliveryResponse;
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
