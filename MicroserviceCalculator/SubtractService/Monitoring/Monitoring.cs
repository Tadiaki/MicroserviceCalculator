using System.Diagnostics;
using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using ILogger = Serilog.ILogger;

namespace SubtractService.Monitoring
{
    public class MonitoringService
    {
        public static readonly ActivitySource ActivitySource = new("RPS", "1.0.0");
        private static TracerProvider _tracerProvider;
        public static ILogger Log => Serilog.Log.Logger;

        static MonitoringService()
        {
            // Configure tracing
            var serviceName = Assembly.GetExecutingAssembly().GetName().Name;
            var version = "1.0.0";

            _tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddZipkinExporter(options =>
                {
                    options.Endpoint = new Uri("http://zipkin:9411/api/v2/spans"); // Adjust the URL and port accordingly
                })
                .AddConsoleExporter()
                .AddSource(ActivitySource.Name)
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: serviceName, serviceVersion: version))
                .Build();

            // Configure logging
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithSpan()
                .WriteTo.Seq("http://seq:5341")
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
