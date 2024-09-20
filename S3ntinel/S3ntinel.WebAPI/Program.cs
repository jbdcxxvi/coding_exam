using S3ntinel.Application.Interfaces;
using S3ntinel.Application.Services;
using S3ntinel.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Configure Serilog for creating the log file
var outpath = "Alerts/monitoring_logs-.txt";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(  outpath,
                    rollingInterval: RollingInterval.Day,
                    shared: true)
    .CreateLogger();

builder.Host.UseSerilog();

//Add services to the container.
builder.Services.AddScoped<WindowsServerChecker>(); //for windows server checking
builder.Services.AddScoped<LinuxServerChecker>(); // for linux server checking
builder.Services.AddScoped<IServerCheckService, ServerCheckService>(); //the main monitoring class

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

app.Run();
