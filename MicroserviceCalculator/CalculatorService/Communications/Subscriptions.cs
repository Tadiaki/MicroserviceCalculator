using CalculatorService.DTO_s;
using CalculatorService.Services;
using EasyNetQ;

namespace CalculatorService.Communications
{
    public static class Subscriptions
    {
        private static IBus? _bus;
        private static ResultService? _resultService;

        public static void StartSubtractionSubscription(ResultService resultService)
        {
            _resultService = resultService;
            _bus = RabbitHutch.CreateBus("host=rmq;username=guest;password=guest");

            var topic = "subtractionResult";

            var subscription = _bus.PubSub.SubscribeAsync<CalculationResponseDTO>(topic, async (e, cancellationToken) =>
            {
                if (e != null)
                {
                    _resultService.HandleCalculationResult(e);
                }
                else
                {
                    Console.WriteLine("Received null response from the message bus (subtractionResult).");
                }
            }, configure => { });
        }

        public static void StartAdditionSubscription(ResultService resultService)
        {
            _resultService = resultService;
            _bus = RabbitHutch.CreateBus("host=rmq;username=guest;password=guest");

            var topic = "additionResult";

            var subscription = _bus.PubSub.SubscribeAsync<CalculationResponseDTO>(topic, async (e, cancellationToken) =>
            {
                if (e != null)
                {
                    _resultService.HandleCalculationResult(e);
                }
                else
                {
                    Console.WriteLine("Received null response from the message bus (additionResult).");
                }
            }, configure => { });
        }
    }
}
