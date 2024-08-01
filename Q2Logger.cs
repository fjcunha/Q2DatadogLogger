using Serilog.Sinks.Datadog.Logs;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Context;
using Datadog.Trace.Configuration;
using Datadog.Trace;

namespace Q2DatadogLogger
{
    public class Q2Logger
    {
        public static void Configure(string DATADOG_API_KEY, string DD_Service_Name, string hostUrlRegion, string DD_configuration_url, string[] tags)
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .Enrich.FromLogContext()
                            .Enrich.WithSpan()
                            .WriteTo.DatadogLogs(
                                apiKey: DATADOG_API_KEY,
                                source: "csharp",
                                service: DD_Service_Name,
                                host: hostUrlRegion,
                                tags: tags,
                                configuration: new DatadogConfiguration(url: DD_configuration_url))
                            .WriteTo.Console()
                            .CreateLogger();

            var settings = TracerSettings.FromDefaultSources();
            Tracer.Configure(settings);
        }

        public static void LogError(Exception ex, string message)
        {
            Log.Error(ex, message);
        }

        public static void LogInfo(string message)
        {
            Log.Information(message);
        }

        public static void LogWarning(Exception ex, string message)
        {
            Log.Warning(ex, message);
        }

        public static IDisposable PushLogContext(string propName, object? value)
        {
            return LogContext.PushProperty(propName, value);
        }

        public static IDisposable StartTraceScope(string traceScope)
        {
            var tracer = Tracer.Instance;
            return tracer.StartActive(traceScope);
        }
    }
}
