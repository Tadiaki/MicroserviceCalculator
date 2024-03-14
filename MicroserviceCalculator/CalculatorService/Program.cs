using CalculatorService.Communications;
using CalculatorService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Create an instance of ResultService
var resultService = new ResultService();

// Start subtraction subscription
Subscriptions.StartSubtractionSubscription(resultService);

// Start addition subscription
Subscriptions.StartAdditionSubscription(resultService);


app.Run();
