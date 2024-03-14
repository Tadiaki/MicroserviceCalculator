using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using Serilog.Enrichers.Span;
using ILogger = Serilog.ILogger;

namespace CalculatorService.Helpers
{
    public static class Monitoring
    {
        public static ILogger Log => Serilog.Log.Logger;
        public static readonly ActivitySource ActivitySource = new("RPS", "1.0.0");
        private static TracerProvider _tracerProvider;

        static Monitoring()
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
