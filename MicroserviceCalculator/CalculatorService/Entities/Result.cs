using CalculatorService.Enums;

namespace CalculatorService.Entities
{

    public class Result
    {

        public int? Id { get; set; }
        public double? Value { get; set; }
        public CalculationType Type { get; set; }
        public string Calculation { get; set; } = "";
        public double? NumberOne { get; set; }
        public double? NumberTwo { get; set; }

        public Result(double value, CalculationType type, string calculation, double numberOne, double numberTwo)
        {
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
