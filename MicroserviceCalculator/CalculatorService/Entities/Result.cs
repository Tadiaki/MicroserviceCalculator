
using SharedModels;

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

        public int? Id { get; set; }
        public double? Value { get; set; }
        public CalculationType Type { get; set; }
        public string Calculation { get; set; }
        public double? NumberOne { get; set; }
        public double? NumberTwo { get; set; }

    }
}
