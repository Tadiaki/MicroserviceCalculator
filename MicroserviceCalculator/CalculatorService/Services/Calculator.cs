using CalculatorService.DTO_s;
using CalculatorService.Enums;
using EasyNetQ;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using CalculatorService.Helpers;
using CalculatorService.Entities;
using CalculatorService.Data.Handlers;


namespace CalculatorService.Services
{
    public class Calculator
    {
        private readonly CalculatorHandler _calcHandler;

        internal CalculationResponseDTO CreateCalculation(CalculationRequestDTO calcReqDTO)
        {
            using (var activity = Monitoring.ActivitySource.StartActivity())
            {
                var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest");
                var message = (calcReqDTO);

                var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
                var propagationContext = new PropagationContext(activityContext, Baggage.Current);
                var propagator = new TraceContextPropagator();
                propagator.Inject(propagationContext, message.Headers, (headers, key, value) => headers.Add(key, value));

                // Create a cancellation token source for the timeout
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                var responseTask = Task.Run(async () =>
                {
                    try
                    {
                        // Start the RPC request
                        return await bus.Rpc.RequestAsync<CalculationRequestDTO, CalculationResponseDTO>(calcReqDTO);
                    }
                    catch (Exception ex)
                    {
                        Monitoring.Log.Here().Error("bus.Rpc.RequestAsync failed ");

                        Console.WriteLine($"Error occurred during RPC request: {ex.Message}");

                        return null; // Return null if there's an error
                    }
                });

                // Start the fallback mechanism
                var fallbackTask = Task.Run(async () =>
                {
                    // Wait for 5 seconds
                    await Task.Delay(5000);

                    // Check if response is received
                    if (!responseTask.IsCompleted)
                    {
                        Monitoring.Log.Here().Error("5 seconds has gone by - fallback reached.");

                        // If not, print fallback message
                        Console.WriteLine("This service is unavailable at this moment");

                        // Cancel the RPC request if it's still pending
                        cancellationTokenSource.Cancel();
                    }
                });

                // Wait for either the response or the fallback
                Task.WaitAny(responseTask, fallbackTask);

                // If the response task is still not completed, cancel it
                if (!responseTask.IsCompleted)
                {
                    // Cancel the RPC request
                    cancellationTokenSource.Cancel();
                    return null; // Return null if the response task is not completed
                }
                else
                {
                    // If the response task is completed, handle the response
                    var response = responseTask.Result;
                    if (response != null)
                    {
                        var str = response.CalculationType == CalculationType.Addition ? " + " : " - ";
                        var result = new Result(response.CalculationResult, response.CalculationType, calcReqDTO.NumberOne + str + calcReqDTO.NumberTwo);

                        _calcHandler.CreateCalculationAsync(result).ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                foreach (var ex in task.Exception.InnerExceptions)
                                {
                                    Monitoring.Log.Here().Error("bus.Rpc.RequestAsync failed ");

                                    Console.WriteLine($"Error occurred while creating calculation result: {ex.Message}");
                                    // Handle the exception appropriately
                                }
                            }
                        });

                        return response;

                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
