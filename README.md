# rest-countries-client-API
## What is
It`s challenge API that consumes the restcountries api, returning the countries data in several formats(json, xml, xls and csv)

The Api contains separates routes for each data format return

## Available routes

1. host/api/countries/all - returns all the countries data in json format
2. host/api/countries/all/csv - returns all the countries data in cvs format
3. host/api/countries/all/xml - returns all the countries data in xml format
4. host/api/countries/all/xls - returns all the countries data in xls format
5. host/api/countries/country/{name} - returns one country data in json format according to the name of the country
6. host/api/countries/csv/{name} - returns one country data in csv format according to the name of the country
7. host/api/countries/xml/{name} - returns one country data in xml format according to the name of the country
8. host/api/countries/xls/{name} - returns one country data in xls format according to the name of the country

Note that the host is the api host(eg: https://localhost:7249) and {name} is the name of the country (eg: mozambique)

## How to run
Clone with git clone "https://github.com/denilson-dxt/rest-countries-client"

cd  rest-countries-client

dotnet build || dotnet run || dotnet watch run

You may test the api by using the browser or applications like insomnia

