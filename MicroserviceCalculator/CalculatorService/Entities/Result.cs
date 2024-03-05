using CalculatorService.Enums;

namespace CalculatorService.Entities
{
    public class Result
    {
        private int Id { get; set; }
        private double Value { get; set; }
        private CalculationType Type { get; set; }
        private string Calculation { get; set; }

    }
}
