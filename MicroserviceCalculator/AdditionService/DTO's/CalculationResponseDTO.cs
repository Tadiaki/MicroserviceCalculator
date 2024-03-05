using AdditionService.Eums;

namespace AdditionService.DTO_s
{
    public class CalculationResponseDTO
    {
        public double CalculationResult { get; set; }
        public CalculationType CalculationType { get; set; }
        public Dictionary<string, object> Headers { get; } = new();
    }
}
