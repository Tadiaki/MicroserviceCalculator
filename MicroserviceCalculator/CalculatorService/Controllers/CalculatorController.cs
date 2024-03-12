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

namespace CalculatorService.Controllers
{
    public class CalculatorController : Controller
    {
        private readonly Calculator _cs;
        private ResultService _resultService;

        public CalculatorController(Calculator cs, ResultService resultService)
        {
            _cs = cs;
            _resultService = resultService;
        }

        // POST: CalculatorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCalculationAsync(CalculationRequestDTO calcReqDTO)
        {
            try
            {
                using var activity = Monitoring.ActivitySource.StartActivity();

                await _cs.SendCalculationRequestAsync(calcReqDTO);

                var result = _resultService.GetResult();
                

                return Ok(result);
            }
            catch (Exception ex)
            {
                Monitoring.Log.Here().Error(ex, "An error occurred while creating the calculation");
                Console.WriteLine(ex.ToString());
                return Content("An error occurred while creating the calculation. Please try again later.");
            }
        }

        // GET: CalculatorController/GetHistory
        [HttpGet]
        public ActionResult GetHistory() { 
            return View();
        }


    }
}
