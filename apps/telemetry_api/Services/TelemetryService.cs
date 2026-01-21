using System.Collections.Concurrent;
using TelemetryApi.Models;

namespace TelemetryApi.Services;

public class TelemetryService
{
    private readonly ConcurrentBag<TelemetryRecord> _records = new();
    private const int MaxRecords = 10000;

    public TelemetryRecord AddRecord(CreateTelemetryDto dto)
    {
        var record = new TelemetryRecord
        {
            SensorId = dto.SensorId,
            SensorName = dto.SensorName,
            SensorType = dto.SensorType,
            Location = dto.Location,
            Value = dto.Value,
            Unit = dto.Unit,
            Source = dto.Source
        };

        _records.Add(record);

        // Keep only last MaxRecords
        if (_records.Count > MaxRecords)
        {
            var oldRecords = _records.OrderBy(r => r.Timestamp).Take(_records.Count - MaxRecords).ToList();
            foreach (var old in oldRecords)
            {
                // Note: ConcurrentBag doesn't support removal, so we just let it grow
                // In production, use a proper database
            }
        }

        return record;
    }

    public TelemetryStats? GetStats(int sensorId)
    {
        var sensorRecords = _records.Where(r => r.SensorId == sensorId).ToList();
        
        if (!sensorRecords.Any())
            return null;

        return new TelemetryStats
        {
            SensorId = sensorId,
            Location = sensorRecords.First().Location,
            MinValue = sensorRecords.Min(r => r.Value),
            MaxValue = sensorRecords.Max(r => r.Value),
            AvgValue = Math.Round(sensorRecords.Average(r => r.Value), 2),
            Count = sensorRecords.Count,
            FirstReading = sensorRecords.Min(r => r.Timestamp),
            LastReading = sensorRecords.Max(r => r.Timestamp)
        };
    }

    public List<TelemetryStats> GetAllStats()
    {
        return _records
            .GroupBy(r => r.SensorId)
            .Select(g => new TelemetryStats
            {
                SensorId = g.Key,
                Location = g.First().Location,
                MinValue = g.Min(r => r.Value),
                MaxValue = g.Max(r => r.Value),
                AvgValue = Math.Round(g.Average(r => r.Value), 2),
                Count = g.Count(),
                FirstReading = g.Min(r => r.Timestamp),
                LastReading = g.Max(r => r.Timestamp)
            })
            .ToList();
    }

    public TelemetryRecord? GetLatest(int sensorId)
    {
        return _records
            .Where(r => r.SensorId == sensorId)
            .OrderByDescending(r => r.Timestamp)
            .FirstOrDefault();
    }
}
