using WebApıBegining.Models;

namespace WebApıBegining.Interfaces
{
    public interface ICountryRepository
    {
       ICollection<Country> GetCountries();
        Country GetCountry(int id);
        Country GetCountryByOwner(int ownerId);

        ICollection<Owner> GetOwnersFromACountry(int countryId);

        bool CountryExists(int id);
        bool CreateCountry(Country country);
        bool DeleteCountry(Country country);
        bool UpdateCountry(Country country);
        bool Save();
    }
}
