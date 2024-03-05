using CalculatorService.Enums;

namespace CalculatorService.Entities
{

    public class Result
    {
        public Result(double value, CalculationType type, string calculation)
        {
            Value = value;
            Type = type;
            Calculation = calculation;
        }

        public int Id { get; set; }
        public double Value { get; set; }
        public CalculationType Type { get; set; }
        public string Calculation { get; set; } = "";
    }
}
