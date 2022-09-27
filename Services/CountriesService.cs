using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
using ClosedXML.Excel;
using rest_countries_client.Models;

namespace rest_countries_client.Services;
public class CountriesService
{

    ///<summary>
    /// Consome a api de paises
    ///</summary>
    ///<returns>Lista de paises do tipo Country<returns>
    public async Task<List<Country>> GetAllCountries()
    {
        var url = "https://restcountries.com/v3.1/all";
        var client = new HttpClient();
        var res = await client.GetAsync(url);

        // Carregar o conteudo da api e converter em um documento JSON
        var countriesJsonContent = JsonDocument.Parse(await res.Content.ReadAsStringAsync()).RootElement.EnumerateArray();

        var countries = new List<Country>();
        foreach (var countryJsonContent in countriesJsonContent)
        {
            // Adicionar cada pais a lista dos paises
            countries.Add(_extractCountryInfoFromJsonContent(countryJsonContent));
        }
        return countries;
    }


    ///<returns>Retorna os paises em formato CSV<returns>
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

    ///<returns>Retorna os paises em formato XLS<returns>
    public async Task<Byte[]> GetAllCountriesAsXLS()
    {
        var countries = await GetAllCountries();

        return _convertCountriesToXLS(countries);

    }

    ///<returns>Retorna os paises em formato XML<returns>
    public async Task<string> GetAllCountriesAsXML()
    {
        var countries = await GetAllCountries();
        var xml = "";
        var serializer = new XmlSerializer(typeof(List<Country>));

        using (var stringWriter = new StringWriter())
        {
            using (XmlWriter writter = XmlWriter.Create(stringWriter))
            {
                serializer.Serialize(writter, countries);
                xml = stringWriter.ToString();
            }
        }
        return xml;
    }

    ///<param name="name">Nome do pais a ser procurado</param>
    ///<returns>Retorna o pais em formato JSON<returns>
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

    ///<param name="name">Nome do pais a ser procurado</param>
    ///<returns>Retorna o pais em formato CSV<returns>
    public async Task<string?> GetCountryInfoAsCSV(string name)
    {
        var csv = "name,nativeName,region,subregion,population,area,timezone,flagUrl\n";
        var country = await GetCountryInfo(name);
        if (country == null) return null;

        csv += _convertCountryInfoToCsv(country);
        return csv;
    }

    ///<param name="name">Nome do pais a ser procurado</param>
    ///<returns>Retorna o pais em formato XLS<returns>
    public async Task<Byte[]> GetCountryInfoAsXLS(string name)
    {
        var country = await GetCountryInfo(name);
        var countries = new List<Country>();
        countries.Add(country);


        return _convertCountriesToXLS(countries);
    }

    ///<param name="name">Nome do pais a ser procurado</param>
    ///<returns>Retorna o pais em formato XML<returns>
    public async Task<string?> GetCountryInfoAsXML(string name)
    {
        var country = await GetCountryInfo(name);
        var xml = _convertCountryInfoToXML(country);
        return xml;

        //return _convertCountryInfoToCsv(country);
    }

    ///<param name="countries">Lista de paises</param>
    ///<returns>Retorna os paises em formato Byte[]<returns>
    private Byte[] _convertCountriesToXLS(List<Country> countries)
    {
        using (var workbook = new XLWorkbook())
        {
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

            foreach (var country in countries)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = country.Name;
                worksheet.Cell(currentRow, 2).Value = country.NativeName;
                worksheet.Cell(currentRow, 3).Value = country.Region;
                worksheet.Cell(currentRow, 4).Value = country.SubRegion;
                worksheet.Cell(currentRow, 5).Value = country.Population;
                worksheet.Cell(currentRow, 6).Value = country.Area;
                worksheet.Cell(currentRow, 7).Value = country.TimeZone;
                worksheet.Cell(currentRow, 8).Value = country.FlagUrl;

            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return content;
            }

        }

    }

    ///<param name="country">Nome do pais a ser procurado</param>
    ///<returns>Retorna o pais em formato XML<returns>
    private string _convertCountryInfoToXML(Country country)
    {
        var serializer = new XmlSerializer(typeof(Country));
        var xml = "";
        using (var stringWriter = new StringWriter())
        {
            using (XmlWriter writer = XmlWriter.Create(stringWriter))
            {
                serializer.Serialize(writer, country);
                xml = stringWriter.ToString();
            }
        }
        return xml;
    }

    private string _convertCountryInfoToCsv(Country country)
    {
        var countryCSV = $"{country.Name},{country.NativeName},{country.Region},{country.SubRegion},{country.Population},{country.Area},{country.TimeZone},{country.FlagUrl}";

        return countryCSV;
    }

    ///<summary>
    /// Extrai os dados de um pais de um elemento json para um object do tipo Country
    ///</summary>
    ///<param name="jsonElement">Representa o elemento json com os dados de um pais</param>
    ///<returns>Objecto do tipo Country<returns>
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