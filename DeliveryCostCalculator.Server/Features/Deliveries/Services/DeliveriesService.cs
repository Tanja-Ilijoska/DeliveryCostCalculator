using Azure.Core;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Countries.Contracts;
using DeliveryCostCalculator.Server.Features.Deliveries;
using DeliveryCostCalculator.Server.Features.Deliveries.Contracts;
using DeliveryCostCalculator.Server.Features.Deliveries.Services;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace DeliveryCostCalculator.Server.Features.Countries.Services
{
    public class DeliveriesService : IDeliveriesService
    {
        private readonly DataContext _context;

        public DeliveriesService(DataContext context)
        {
            _context = context;
        }

        public async Task<int> CreateDeliveryAsync(CreateDelivery.Command request)
        {
            var delivery = new Delivery()
            {
                Recipient = request.Recipient,
                Distance = request.Distance,
                Weight = request.Weight,
                CountryId = request.CountryId,
                DeliveryServiceId = request.DeliveryServiceId,
                Cost = request.Cost
            };

            _context.Add(delivery);
            await _context.SaveChangesAsync();

            return delivery.Id;
        }

        public async Task<bool> DeleteDelivery(int id)
        {
            var delivery = await _context.Deliveries.Where(x => x.Id == id).FirstOrDefaultAsync();

            _context.Remove(delivery);
            _context.SaveChangesAsync().Wait();

            return true;
        }

        public Task<List<DeliveryDto>> GetDeliveriesAsync()
        {
            var deliveryResponse = _context.Deliveries.Select(x => new DeliveryDto
            {
                Id = x.Id,
                Weight = x.Weight,
                CountryId = x.CountryId,
                DeliveryServiceId = x.DeliveryServiceId,
                Cost = x.Cost,
                Recipient = x.Recipient,
                Distance = x.Distance,
            }).ToListAsync();

            return deliveryResponse;
        }

        public async Task<DeliveryDto?> GetDelivery(int id)
        {
            var delivery = await _context.Deliveries.Where(x => x.Id == id).Select(x => new DeliveryDto
            {
                Id = x.Id,
                Recipient = x.Recipient,
                Cost = x.Cost,
                CountryId = x.CountryId,
                DeliveryServiceId = x.DeliveryServiceId,
                Distance = x.Distance,
                Weight = x.Weight
            }).FirstOrDefaultAsync();

            return delivery;
        }

        public async Task<DeliveryDto> UpdateDelivery(UpdateDelivery.Command request)
        {           
            var delivery = await _context.Deliveries.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

            if (delivery is null)
            {
                return null;
            }
            else
            {
                delivery.Distance = request.Distance;
                delivery.Recipient = request.Recipient;
                delivery.Weight = request.Weight;
                delivery.CountryId = request.CountryId;
                delivery.DeliveryServiceId = request.DeliveryServiceId;
            }

            _context.Update(delivery);
            await _context.SaveChangesAsync();

            DeliveryDto deliveryResponse = new DeliveryDto()
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

        public Func<double, double> BuildExpression(List<Tuple<int, string, double>> operations)
        {
            operations.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            var param = Expression.Parameter(typeof(double), "x");
            Expression body = param;

            foreach (var operation in operations)
            {
                switch (operation.Item2)
                {
                    case "+":
                        body = Expression.Add(body, Expression.Constant(operation.Item3));
                        break;
                    case "-":
                        body = Expression.Subtract(body, Expression.Constant(operation.Item3));
                        break;
                    case "*":
                        body = Expression.Multiply(body, Expression.Constant(operation.Item3));
                        break;
                    case "/":
                        if(operation.Item3 != 0)
                        {
                            body = Expression.Divide(body, Expression.Constant(operation.Item3));
                        }
                        else
                        {
                            throw new DivideByZeroException();
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            return Expression.Lambda<Func<double,double>>(body, param).Compile();
        }

        public Func<double, double> CalculateCost()
        {
            throw new NotImplementedException();
        }
    }
}
