using CalculatorService.DTO_s;
using CalculatorService.Entities;
using CalculatorService.Enums;

namespace CalculatorService.Services.interfaces
{
    public interface IResultService
    {
        void HandleCalculationResult(CalculationResponseDTO e);
        Result GetResult(double numberOne, double numberTwo, CalculationType calculationType);
    }
}
