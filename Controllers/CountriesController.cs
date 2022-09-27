namespace rest_countries_client.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rest_countries_client.Services;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private CountriesService _countriesService;
    public CountriesController(CountriesService countriesService)
    {
        _countriesService = countriesService;
    }
    [HttpGet("all")]
    public async Task<ActionResult> GetAllCountries()
    {
        var countries = await _countriesService.GetAllCountries();
        return Ok(countries);
    }

    [HttpGet("all/csv")]
    public async Task<ActionResult> GetAllCountriesAsCSV()
    {
        var countries = await _countriesService.GetAllCountriesAsCSV();
        return Ok(countries);
    }

    [HttpGet("country/{name}")]
    public async Task<ActionResult> GetCountryInfo(string name)
    {
        var countryInfo = await _countriesService.GetCountryInfo(name);
        if (countryInfo == null)
            return NotFound("Sorry, the country you searched for was not found");

        return Ok(countryInfo);
    }

    [HttpGet("csv/{name}")]
    public async Task<ActionResult> GetCountriesAsCSV(string name)
    {
        var countryInfo = await _countriesService.GetCountryInfoAsCSV(name);
        if (countryInfo == null)
            return NotFound("Sorry, the country you searched for was not found");

        return Ok(countryInfo);
    }
}

