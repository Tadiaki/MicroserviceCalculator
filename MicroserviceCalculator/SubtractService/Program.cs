using EasyNetQ;
using SubtractService.Communication;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<Messaging>();

var app = builder.Build();

app.Run();