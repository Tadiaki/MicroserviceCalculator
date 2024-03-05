using EasyNetQ;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using AdditionService.DTO_s;

namespace AdditionService.Communication
{
    public static class Messaging
    {
        public static async Task ListenForRequests()
        {
            var connectionEstablished = false;

            while (!connectionEstablished)
            {
                var bus = ConnectionHelper.GetRMQConnection();
                var subscriptionResult = bus.PubSub.SubscribeAsync<CalculationRequestDTO>("addition", e =>
                {

                    var propagator = new TraceContextPropagator();
                    var parentContext = propagator.Extract(default, e, (r, key) =>
                    {
                        return new List<string>(new[] { r.Headers.ContainsKey(key) ? r.Headers[key].ToString() : String.Empty }!);
                    });
                    Baggage.Current = parentContext.Baggage;

                    var response = new CalculationResponseDTO();
                    response.CalculationResult = e.NumberOne + e.NumberTwo;
                    response.CalculationType = e.CalculationType;

                    using (var activity = Monitoring.Monitoring.ActivitySource.StartActivity("Received task", ActivityKind.Consumer, parentContext.ActivityContext))
                    {
                        var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
                        var propagationContext = new PropagationContext(activityContext, Baggage.Current);
                        propagator.Inject(propagationContext, response.Headers, (headers, key, value) => headers.Add(key, value));

                        bus.PubSub.PublishAsync(response);
                    }
                }).AsTask();

                await subscriptionResult.WaitAsync(CancellationToken.None);
                connectionEstablished = subscriptionResult.Status == TaskStatus.RanToCompletion;
                if (!connectionEstablished) Thread.Sleep(1000);
            }
        }
    }
}
