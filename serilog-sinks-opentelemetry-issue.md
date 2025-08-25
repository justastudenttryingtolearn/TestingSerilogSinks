## Issue: Complex Object Structure Not Preserved When Using OpenTelemetry Sink to Elastic

**Summary:**  
In our project, we log directly to Elastic using Serilog. 
This allows us to write complex objects as log properties, and Serilog correctly parameterizes and serializes these objects in Elastic. 
However, when we use the Serilog Sink for OpenTelemetry (OTEL) and then send logs to Elastic, this behavior does not occur. 
The sink is supposed to map each property to an attribute, keeping the name and maintaining the value's structure, but we don't think this is happening.

**Expected Behavior:**  
- Each property in the log event should be mapped to an OpenTelemetry attribute.
- From there it should be mapped to an Elastic label/field with the same name.
- The value's structure (e.g., nested objects, arrays) should be maintained all the way to Elastic.

**Actual Behavior:**  
- Complex objects are not parameterized or serialized as expected when using the OTEL sink.
- The structure of the value is lost or flattened in Elastic to simple text.

**Steps to Reproduce:**  
1. Log a complex object (e.g., with nested properties) using Serilog.
2. Send logs directly to Elastic and observe correct parameterization.
3. Send logs via Serilog Sink for OpenTelemetry, then to Elastic, and observe loss of structure.

**Example:**
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = "https://otel.out.co.za";
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

**Elastic Output (via OTEL Sink):**
```json
{
  "message": "Logging a complex object: {\"Id\":1,\"Author\":\"Albert Einstein\",\"Text\":\"Life is like riding a bicycle. To keep your balance you must keep moving.\",\"CreatedAt\":\"2025-08-22T11:40:51.2619620Z\",\"Tags\":[\"life\",\"balance\",\"movement\"],\"Metadata\":{\"source\":\"BrainyQuote\",\"rating\":5,\"isFavorite\":true},\"$type\":\"Quote\"}"
}
```
- The entire object is serialized into the message field as a string, rather than being mapped to individual fields.

**Environment:**  
- Serilog version: 4.3.1-dev-02373 
- Serilog.Sinks.OpenTelemetry version: 4.2.1-dev-02306
- .NET version: 8.0.408

**Additional Information:**  
Ideally, each property of the complex object should be available as a separate field in Elastic, preserving the structure. Please advise if this is a limitation of the OTEL sink or if there is a recommended workaround.
