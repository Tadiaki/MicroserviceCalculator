using CalculatorService.DTO_s;
using CalculatorService.Entities;
using CalculatorService.Enums;

namespace CalculatorService.Services
{
    public class ResultService
    {
        private List<Result> results = new List<Result>();
        private static readonly object resultsLock = new object(); // Lock object for thread safety (is it really needed that it is static?)

        public void HandleCalculationResult(CalculationResponseDTO e)
        {
            var str = e.CalculationType == CalculationType.Addition ? " + " : " - ";
            var calculation = "" + e.NumberOne + str + e.NumberTwo;
            var result = new Result(e.CalculationResult, e.CalculationType, calculation, e.NumberOne, e.NumberTwo);
            lock (resultsLock)
            {
                results.Add(result);
            }
        }

        public Result GetResult(double numberOne, double numberTwo, CalculationType calculationType)
        {
            lock (resultsLock)
            {
                var matchingResult = results.FirstOrDefault(r =>
            r.NumberOne == numberOne &&
            r.NumberTwo == numberTwo &&
            r.Type == calculationType);

                if (matchingResult != null)
                {
                    results.Remove(matchingResult);
                    return matchingResult;
                }
            }
            Thread.Sleep(1000);
            return GetResult(numberOne, numberTwo, calculationType);
        }
    }
}
