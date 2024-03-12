using CalculatorService.DTO_s;
using CalculatorService.Entities;
using CalculatorService.Enums;

namespace CalculatorService.Services
{
    public class ResultService
    {
        private List<Result> results = new List<Result>();

        public void HandleCalculationResult(CalculationResponseDTO e)
        {
            var str = e.CalculationType == CalculationType.Addition ? " + " : " - ";
            var calculation = "" + e.NumberOne + str + e.NumberTwo;
            var result = new Result(e.CalculationResult, e.CalculationType, calculation);
            results.Add(result);
        }

        public Result GetResult () 
        {
            if (results.Any())
            {
                return results.First();
            }
            Thread.Sleep(5000);
            return GetResult(); 
        }
    }
}
