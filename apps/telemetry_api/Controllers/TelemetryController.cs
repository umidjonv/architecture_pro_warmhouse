using Microsoft.AspNetCore.Mvc;
using TelemetryApi.Models;
using TelemetryApi.Services;

namespace TelemetryApi.Controllers;

[ApiController]
[Route("telemetry")]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryService _telemetryService;

    public TelemetryController(TelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    [HttpPost]
    public IActionResult AddRecord([FromBody] CreateTelemetryDto dto)
    {
        var record = _telemetryService.AddRecord(dto);
        return Created($"/telemetry/{record.Id}", record);
    }

    [HttpGet]
    public IActionResult Query(
        [FromQuery] int? sensorId,
        [FromQuery] string? location,
        [FromQuery] string? sensorType,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int limit = 100)
    {
        var query = new TelemetryQuery
        {
            SensorId = sensorId,
            Location = location,
            SensorType = sensorType,
            From = from,
            To = to,
            Limit = limit
        };

        var results = _telemetryService.Query(query);
        return Ok(results);
    }
    

    [HttpGet("sensor/{sensorId}/latest")]
    public IActionResult GetLatest(int sensorId)
    {
        var record = _telemetryService.GetLatest(sensorId);
        if (record == null)
            return NotFound(new { message = $"No telemetry found for sensor {sensorId}" });
        return Ok(record);
    }

    [HttpGet("sensor/{sensorId}/stats")]
    public IActionResult GetSensorStats(int sensorId)
    {
        var stats = _telemetryService.GetStats(sensorId);
        if (stats == null)
            return NotFound(new { message = $"No telemetry found for sensor {sensorId}" });
        return Ok(stats);
    }

    [HttpGet("stats")]
    public IActionResult GetAllStats()
    {
        var stats = _telemetryService.GetAllStats();
        return Ok(stats);
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
