using CalculatorService.Communications;
using CalculatorService.Data.Contexts;
using CalculatorService.Services;
using CalculatorService.Services.interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<ICalculator, Calculator>();
builder.Services.AddScoped<IResultService, ResultService>();
builder.Services.AddDbContext<Context>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetService<Context>();
if (context != null)
{
    context.Database.EnsureCreated();
}


app.UseCors(options =>
{
    options.SetIsOriginAllowed(origin => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Create an instance of ResultService
var resultService = new ResultService();

// Start subtraction subscription
Subscriptions.StartSubtractionSubscription(resultService);

// Start addition subscription
Subscriptions.StartAdditionSubscription(resultService);


app.Run();
