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
        private static IBus? _bus;
        private static ResultService? _resultService;

        public static void StartSubtractionSubscription(ResultService resultService)
        {
            _resultService = resultService;
            _bus = RabbitHutch.CreateBus("host=rmq;port=5672;virtualHost=/;username=guest;password=guest");
                var topic = "subtractionResult";

                _bus.PubSub.SubscribeAsync<CalculationResponseDTO>("CalcService-" + Environment.MachineName, e =>
                {
                    var propagator = new TraceContextPropagator();
                    var parentContext = propagator.Extract(default, e, (r, key) =>
                    {
                        return new List<string>(new[] { r.Headers.ContainsKey(key) ? r.Headers[key].ToString() : String.Empty }!);
                    });
                    using var activity = Monitoring.ActivitySource.StartActivity("Received subtraction response from sub service.", ActivityKind.Consumer, parentContext.ActivityContext);
                    var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
                    var propagationContext = new PropagationContext(activityContext, Baggage.Current);
                    propagator.Inject(propagationContext, e.Headers, (headers, key, value) => headers.Add(key, value));
                    
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
            _bus = RabbitHutch.CreateBus("host=rmq;port=5672;virtualHost=/;username=guest;password=guest");


            var topic = "additionResult";

            _bus.PubSub.SubscribeAsync<CalculationResponseDTO>("CalcService-" + Environment.MachineName, e =>
            {
                var propagator = new TraceContextPropagator();
                var parentContext = propagator.Extract(default, e, (r, key) =>
                {
                    return new List<string>(new[] { r.Headers.ContainsKey(key) ? r.Headers[key].ToString() : String.Empty }!);
                });
                
                using var activity = Monitoring.ActivitySource.StartActivity("Received addition response from add service.", ActivityKind.Consumer, parentContext.ActivityContext);
                var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
                var propagationContext = new PropagationContext(activityContext, Baggage.Current);
                propagator.Inject(propagationContext, e.Headers, (headers, key, value) => headers.Add(key, value));
                
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
