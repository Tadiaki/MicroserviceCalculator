using EasyNetQ;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using SharedModels;
using SubtractService.Monitoring;


namespace SubtractService.Communication
{
    public class Messaging: BackgroundService
    {
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionEstablished = false;

            while (!connectionEstablished)
            {
                try
                {
                    var bus = ConnectionHelper.GetRMQConnection();
                    
                    bus.PubSub.SubscribeAsync<CalculationRequestDTO>
                        ("AddService-" + Environment.MachineName, HandleSubtractionMessage, x => x.WithTopic("subtraction"));
                    

                    // Log that the subscription is set up
                    MonitoringService.Log.Here().Information("Subscription to subtraction topic is set up. Machine: " + Environment.MachineName + " is ready to consume.");

                    connectionEstablished = true; // Subscription successful
                }
                catch (Exception ex)
                {
                    MonitoringService.Log.Here().Error($"An error occurred while establishing subscription: {ex.Message}");
                    await Task.Delay(1000); // Wait before attempting to reconnect
                }
            }
        }
        private async void HandleSubtractionMessage(CalculationRequestDTO message)
        {
            try
            {
                MonitoringService.Log.Here()
                    .Information("Received request for subtraction of {NumberOne} and {NumberTwo}",
                        message.NumberOne, message.NumberTwo);

                var propagator = new TraceContextPropagator();
                var parentContext = propagator.Extract(default, message,
                    (r, key) =>
                    {
                        return new List<string>(new[]
                            { r.Headers.ContainsKey(key) ? r.Headers[key].ToString() : String.Empty }!);
                    });
                Baggage.Current = parentContext.Baggage;

                var response = new CalculationResponseDTO();
                response.CalculationResult = message.NumberOne - message.NumberTwo;
                response.CalculationType = message.CalculationType;
                response.NumberOne = message.NumberOne;
                response.NumberTwo = message.NumberTwo;
                MonitoringService.Log.Here()
                    .Information(
                        "calculated result for subtraction of {NumberOne} and {NumberTwo}: result {response}",
                        message.NumberOne, message.NumberTwo, response.CalculationResult);

                using (var activity = MonitoringService.ActivitySource.StartActivity("Received task",
                           ActivityKind.Consumer, parentContext.ActivityContext))
                {
                    var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
                    var propagationContext = new PropagationContext(activityContext, Baggage.Current);
                    propagator.Inject(propagationContext, response.Headers,
                        (headers, key, value) => headers.Add(key, value));
                    string topic = "subtractionResult";
                    MonitoringService.Log.Here()
                        .Information("publishing result to topic {topic} {response}", topic, response);
                    
                    var bus = ConnectionHelper.GetRMQConnection();
                    await bus.PubSub.PublishAsync(response, x => x.WithTopic(topic));
                    bus.Dispose();
                }
            }
            catch (Exception ex)
            {
                MonitoringService.Log.Here()
                    .Error($"An error occurred while processing subtraction request: {ex.Message}");
            }
        }
        

        
    }
}
