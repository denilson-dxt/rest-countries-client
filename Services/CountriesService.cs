using System.Text.Json;
using rest_countries_client.Models;

namespace rest_countries_client.Services;
public class CountriesService
{
    public async Task<List<Country>> GetAllCountries()
    {
        var url = "https://restcountries.com/v3.1/all";
        var client = new HttpClient();
        var res = await client.GetAsync(url);
        var countriesJsonContent = JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement.EnumerateArray();
        var countries = new List<Country>();
        foreach (var countryJsonContent in countriesJsonContent)
        {
            countries.Add(_extractCountryInfoFromJsonContent(countryJsonContent));
        }
        return countries;
    }
    public async Task<string> GetAllCountriesAsCSV()
    {
        var countries = await GetAllCountries();
        var csv = "name,nativeName,region,subregion,population,area,timezone,flagUrl\n";
        foreach(var country in countries){
            csv += $"{_convertCountryInfoToCsv(country)}\n";
        }
        return csv;
    }
    public async Task<Country?> GetCountryInfo(string name)
    {
        var url = $"https://restcountries.com/v3.1/name/{name}";
        var client = new HttpClient();
        var res = await client.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            return null;

        var country = _extractCountryInfoFromJsonContent(JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement.EnumerateArray().First());
        return country;
    }

    public async Task<string?> GetCountryInfoAsCSV(string name)
    {
        var country = await GetCountryInfo(name);
        if (country == null) return null;

        return _convertCountryInfoToCsv(country);
    }

    private string _convertCountryInfoToCsv(Country country)
    {
        var countryCSV = "name,nativeName,region,subregion,population,area,timezone,flagUrl\n";
        countryCSV += $"{country.Name},{country.NativeName},{country.Region},{country.SubRegion},{country.Population},{country.Area},{country.TimeZone},{country.FlagUrl}";

        return countryCSV;
    }

    private Country _extractCountryInfoFromJsonContent(JsonElement jsonElement)
    {
        var country = new Country();
        var countryJsonContent = jsonElement;

        country.Name = countryJsonContent.GetProperty("name").GetProperty("common").GetString();
        Console.WriteLine(country.Name);
        try
        {
            country.NativeName = countryJsonContent.GetProperty("name").GetProperty("nativeName").EnumerateObject().First().Value.GetProperty("common").GetString();
        }
        catch { }

        country.Region = countryJsonContent.GetProperty("region").GetString();
        System.Console.WriteLine(country.Region);
        var subregion = new JsonElement();
        countryJsonContent.TryGetProperty("subregion", out subregion);
        try
        {
            country.SubRegion = subregion.GetString();

        }
        catch { }
        country.Population = countryJsonContent.GetProperty("population").GetInt32();
        System.Console.WriteLine(country.Population);

        country.Area = countryJsonContent.GetProperty("area").GetDouble();
        System.Console.WriteLine(country.Area);

        country.TimeZone = countryJsonContent.GetProperty("timezones").EnumerateArray().First().GetString();
        System.Console.WriteLine(country.TimeZone);

        country.FlagUrl = countryJsonContent.GetProperty("flags").GetProperty("png").GetString();
        System.Console.WriteLine(country.FlagUrl);

        return country;
    }
}