using EasyNetQ;

namespace AdditionService.Communication
{
    public static class ConnectionHelper
    {
        public static IBus GetRMQConnection()
        {
            return RabbitHutch.CreateBus("host=localhost;username=guest;password=guest");
        }
    }
}
