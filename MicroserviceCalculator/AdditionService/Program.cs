using AdditionService.Communication;

public static class Program
{
    public static async Task Main()
    {

        await Messaging.ListenForRequests();

        while (true) {
            Thread.Sleep(5000);
        }
        
    }
}