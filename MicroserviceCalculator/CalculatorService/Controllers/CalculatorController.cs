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
using CalculatorService.Services.interfaces;

namespace CalculatorService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : Controller
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
            var res = new Result(0, CalculationType.Addition, "1+1=2", 1, 1);
            var contextresult = _context.Results.Add(res);
            await _context.SaveChangesAsync(); // Save changes to the database
            return Ok(contextresult);
            //try
            //{
            //    using var activity = Monitoring.ActivitySource.StartActivity();

            //    await _cs.SendCalculationRequestAsync(calcReqDTO);

            //    var timeoutTask = Task.Delay(5000); // 5 seconds timeout

            //    Result? result = null;

            //    while (!timeoutTask.IsCompleted && result == null)
            //    {
            //        result = _resultService.GetResult(calcReqDTO.NumberOne, calcReqDTO.NumberTwo, calcReqDTO.CalculationType);
            //    }

            //    if (result != null)
            //    {
            //        _context.Results.Add(result);
            //        return Ok(result);
            //    }

            //    Monitoring.Log.Here().Error("An error occured while trying to GetResult");
            //    return BadRequest();
            //}
            //catch (Exception ex)
            //{
            //    Monitoring.Log.Here().Error(ex, "An error occurred while creating the calculation");
            //    Console.WriteLine(ex.ToString());
            //    return Content("An error occurred while creating the calculation. Please try again later.");
            //}
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
                return Ok(ex.Message);
            }
        }

    }
}
