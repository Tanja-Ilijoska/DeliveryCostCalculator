using DeliveryCostCalculator.Server.Features.Countries.Contracts;

namespace DeliveryCostCalculator.Server.Features.Countries.Services
{
    public interface ICountriesService
    {
        public Task<CountryDto?> GetCountry(int id);

        public Task<List<CountryDto>> GetCountriesAsync();

        public Task<bool> DeleteCountry(int id);

        public int CreateCountry(CreateCountry.Command request);

        public Task<CountryDto> UpdateCountry(UpdateCountry.Command request);
    }
}
