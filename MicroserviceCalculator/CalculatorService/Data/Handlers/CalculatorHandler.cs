using CalculatorService.Data.Contexts;
using CalculatorService.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CalculatorService.Data.Handlers
{
    public class CalculatorHandler
    {
        private List<Result> results = new List<Result>();
        internal async Task CreateCalculationAsync(Result result)
        {
            using (var context = new Context())
            {
                // Add the result to the Results DbSet
                context.Results.Add(result);

                try
                {
                    // Save changes to the database
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    // Handle exceptions, if any
                    Console.WriteLine($"Error occurred while saving calculation result: {ex.Message}");
                    throw; // Rethrow the exception
                }
            }
        }
    }
}
