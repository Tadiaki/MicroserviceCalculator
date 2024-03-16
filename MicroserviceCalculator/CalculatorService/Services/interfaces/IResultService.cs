using CalculatorService.Entities;
using SharedModels;


namespace CalculatorService.Services.interfaces
{
    public interface IResultService
    {
        void HandleCalculationResult(CalculationResponseDTO e);
        Result GetResult(double numberOne, double numberTwo, CalculationType calculationType);
    }
}
