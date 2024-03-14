using CalculatorService.DTO_s;
using CalculatorService.Entities;
using CalculatorService.Enums;
using CalculatorService.Services.interfaces;

namespace CalculatorService.Services
{
    public class ResultService : IResultService
    {
        private List<Result> results = new List<Result>();


        public void HandleCalculationResult(CalculationResponseDTO e)
        {
            var str = e.CalculationType == CalculationType.Addition ? " + " : " - ";
            var calculation = "" + e.NumberOne + str + e.NumberTwo;
            var result = new Result(null, e.CalculationResult, e.CalculationType, calculation, e.NumberOne, e.NumberTwo);

            results.Add(result);

        }

        public Result GetResult(double numberOne, double numberTwo, CalculationType calculationType)
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
            else
            {
                return null;
            }
        }
    }
}
