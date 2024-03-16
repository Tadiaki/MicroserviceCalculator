using CalculatorService.Services;
using EasyNetQ;
using SharedModels;

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
                    if (e != null)
                    {
                        _resultService.HandleCalculationResult(e);
                    }
                    else
                    {
                        Console.WriteLine("Received null response from the message bus (subtractionResult).");
                    }
                }, x => x.WithTopic(topic));
            
        }

        public static void StartAdditionSubscription(ResultService resultService)
        {
            _resultService = resultService;
            _bus = RabbitHutch.CreateBus("host=rmq;port=5672;virtualHost=/;username=guest;password=guest");


            var topic = "additionResult";

            _bus.PubSub.SubscribeAsync<CalculationResponseDTO>("CalcService-" + Environment.MachineName, e =>
            {
                if (e != null)
                {
                    _resultService.HandleCalculationResult(e);
                }
                else
                {
                    Console.WriteLine("Received null response from the message bus (additionResult).");
                }
            }, x => x.WithTopic(topic));
        }
    }
}
