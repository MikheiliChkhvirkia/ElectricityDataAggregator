using ElectricityDataAggregator.API.Tools.Extensions;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Assembly assembly = Assembly.GetExecutingAssembly();

// Add services to the container.
var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddSerilog());
builder.ConfigureServices(assembly, loggerFactory);


var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureMiddlewares(assembly);

app.Run();
