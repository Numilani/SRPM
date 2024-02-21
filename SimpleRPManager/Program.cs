using Microsoft.Extensions.Configuration;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

public class Program
{
    public static async Task Main()
    {
        // load in config file
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration config = builder.Build();

        // Create the default logger

        var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(config)
        .CreateLogger();

        Log.Logger = logger;

        logger.Error("Test!");

        await logger.DisposeAsync();

    }
}