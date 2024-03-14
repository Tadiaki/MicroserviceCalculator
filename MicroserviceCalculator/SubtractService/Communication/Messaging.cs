using EasyNetQ;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using SubtractService.Monitoring;
using SubtractService.DTO_s;

namespace SubtractService.Communication
{
    public static class Messaging
    {
        public static async Task ListenForRequests()
        {
            var connectionEstablished = false;

            while (!connectionEstablished)
            {
                try
                {
                    var bus = ConnectionHelper.GetRMQConnection();
                    MonitoringService.Log.Here().Information("Got RMQConnection");
                   bus.PubSub.SubscribeAsync<CalculationRequestDTO>("SubService-"+Environment.MachineName, (e) =>
                    {
                        try
                        {
                            MonitoringService.Log.Here().Information("Received request for subtraction of {NumberOne} and {NumberTwo}", e.NumberOne, e.NumberTwo);

                            var propagator = new TraceContextPropagator();
                            var parentContext = propagator.Extract(default, e, (r, key) =>
                            {
                                return new List<string>(new[] { r.Headers.ContainsKey(key) ? r.Headers[key].ToString() : String.Empty }!);
                            });
                            Baggage.Current = parentContext.Baggage;
                           
                            var response = new CalculationResponseDTO();
                            response.CalculationResult = e.NumberOne - e.NumberTwo;
                            response.CalculationType = e.CalculationType;
                            response.NumberOne = e.NumberOne;
                            response.NumberTwo = e.NumberTwo;
                            MonitoringService.Log.Here().Information("calculated result for subtraction of {NumberOne} and {NumberTwo}: result {response}", e.NumberOne, e.NumberTwo, response.CalculationResult);

                            using (var activity = MonitoringService.ActivitySource.StartActivity("Received task", ActivityKind.Consumer, parentContext.ActivityContext))
                            {
                                var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
                                var propagationContext = new PropagationContext(activityContext, Baggage.Current);
                                propagator.Inject(propagationContext, response.Headers, (headers, key, value) => headers.Add(key, value));
                                string topic = "subtractionResult";
                                MonitoringService.Log.Here().Information("publishing result to topic {topic} {response}", topic, response);
                                bus.PubSub.PublishAsync(response, x => x.WithTopic(topic));
                            }
                        }
                        catch (Exception ex)
                        {
                            MonitoringService.Log.Here().Error($"An error occurred while processing subtraction request: {ex.Message}");
                        }
                    }, x => x.WithTopic("subtraction"));

                    // Log that the subscription is set up
                    MonitoringService.Log.Here().Information("Subscription to 'subtraction' topic is set up.");
                    connectionEstablished = true; // Subscription successful
                }
                catch (Exception ex)
                {
                    MonitoringService.Log.Here().Error($"An error occurred while establishing subscription: {ex.Message}");
                    await Task.Delay(1000); // Wait before attempting to reconnect
                }
            }
            MonitoringService.Log.Here().Information("Connection Established. Subscription successful!");
        }

    }
}
