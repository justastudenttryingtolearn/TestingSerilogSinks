using Serilog;
using Serilog.Sinks.OpenTelemetry;
using System.Collections.Generic;
using System;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.Protocol = OtlpProtocol.HttpProtobuf;
    })
    .CreateLogger();

try
{
    Log.Information("blahblah");

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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
