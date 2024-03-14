using SubtractService.Eums;

namespace SubtractService.DTO_s
{
    public class CalculationResponseDTO
    {
        public double CalculationResult { get; set; }
        public CalculationType CalculationType { get; set; }
        public Dictionary<string, object> Headers { get; } = new();
    }
}
