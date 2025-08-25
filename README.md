# TestingSerilogSinks

This project demonstrates an issue with logging complex objects using Serilog and the Serilog.Sinks.OpenTelemetry package, specifically when sending logs to Elastic via OpenTelemetry.

## Description

When logging complex objects using Serilog and sending them to Elastic via Serilog.Sinks.OpenTelemetry, the structure of the object is not preserved. 
Instead, the entire object is serialized into the message field as a string, rather than being mapped to individual fields in Elastic. 
Logging directly to Elastic using Serilog works as expected.

## Reproduction

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.Protocol = OtlpProtocol.HttpProtobuf;
    })
    .CreateLogger();

var quote = new Quote
{
    Id = 1,
    Author = "Albert Einstein",
    Text = "Life is like riding a bicycle. To keep your balance you must keep moving.",
    CreatedAt = DateTime.UtcNow,
    Tags = new List<string> { "life", "balance", "movement" },
    Metadata = new Dictionary<string, object>
    {
        { "source", "BrainyQuote" },
        { "rating", 5 },
        { "isFavorite", true }
    }
};

Log.Information("Logging a complex object: {@Quote}", quote);
```

Minimal repo for reproduction:  
https://github.com/justastudenttryingtolearn/TestingSerilogSinks/tree/main/TestingSerilogSinks

**Elastic Output (via OTEL Sink):**
```json
{
  "message": "Logging a complex object: {\"Id\":1,\"Author\":\"Albert Einstein\",\"Text\":\"Life is like riding a bicycle. To keep your balance you must keep moving.\",\"CreatedAt\":\"2025-08-22T11:40:51.2619620Z\",\"Tags\":[\"life\",\"balance\",\"movement\"],\"Metadata\":{\"source\":\"BrainyQuote\",\"rating\":5,\"isFavorite\":true},\"$type\":\"Quote\"}"
}
```

## Expected behavior

Each property in the log event should be mapped to an OpenTelemetry attribute, and then to an Elastic field with the same name. 
The value's structure (e.g., nested objects, arrays) should be maintained all the way to Elastic, so that each property is available as a separate field.

## Relevant package, tooling and runtime versions

- Serilog version: 4.3.1-dev-02373 
- Serilog.Sinks.OpenTelemetry version: 4.2.1-dev-02306
- .NET version: 8.0.408

## Additional context

Ideally, each property of the complex object should be available as a separate field in Elastic, preserving the structure. 
Please advise if this is a limitation of the OTEL sink or if there is a recommended workaround.

