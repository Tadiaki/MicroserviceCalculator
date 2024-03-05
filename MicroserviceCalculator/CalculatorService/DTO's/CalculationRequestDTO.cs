using CalculatorService.Enums;

namespace CalculatorService.DTO_s
{
    public class CalculationRequestDTO
    {
        private double NumberOne {  get; set; }
        private double NumberTwo { get; set; }
        private CalculationType CalculationType { get; set; }
    }
}
