using CalculatorService.Enums;

namespace CalculatorService.Entities
{

    public class Result
    {
        public Result(int? id, double? value, CalculationType type, string calculation, double? numberOne, double? numberTwo)
        {
            Id = id;
            Value = value;
            Type = type;
            Calculation = calculation;
            NumberOne = numberOne;
            NumberTwo = numberTwo;
        }

        // Constructor with parameters that EF Core can bind
        public Result(double value, CalculationType type, string calculation)
        {
            Value = value;
            Type = type;
            Calculation = calculation;
        }

        public Result()
        {
        }


    }
}
