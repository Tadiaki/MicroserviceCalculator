using EasyNetQ;

namespace AdditionService.Communication
{
    public static class ConnectionHelper
    {
        public static IBus GetRMQConnection()
        {
            return RabbitHutch.CreateBus("host=rmq;port=5672;virtualHost=/;username=guest;password=guest");
        }
    }
}
