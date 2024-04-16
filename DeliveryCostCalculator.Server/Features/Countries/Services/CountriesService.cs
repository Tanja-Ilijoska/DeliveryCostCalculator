using Azure.Core;
using DeliveryCostCalculator.Server.Data;
using DeliveryCostCalculator.Server.Entities;
using DeliveryCostCalculator.Server.Features.Countries.Contracts;
using DeliveryCostCalculator.Server.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace DeliveryCostCalculator.Server.Features.Countries.Services
{
    public class CountriesService : ICountriesService
    {
        private readonly DataContext _context;

        public CountriesService(DataContext context)
        {
            _context = context;
        }

        public async Task<CountryDto?> GetCountry(int id)
        {
            var country = await _context.Country.Where(x => x.Id == id).Select(x => new CountryDto
            {
                Id = id,
                Name = x.Name,
                CountryType = x.CountryType,
                CostCorrectionPercentage = x.CostCorrectionPercentage,
            }).FirstOrDefaultAsync();

            return country;
        }

        public async Task<List<CountryDto>> GetCountriesAsync()
        {
            var countryResponse = await _context.Country.Select(x => new CountryDto
            {
                Id = x.Id,
                Name = x.Name,
                CostCorrectionPercentage = x.CostCorrectionPercentage,
                CountryType = x.CountryType,
            }).ToListAsync();

            return countryResponse;
        }

        public async Task<bool> DeleteCountry(int id)
        {
            var country = await _context.Country.Where(x => x.Id == id).FirstOrDefaultAsync();

            _context.Remove(country);
            _context.SaveChangesAsync().Wait();

            return true;
        }

        public int CreateCountry(CreateCountry.Command request)
        {
            var country = new Country()
            {
                Name = request.Name,
                CountryType = request.CountryType.ToString(),
                CostCorrectionPercentage = request.CostCorrectionPercentage
            };

            _context.Add(country);
            _context.SaveChangesAsync();

            return country.Id;
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
    }
}
