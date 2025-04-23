# Assignment.AzureWeather
A solution that includes a couple independent services: a service with Azure Functions (_Assignment.AzureWeather.Function_) and a web API (_Assignment.AzureWeather.Api_). Additionally, the frontend app is located in UI/weather_app folder.

**Notes:**
* Default backend URL: https://localhost:7184 (can be changed)
* Default frontend URL: http://localhost:3000 (can be changed)
* Backend uses the persistence layer based on MSSQL and EF core, so an initial migration is required before using the service
* Both services should be configured before first use
* By default, the available API (`api/weather`) returns all existing weather entries. However, it is possible to limit the resulting set by using `from` and `to` query parameters and specifying initial values (the suggested date format is 'YYYY-mm-DD')

An example of test configuration for _Assignment.AzureWeather.Function_:

    "IsEncrypted": false,
        "Values": {
            "AzureWebJobsStorage": "UseDevelopmentStorage=true",
            "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
            "WeatherService:Units": "metric",
            "WeatherService:ApiKey": "api_key",
            "WeatherService:ServiceUrl": "https://api.openweathermap.org",
            "WeatherDataCityName": "Melbourne"
        }
        

An example of test configuration for _Assignment.AzureWeather.Api_:

    {
      "ConnectionStrings": {
        "WeatherDb": "<connection-string>"
      },
      "FrontUrl": "http://localhost:3000",
      "WeatherService": {
        "Units": "metric",
        "ApiKey": "api_key",
        "ServiceUrl": "https://api.openweathermap.org"
      },
      "WeatherWorker": {
        "Locations": [
          "Stockholm, SE",
          "Gothenburg, SE",
          "Oslo, NO",
          "Bergen, NO",
          "Copenhagen, DK",
          "Frederikssund, DK"
        ],
        "IntervalSeconds": 60
      }
    }