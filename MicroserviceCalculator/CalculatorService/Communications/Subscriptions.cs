using System.Diagnostics;
using CalculatorService.Helpers;
using CalculatorService.Services;
using EasyNetQ;
using SharedModels;
using CalculatorService.Helpers.Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace CalculatorService.Communications
{
    public static class Subscriptions
    {
        private static ResultService? _resultService;

        public static void StartSubtractionSubscription(ResultService resultService)
        {
            
            _resultService = resultService;
            var bus = RabbitHutch.CreateBus("host=rmq;port=5672;virtualHost=/;username=guest;password=guest");
                var topic = "subtractionResult";
                

                bus.PubSub.SubscribeAsync<CalculationResponseDTO>("CalcService-" + Environment.MachineName, e =>
                {
                    var propagator = new TraceContextPropagator();
                    var parentContext = propagator.Extract(default, e, (r, key) =>
                    {
                        return new List<string>(new[] { r.Headers.ContainsKey(key) ? r.Headers[key].ToString() : String.Empty }!);
                    });
                    Baggage.Current = parentContext.Baggage;

                    using var activity = Monitoring.ActivitySource.StartActivity("Received addition task", ActivityKind.Consumer, parentContext.ActivityContext);
                    if (e != null)
                    {
                        _resultService.HandleCalculationResult(e);
                    }
                    else
                    {
                        Console.WriteLine("Received null response from the message bus (subtractionResult).");
                    }
                    
                }, x => x.WithTopic(topic));
                
                Monitoring.Log.Here()
                    .Information(
                        "Subscription set up in Calculation Service, ready to consume messages on route: {topic}", topic );
        }

        public static void StartAdditionSubscription(ResultService resultService)
        {
            _resultService = resultService;
            var bus = RabbitHutch.CreateBus("host=rmq;port=5672;virtualHost=/;username=guest;password=guest");


            var topic = "additionResult";

            bus.PubSub.SubscribeAsync<CalculationResponseDTO>("CalcService-" + Environment.MachineName, e =>
            {
                var propagator = new TraceContextPropagator();
                var parentContext = propagator.Extract(default, e, (r, key) =>
                {
                    return new List<string>(new[] { r.Headers.ContainsKey(key) ? r.Headers[key].ToString() : String.Empty }!);
                });
                Baggage.Current = parentContext.Baggage;

                using var activity = Monitoring.ActivitySource.StartActivity("Received addition task", ActivityKind.Consumer, parentContext.ActivityContext);
                if (e != null)
                {
                    _resultService.HandleCalculationResult(e);
                }
                else
                {
                    Console.WriteLine("Received null response from the message bus (additionResult).");
                }
                
            }, x => x.WithTopic(topic));
            
            Monitoring.Log.Here()
                .Information(
                    "Subscription set up in Calculation Service, ready to consume messages on route: {topic}", topic );
        }
    }
}
