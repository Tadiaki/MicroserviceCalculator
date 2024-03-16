using Microsoft.AspNetCore.Mvc;
using CalculatorService.Helpers;
using CalculatorService.Entities;
using CalculatorService.Data.Contexts;
using CalculatorService.Helpers.Monitoring;
using CalculatorService.Services.interfaces;
using SharedModels;

namespace CalculatorService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : ControllerBase
    {
        private readonly ICalculator _cs;
        private readonly Context _context;
        private IResultService _resultService;

        public CalculatorController(ICalculator cs, IResultService resultService, Context context)
        {
            _cs = cs;
            _resultService = resultService;
            _context = context;
        }

        // POST: CalculatorController/Create
        [HttpPost("CreateCalculation")]
        public async Task<ActionResult> CreateCalculationAsync(CalculationRequestDTO calcReqDTO)
        {
            try
            {
                using var activity = Monitoring.ActivitySource.StartActivity();

                await _cs.SendCalculationRequestAsync(calcReqDTO);

                var timeoutTask = Task.Delay(20000); // 20 seconds timer
                

                Result? result = null;
                Monitoring.Log.Here().Information("Going into loop, looking for result");
                while (!timeoutTask.IsCompleted && result == null)
                {
                    result = _resultService.GetResult(calcReqDTO.NumberOne, calcReqDTO.NumberTwo,
                        calcReqDTO.CalculationType);
                    Thread.Sleep(1000);
                }

                if (result != null)
                {
                    _context.Results.Add(result);
                    await _context.SaveChangesAsync();
                    return Ok(result);
                }

                Monitoring.Log.Here().Error("An error occured while trying to GetResult");
                return BadRequest();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Here().Error(ex, "An error occurred while creating the calculation");
                Console.WriteLine(ex.ToString());
                return Content(ex.Message + ". An error occurred while creating the calculation. Please try again later.");
            }
        }

        // GET: CalculatorController/GetHistory
        [HttpGet("GetHistory")]
        public ActionResult GetHistory()
        {
            try
            {
                var results = _context.Results.ToList();
                return Ok(results ?? new List<Result>());
            }
            catch (Exception ex)
            {
                Monitoring.Log.Here().Error(ex, "An error occurred while retrieving the history the calculation");
                return StatusCode(500, ex.Message + ". An error occurred while retrieving history.");
            }
        }

    }
}