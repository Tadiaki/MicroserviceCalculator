using CalculatorService.DTO_s;
using EasyNetQ;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using CalculatorService.Helpers;
using CalculatorService.Helpers.Monitoring;
using CalculatorService.Services.interfaces;
using EasyNetQ.Topology;


namespace CalculatorService.Services
{
    public class Calculator : ICalculator
    {

        public async Task SendCalculationRequestAsync(CalculationRequestDTO calcReqDto)
        {

            Monitoring.Log.Here().Information("Entered calculation service");
            var bus = RabbitHutch.CreateBus("host=rmq;port=5672;virtualHost=/;username=guest;password=guest");

            Monitoring.Log.Here().Information("Created bus");

            // pub
            var message = calcReqDto;

            Monitoring.Log.Here().Information("Ready to send message");

            var topic = "";
            topic = calcReqDto.CalculationType == Enums.CalculationType.Addition ? "addition" : "subtraction";

            Monitoring.Log.Here().Information("Sending message to topic: " + topic);

            await bus.PubSub.PublishAsync(message, x => x.WithTopic(topic));
            bus.Dispose();


            Monitoring.Log.Here().Information("Message was send");
        }
    }

}
