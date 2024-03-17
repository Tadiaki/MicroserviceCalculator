using System.Diagnostics;
using EasyNetQ;
using CalculatorService.Helpers;
using CalculatorService.Helpers.Monitoring;
using CalculatorService.Services.interfaces;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using SharedModels;


namespace CalculatorService.Services
{
    public class Calculator : ICalculator
    {

        public async Task SendCalculationRequestAsync(CalculationRequestDTO calcReqDto)
        {
            using var activity = Monitoring.ActivitySource.StartActivity("Received create calculation REST call.");
            Monitoring.Log.Here().Information("Entered calculation service");
            var bus = RabbitHutch.CreateBus("host=rmq;port=5672;virtualHost=/;username=guest;password=guest");

            Monitoring.Log.Here().Information("Created bus");

            // pub
            var message = calcReqDto;
            
            Monitoring.Log.Here().Information("Ready to send message");

            var topic = "";
            topic = calcReqDto.CalculationType == CalculationType.Addition ? "addition" : "subtraction";

            Monitoring.Log.Here().Information("Sending message to topic: " + topic);

            await bus.PubSub.PublishAsync(message, x => x.WithTopic(topic));
            bus.Dispose();


            Monitoring.Log.Here().Information("Message was send");
        }
    }
}
