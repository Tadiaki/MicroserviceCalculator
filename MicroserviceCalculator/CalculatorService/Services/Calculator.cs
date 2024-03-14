using CalculatorService.DTO_s;
using EasyNetQ;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using CalculatorService.Helpers;
using CalculatorService.Services.interfaces;



namespace CalculatorService.Services
{
    public class Calculator : ICalculator
    {

        public async Task SendCalculationRequestAsync(CalculationRequestDTO calcReqDTO)
        {
            using (var activity = Monitoring.ActivitySource.StartActivity())
            {
                Monitoring.Log.Here().Error("Entered calculation service");
                var bus = RabbitHutch.CreateBus("host=rmq;username=guest;password=guest");
                Monitoring.Log.Here().Error("Created bus");

                // pub
                var message = (calcReqDTO);

                var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
                var propagationContext = new PropagationContext(activityContext, Baggage.Current);
                var propagator = new TraceContextPropagator();
                propagator.Inject(propagationContext, message.Headers, (headers, key, value) => headers.Add(key, value));

                Monitoring.Log.Here().Error("Ready to send message");

                var topic = "";
                if (calcReqDTO.CalculationType == Enums.CalculationType.Addition)
                {
                    topic = "addition";
                }
                else
                {
                    topic = "subtraction";
                }
                Monitoring.Log.Here().Error("Sending message");
                await bus.PubSub.PublishAsync(message, typeof(CalculationRequestDTO), topic);
                Monitoring.Log.Here().Error("Message was send");
            }
        }


    }
}
