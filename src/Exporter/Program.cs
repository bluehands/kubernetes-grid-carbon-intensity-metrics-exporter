using System.Security.AccessControl;
using CarbonAwareComputing.ExecutionForecast;
using Prometheus;

namespace Exporter
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            var app = builder.Build();

            // Configure the HTTP request pipeline.


            app.UseAuthorization();
            app.UseMetricServer();
            app.MapControllers();

            Metrics.SuppressDefaultMetrics();
            var metric = Metrics.CreateGauge("grid_carbon_intensity", "Grid carbon intensity in g/kWh", "location");


            var computingLocation = app.Configuration.GetValue<string>("Configuration:ComputingLocation","de");
            var endpoint = app.Configuration.GetValue<string>("Configuration:ForecastDataEndpointTemplate", "https://carbonawarecomputing.blob.core.windows.net/forecasts/{0}.json");

            if (computingLocation is null)
            {
                Console.Error.WriteLine($"Setting 'Configuration:ComputingLocation' not set");
                return -1;
            }
            if (endpoint is null)
            {
                Console.Error.WriteLine($"Setting 'Configuration:ForecastDataEndpointTemplate' not set");
                return -1;
            }
            if (!ComputingLocations.TryParse(computingLocation!, out ComputingLocation? location))
            {
                Console.Error.WriteLine($"No supported computing location found for {computingLocation}");
                Console.Error.WriteLine($"See https://github.com/bluehands/Carbon-Aware-Computing");
                Console.Error.WriteLine($"To get the list of locations: https://forecast.carbon-aware-computing.com/locations");
                return -1;
            }
            var provider = new CarbonAwareDataProviderOpenData(endpoint);

            Metrics.DefaultRegistry.AddBeforeCollectCallback(async t =>
            {
                try
                {
                    var intensity = await provider.GetCarbonIntensity(location, DateTimeOffset.Now);
                    intensity.Switch(
                        i => metric.WithLabels(location.Name).Set(i.Value),
                        _ => metric.WithLabels(location.Name).Unpublish()
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    metric.WithLabels(location.Name).Unpublish();
                }
            });

            app.Run();
            return 0;
        }
    }
}
