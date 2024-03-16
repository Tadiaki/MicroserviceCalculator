using AdditionService.Communication;
using EasyNetQ;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<Messaging>();

var app = builder.Build();

app.Run();
