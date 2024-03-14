using EasyNetQ;

namespace SubtractService.Communication
{
    public static class ConnectionHelper
    {
        public static IBus GetRMQConnection()
        {
            return RabbitHutch.CreateBus("host=rmq;username=guest;password=guest");
        }
    }
}
