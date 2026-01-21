using Microsoft.AspNetCore.Mvc;
using HomeApi.Models;
using HomeApi.Services;

namespace HomeApi.Controllers;

[ApiController]
[Route("home")]
public class HomeController : ControllerBase
{
    private readonly HomeService _homeService;

    public HomeController(HomeService homeService)
    {
        _homeService = homeService;
    }

    [HttpGet]
    public IActionResult GetAllHomes()
    {
        return Ok(_homeService.GetAllHomes());
    }

    [HttpGet("{id}")]
    public IActionResult GetHomeById(int id)
    {
        var home = _homeService.GetHomeById(id);
        if (home == null)
            return NotFound(new { message = $"Home with id {id} not found" });
        return Ok(home);
    }

    [HttpPost]
    public IActionResult CreateHome([FromBody] CreateHomeDto dto)
    {
        var home = _homeService.CreateHome(dto);
        return CreatedAtAction(nameof(GetHomeById), new { id = home.Id }, home);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteHome(int id)
    {
        if (!_homeService.DeleteHome(id))
            return NotFound(new { message = $"Home with id {id} not found" });
        return NoContent();
    }

    [HttpPost("{homeId}/sensors")]
    public async Task<IActionResult> AddSensor(int homeId, [FromBody] AddSensorDto dto)
    {
        var sensor = await _homeService.AddSensorToHomeAsync(homeId, dto);
        if (sensor == null)
            return NotFound(new { message = $"Home with id {homeId} not found or failed to create sensor" });
        return Created($"/home/{homeId}/sensors/{sensor.Id}", sensor);
    }

    [HttpDelete("{homeId}/sensors/{sensorId}")]
    public async Task<IActionResult> RemoveSensor(int homeId, int sensorId)
    {
        if (!await _homeService.RemoveSensorFromHomeAsync(homeId, sensorId))
            return NotFound(new { message = "Home or sensor not found" });
        return NoContent();
    }

    [HttpPatch("{homeId}/sensors/{sensorId}/value")]
    public async Task<IActionResult> UpdateSensorValue(int homeId, int sensorId)
    {
        var sensor = await _homeService.UpdateSensorValueAsync(homeId, sensorId);
        if (sensor == null)
            return NotFound(new { message = "Home or sensor not found" });
        return Ok(sensor);
    }

    [HttpPost("{homeId}/sync")]
    public async Task<IActionResult> SyncSensors(int homeId)
    {
        var home = await _homeService.SyncSensorsFromSmartHomeAsync(homeId);
        if (home == null)
            return NotFound(new { message = $"Home with id {homeId} not found" });
        return Ok(home);
    }

    [HttpGet("sensors/{sensorId}/telemetry")]
    public async Task<IActionResult> GetSensorTelemetry(int sensorId, [FromQuery] int limit = 100)
    {
        var telemetry = await _homeService.GetSensorTelemetryAsync(sensorId, limit);
        if (telemetry == null)
            return NotFound(new { message = $"No telemetry found for sensor {sensorId}" });
        return Ok(telemetry);
    }

    [HttpGet("sensors/{sensorId}/stats")]
    public async Task<IActionResult> GetSensorStats(int sensorId)
    {
        var stats = await _homeService.GetSensorStatsAsync(sensorId);
        if (stats == null)
            return NotFound(new { message = $"No stats found for sensor {sensorId}" });
        return Ok(stats);
    }

    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
