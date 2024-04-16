using Azure.Core;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Countries.Contracts;
using DeliveryCostCalculator.Server.Features.DeliveryServices;
using DeliveryCostCalculator.Server.Features.DeliveryServices.Contracts;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace DeliveryCostCalculator.Server.Features.Countries.Services
{
    public class DeliveryServiceService : IDeliveryServiceService
    {
        private readonly DataContext _context;

        public DeliveryServiceService(DataContext context)
        {
            _context = context;
        }

        public async Task<CountryDto> UpdateCountry(UpdateCountry.Command request)
        {
            var country = await _context.Country.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

            if (country is null)
            {
                //return Result.Failure<CountryDto>(new Error(
                //    "GetCountryResponse.Null",
                //    "The Country with the specified ID was not found"));

                return null;
            }
            else
            {
                country.Name = request.Name;
                country.CostCorrectionPercentage = request.CostCorrectionPercentage;
                country.CountryType = request.CountryType.ToString();
            }


            _context.Update(country);
            await _context.SaveChangesAsync();

            CountryDto countryResponse = new CountryDto()
            {
                Id = country.Id,
                Name = country.Name,
                CountryType = country.CountryType,
                CostCorrectionPercentage = country.CostCorrectionPercentage
            };

            return countryResponse;
        }

        public async Task<DeliveryServiceDto?> GetDeliveryService(int id)
        {
           var deliveryService = await _context.DeliveryServices
                            .Include(x => x.DeliveryServiceProperties)
                            .Where(x => x.Id == id).Select(x => new DeliveryServiceDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                DeliveryServiceProperties = x.DeliveryServiceProperties.ToList()
                            }).FirstOrDefaultAsync();

            return deliveryService;
        }

        Task<List<DeliveryServiceDto>> IDeliveryServiceService.GetDeliveryServicesAsync()
        {
            var deliveryServiceResponse = _context.DeliveryServices.Include(x => x.DeliveryServiceProperties).Select(x => new DeliveryServiceDto
            {
                Id = x.Id,
                Name = x.Name,
                DeliveryServiceProperties = x.DeliveryServiceProperties.ToList()
            }).ToListAsync();

            return deliveryServiceResponse;
        }

        public async Task<bool> DeleteDeliveryService(int id)
        {
            var deliveryService = await _context.DeliveryServices.Where(x => x.Id == id).FirstOrDefaultAsync();

            _context.Remove(deliveryService);
            _context.SaveChangesAsync().Wait();

            return true;
        }

        public int CreateDeliveryService(CreateDeliveryService.Command request)
        {

            var deliveryService = new DeliveryService()
            {
                Name = request.Name,
                Formula = request.Formula ?? string.Empty,
            };

            _context.Add(deliveryService);
            _context.SaveChangesAsync();

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
                _context.Add(dsProperty);
            }

            _context.SaveChangesAsync();

            return deliveryService.Id;
        }

        public async Task<DeliveryServiceDto> UpdateDeliveryService(UpdateDeliveryService.Command request)
        {

            var delivery = _context.DeliveryServices.Where(x => x.Id == request.Id).FirstOrDefault();

            if (delivery is null)
            {
                //return Result.Failure<DeliveryServiceDto>(new Error(
                //    "GetDeliveryResponse.Null",
                //    "The Delivery with the specified ID was not found"));

                return null;
            }
            else
            {
                delivery.Formula = request.Formula;
                delivery.Name = request.Name;
                delivery.DeliveryServiceProperties.Clear();
                foreach (var property in request.DeliveryServiceProperties)
                {

                    delivery.DeliveryServiceProperties.Add(new DeliveryServiceProperties()
                    {
                        Id = property.Id,
                        Name = property.Name,
                        Operation = property.Operation,
                        Order = property.Order
                    });
                };
            }

            _context.Update(delivery);
            await _context.SaveChangesAsync();

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
