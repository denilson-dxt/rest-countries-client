using Newtonsoft.Json;
namespace rest_countries_client.Models;

public class Country
{
    [JsonProperty("name/common")]
    public string? Name{get;set;}
    public string? NativeName{get;set;}
    [JsonProperty("region")]
    public string? Region{get;set;}
    public string? SubRegion{get;set;}
    public int Population{get;set;}
    public double Area{get;set;}
    public string? TimeZone{get;set;}
    public string? FlagUrl{get;set;}
    
}