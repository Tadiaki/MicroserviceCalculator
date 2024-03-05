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

namespace CalculatorService.Controllers
{
    public class CalculatorController : Controller
    {
        private readonly Calculator _cs;

        // POST: CalculatorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CalculationRequestDTO calcReqDTO)
        {
            try
            {
                using var activity = Monitoring.ActivitySource.StartActivity();
                var response = _cs.CreateCalculation(calcReqDTO);



                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CalculatorController/GetHistory
        [HttpGet]
        public ActionResult GetHistory() { 
            return View();
        }


    }
}
