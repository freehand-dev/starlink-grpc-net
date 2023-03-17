using dish_status_watcher.Services;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

IHost host = Host.CreateDefaultBuilder(args)
     .ConfigureAppConfiguration((context, config) =>
     {
         var switchMappings = new Dictionary<string, string>()
         {
             { "-i", "interval" },
             { "-l", "log" },
             { "-f", "fields" },
         };
         config.AddCommandLine(args, switchMappings);
     })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();

        var loggerConfiguration = new LoggerConfiguration();
        loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
        loggerConfiguration.MinimumLevel.Override(typeof(Worker).FullName!, LogEventLevel.Debug);
        loggerConfiguration.WriteTo.Async(wt => wt.Console(
            outputTemplate: "{Timestamp:yyyy-MM-ddTHH:mm:ss.fffK},{Message:lj}{NewLine}"
        ));

        var saveTo = context.Configuration["log"];
        if (!string.IsNullOrEmpty(saveTo))
        {
            loggerConfiguration.WriteTo.Async(wt => wt.File(saveTo,
                        rollingInterval: RollingInterval.Infinite,
                        outputTemplate: "{Timestamp:yyyy-MM-ddTHH:mm:ss.fffK},{Message:lj}{NewLine}"));
        }

        var logger  = loggerConfiguration.CreateLogger();
        logging.AddSerilog(logger);

    })
    .UseSystemd()
    .Build();

host.Run();
