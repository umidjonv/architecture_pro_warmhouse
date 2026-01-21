using Microsoft.AspNetCore.Mvc;

namespace TemperatureApi.Controllers;

[ApiController]
public class TemperatureController : ControllerBase
{
    private static readonly Random _random = new();

    [HttpGet("temperature")]
    public IActionResult GetTemperature([FromQuery] string? location, [FromQuery] string? sensorId)
    {
        location ??= "";
        sensorId ??= "";

        if (string.IsNullOrEmpty(location))
        {
            location = sensorId switch
            {
                "1" => "Living Room",
                "2" => "Bedroom",
                "3" => "Kitchen",
                _ => "Unknown"
            };
        }

        if (string.IsNullOrEmpty(sensorId))
        {
            sensorId = location switch
            {
                "Living Room" => "1",
                "Bedroom" => "2",
                "Kitchen" => "3",
                _ => "0"
            };
        }

        var temperature = Math.Round(18.0 + _random.NextDouble() * 10.0, 1);

        return Ok(new
        {
            sensorId,
            location,
            temperature,
            unit = "°C",
            timestamp = DateTime.UtcNow
        });
    }
}
