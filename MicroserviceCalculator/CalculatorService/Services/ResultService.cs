using CalculatorService.Entities;
using CalculatorService.Services.interfaces;
using SharedModels;

namespace CalculatorService.Services
{
    public class ResultService : IResultService
    {
        private static List<Result> _results = new();


        public void HandleCalculationResult(CalculationResponseDTO e)
        {
            Console.WriteLine("I received a result");
            var str = e.CalculationType == CalculationType.Addition ? " + " : " - ";
            var calculation = "" + e.NumberOne + str + e.NumberTwo;
            var result = new Result(null, e.CalculationResult, e.CalculationType, calculation, e.NumberOne, e.NumberTwo);

            _results.Add(result);
            Console.WriteLine("Adding count : " +_results.Count);
            Console.WriteLine("Added Result To List");
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
