using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using ClosedXML.Excel;
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
        foreach (var country in countries)
        {
            csv += $"{_convertCountryInfoToCsv(country)}\n";
        }
        return csv;
    }
    public async Task<Byte[]> GetAllCountriesAsXLS()
    {
        var countries = await GetAllCountries();

        return _convertCountriesToXLS(countries);
        
    }
    public async Task<string> GetAllCountriesAsXML()
    {
        var countries = await GetAllCountries();
        var xml = "";
        var serializer = new XmlSerializer(typeof(List<Country>));

        using (var sww = new StringWriter())
        {
            using (XmlWriter writter = XmlWriter.Create(sww))
            {
                serializer.Serialize(writter, countries);
                xml = sww.ToString();
            }
        }
        return xml;
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
        var csv = "name,nativeName,region,subregion,population,area,timezone,flagUrl\n";
        var country = await GetCountryInfo(name);
        if (country == null) return null;

        csv += _convertCountryInfoToCsv(country);
        return csv;
    }
    public async Task<Byte[]> GetCountryInfoAsXLS(string name)
    {
        var country = await GetCountryInfo(name);
        var countries = new List<Country>();
        countries.Add(country);


        return _convertCountriesToXLS(countries);
    }
    public async Task<string?> GetCountryInfoAsXML(string name)
    {
        var country = await GetCountryInfo(name);
        var xml = _convertCountryInfoToXML(country);
        return xml;

        //return _convertCountryInfoToCsv(country);
    }
    
    public Byte[] _convertCountriesToXLS(List<Country> countries)
    {
        using (var workbook = new XLWorkbook()){
            var worksheet = workbook.Worksheets.Add("Countries");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Name";
            worksheet.Cell(currentRow, 2).Value = "NativeName";
            worksheet.Cell(currentRow, 3).Value = "Region";
            worksheet.Cell(currentRow, 4).Value = "SubRegion";
            worksheet.Cell(currentRow, 5).Value = "Population";
            worksheet.Cell(currentRow, 6).Value = "Area";
            worksheet.Cell(currentRow, 7).Value = "Timezone";
            worksheet.Cell(currentRow, 8).Value = "FlagUrl";

            foreach(var country in countries){
                currentRow ++;
                worksheet.Cell(currentRow, 1).Value = country.Name;
                worksheet.Cell(currentRow, 2).Value = country.NativeName;
                worksheet.Cell(currentRow, 3).Value = country.Region;
                worksheet.Cell(currentRow, 4).Value = country.SubRegion;
                worksheet.Cell(currentRow, 5).Value = country.Population;
                worksheet.Cell(currentRow, 6).Value = country.Area;
                worksheet.Cell(currentRow, 7).Value = country.TimeZone;
                worksheet.Cell(currentRow, 8).Value = country.FlagUrl;
                
            }

            using (var stream  = new MemoryStream()){
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return content;
            }

        }
        
    }

    private string _convertCountryInfoToXML(Country country)
    {
        var serializer = new XmlSerializer(typeof(Country));
        var xml = "";
        using (var sww = new StringWriter())
        {
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                serializer.Serialize(writer, country);
                xml = sww.ToString();
            }
        }
        return xml;
    }

    private string _convertCountryInfoToCsv(Country country)
    {
        var countryCSV = $"{country.Name},{country.NativeName},{country.Region},{country.SubRegion},{country.Population},{country.Area},{country.TimeZone},{country.FlagUrl}";

        return countryCSV;
    }

    private Country _extractCountryInfoFromJsonContent(JsonElement jsonElement)
    {
        var country = new Country();
        var countryJsonContent = jsonElement;

        country.Name = countryJsonContent.GetProperty("name").GetProperty("common").GetString();
        try
        {
            country.NativeName = countryJsonContent.GetProperty("name").GetProperty("nativeName").EnumerateObject().First().Value.GetProperty("common").GetString();
        }
        catch { }

        country.Region = countryJsonContent.GetProperty("region").GetString();
        var subregion = new JsonElement();
        countryJsonContent.TryGetProperty("subregion", out subregion);
        try
        {
            country.SubRegion = subregion.GetString();

        }
        catch { }
        country.Population = countryJsonContent.GetProperty("population").GetInt32();

        country.Area = countryJsonContent.GetProperty("area").GetDouble();

        country.TimeZone = countryJsonContent.GetProperty("timezones").EnumerateArray().First().GetString();

        country.FlagUrl = countryJsonContent.GetProperty("flags").GetProperty("png").GetString();

        return country;
    }
}