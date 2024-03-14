using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EasyNetQ;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
using System.Diagnostics;
using CalculatorService.DTO_s;
using CalculatorService.Services;
using CalculatorService.Enums;
using Serilog;
using CalculatorService.Helpers;
using CalculatorService.Entities;
using CalculatorService.Communications;
using CalculatorService.Data.Contexts;

namespace CalculatorService.Controllers
{
    public class CalculatorController : Controller
    {
        private readonly Calculator _cs;
        private readonly Context _context;
        private ResultService _resultService;

        public CalculatorController(Calculator cs, ResultService resultService, Context context)
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

                var timeoutTask = Task.Delay(5000); // 5 seconds timeout

                Result? result = null;

                while (!timeoutTask.IsCompleted && result == null)
                {
                    result = _resultService.GetResult(calcReqDTO.NumberOne, calcReqDTO.NumberTwo, calcReqDTO.CalculationType);
                }

                if (result != null)
                {
                    _context.Results.Add(result);
                    return Ok(result);
                }

                Monitoring.Log.Here().Error("An error occured while trying to GetResult");
                return BadRequest();
            }
            catch (Exception ex)
            {
                Monitoring.Log.Here().Error(ex, "An error occurred while creating the calculation");
                Console.WriteLine(ex.ToString());
                return Content("An error occurred while creating the calculation. Please try again later.");
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
                return StatusCode(500, "An error occurred while retrieving history.");
            }
        }

    }
}
