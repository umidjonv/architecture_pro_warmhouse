using HomeApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure HTTP clients
var smartHomeApiUrl = builder.Configuration["SmartHomeApiUrl"] ?? "http://app:8080";
var temperatureApiUrl = builder.Configuration["TemperatureApiUrl"] ?? "http://temperature-api:8081";
var telemetryApiUrl = builder.Configuration["TelemetryApiUrl"] ?? "http://telemetry-api:8083";

builder.Services.AddHttpClient<SmartHomeClient>(client =>
{
    client.BaseAddress = new Uri(smartHomeApiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<TemperatureClient>(client =>
{
    client.BaseAddress = new Uri(temperatureApiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<TelemetryClient>(client =>
{
    client.BaseAddress = new Uri(telemetryApiUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddSingleton<HomeService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
