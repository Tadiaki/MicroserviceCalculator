using SharedModels;

namespace CalculatorService.Services.interfaces
{
    public interface ICalculator
    {
        Task SendCalculationRequestAsync(CalculationRequestDTO calcReqDTO);
    }
}
