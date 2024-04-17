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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace DeliveryCostCalculator.Server.Features.Countries.Services
{
    public class CalculationService : ICalculationService
    {
        private readonly DataContext _context;

        public CalculationService(DataContext context)
        {
            _context = context;
        }      

        public async Task<decimal> CalculateCost(GetDeliveryCost.Query request)
        {
            var cost = request.Weight * request.Distance;
            var formula = "Weight * Disance";

            var service = _context.DeliveryServices.Include(x=>x.DeliveryServiceProperties).FirstOrDefault(x => x.Id == request.DeliveryServiceId);
            if (service != null)
            {
                var properties = service.DeliveryServiceProperties.Select(c => new Tuple<int, string, decimal>(c.Order, c.Operation, c.Value))
                .ToList();
                var expression = BuildExpression(properties);
                cost = expression(cost);
            }

            var country = _context.Country.SingleOrDefault(x => x.Id == request.CountryId);
            if (country != null)
                cost *= (1m + country.CostCorrectionPercentage / 100m);

            return cost;
        }

        public Func<decimal, decimal> BuildExpression(List<Tuple<int, string, decimal>> operations)
        {
            operations.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            var param = Expression.Parameter(typeof(decimal), "x");
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
                        if (operation.Item3 != 0)
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

            return Expression.Lambda<Func<decimal, decimal>>(body, param).Compile();
        }
    }
}
