using CalculatorService.DTO_s;

namespace CalculatorService.Services.interfaces
{
    public interface ICalculator
    {
        Task SendCalculationRequestAsync(CalculationRequestDTO calcReqDTO);
    }
}
