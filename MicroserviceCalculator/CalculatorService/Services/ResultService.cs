using System.Diagnostics;
using CalculatorService.Entities;
using CalculatorService.Helpers.Monitoring;
using CalculatorService.Services.interfaces;
using OpenTelemetry.Context.Propagation;
using SharedModels;

namespace CalculatorService.Services
{
    public class ResultService : IResultService
    {
        private static List<Result> _results = new();


        public void HandleCalculationResult(CalculationResponseDTO e)
        {
            var propagator = new TraceContextPropagator();
            var parentContext = propagator.Extract(default, e, (r, key) =>
            {
                return new List<string>(new[] { r.Headers.ContainsKey(key) ? r.Headers[key].ToString() : String.Empty }!);
            });
            using var activity = Monitoring.ActivitySource.StartActivity("Taking care of calculation result", ActivityKind.Consumer, parentContext.ActivityContext);
            
            var str = e.CalculationType == CalculationType.Addition ? " + " : " - ";
            var calculation = "" + e.NumberOne + str + e.NumberTwo;
            var result = new Result(null, e.CalculationResult, e.CalculationType, calculation, e.NumberOne, e.NumberTwo);

            _results.Add(result);
        }

        public Result GetResult(double numberOne, double numberTwo, CalculationType calculationType)
        {
            Console.WriteLine("Trying to get result");
            Console.WriteLine("Getting Count : "+_results.Count);

            var matchingResult = _results.FirstOrDefault(r =>
            r.NumberOne == numberOne &&
            r.NumberTwo == numberTwo &&
            r.Type == calculationType);

            if (matchingResult != null)
            {
                Console.WriteLine($"Found Result {matchingResult}");
                _results.Remove(matchingResult);
                return matchingResult;
            }
            Console.WriteLine("Did not find a result.");
            return null;
        }
    }
}
