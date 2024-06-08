using System.Collections.Concurrent;

namespace APICatalog.Logging;

public class CustomLoggerProvider : ILoggerProvider
{
    private readonly CustomLoggerProviderConfiguration _loggerConfig;
    private readonly ConcurrentDictionary<string, CustomerLogger> _loggers = new();

    public CustomLoggerProvider(CustomLoggerProviderConfiguration loggerConfig)
    {
        _loggerConfig = loggerConfig;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new CustomerLogger(name, _loggerConfig));
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}