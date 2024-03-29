﻿using EasyNetQ;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using AdditionService.Monitoring;
using SharedModels;

namespace AdditionService.Communication
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
                        ("AddService-" + Environment.MachineName, HandleAdditionMessage, x => x.WithTopic("addition"));
                    

                    // Log that the subscription is set up
                    MonitoringService.Log.Here().Information("Subscription to addition topic is set up. Machine: " + Environment.MachineName + " is ready to consume.");

                    connectionEstablished = true; // Subscription successful
                }
                catch (Exception ex)
                {
                    MonitoringService.Log.Here().Error($"An error occurred while establishing subscription: {ex.Message}");
                    await Task.Delay(1000); // Wait before attempting to reconnect
                }
            }
        }
        
        
        private async void HandleAdditionMessage(CalculationRequestDTO message)
        {
             try
            {
                MonitoringService.Log.Here()
                    .Information("Received request for addition of {NumberOne} and {NumberTwo}",
                        message.NumberOne, message.NumberTwo);

                var propagator = new TraceContextPropagator();
                var parentContext = propagator.Extract(default, message, (r, key) =>
                {
                    return new List<string>(new[] { r.Headers.ContainsKey(key) ? r.Headers[key].ToString() : String.Empty }!);
                });
                Baggage.Current = parentContext.Baggage;

                using var activity = MonitoringService.ActivitySource.StartActivity("Received addition task",
                    ActivityKind.Consumer, parentContext.ActivityContext);
                    
                var response = new CalculationResponseDTO();
                response.CalculationResult = message.NumberOne + message.NumberTwo;
                response.CalculationType = message.CalculationType;
                response.NumberOne = message.NumberOne;
                response.NumberTwo = message.NumberTwo;
                MonitoringService.Log.Here()
                    .Information(
                        "calculated result for addition of {NumberOne} and {NumberTwo}: result {response}",
                        message.NumberOne, message.NumberTwo, response.CalculationResult);
                
                var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
                var propagationContext = new PropagationContext(activityContext, Baggage.Current);
                propagator.Inject(propagationContext, response.Headers, (headers, key, value) => headers.Add(key, value));
                string topic = "additionResult";
                var bus = ConnectionHelper.GetRMQConnection();
                MonitoringService.Log.Here()
                    .Information("publishing result to topic {topic} {response}", topic, response);
                    
                await bus.PubSub.PublishAsync(response, x => x.WithTopic(topic));
                bus.Dispose();
                
            }
            catch (Exception ex)
            {
                MonitoringService.Log.Here()
                    .Error($"An error occurred while processing addition request: {ex.Message}");
            }
        }
    }
}
