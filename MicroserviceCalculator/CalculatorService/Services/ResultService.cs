using CalculatorService.Entities;
using CalculatorService.Services.interfaces;
using SharedModels;

namespace CalculatorService.Services
{
    public class ResultService :IResultService
    {
        private readonly List<Result> _results = new();


        public void HandleCalculationResult(CalculationResponseDTO e)
        {
            Console.WriteLine("I received a result");
            var str = e.CalculationType == CalculationType.Addition ? " + " : " - ";
            var calculation = "" + e.NumberOne + str + e.NumberTwo;
            var result = new Result(null, e.CalculationResult, e.CalculationType, calculation, e.NumberOne, e.NumberTwo);

                _results.Add(result);
        }

        public Result GetResult(double numberOne, double numberTwo, CalculationType calculationType)
        {

                var matchingResult = _results.FirstOrDefault(r =>
            r.NumberOne == numberOne &&
            r.NumberTwo == numberTwo &&
            r.Type == calculationType);

            if (matchingResult != null)
            {
                _results.Remove(matchingResult);
                return matchingResult;
            }
            return null;
        }
    }
}
