namespace APICatalog.Logging;

public class CustomerLogger : ILogger
{
    private readonly string _loggerName;
    private readonly CustomLoggerProviderConfiguration _loggerConfig;

    private const string CaminhoRepositorioLog =
        @"C:\Users\vinic\OneDrive\Área de Trabalho\ASP-.NET-Core-WEB-API\APICatalog\Logging\log\Vinicius_Log.txt";

    private const string CaminhoRepositorioException =
        @"C:\Users\vinic\OneDrive\Área de Trabalho\ASP-.NET-Core-WEB-API\APICatalog\Logging\log\Vinicius_Exception.txt";

    public CustomerLogger(string loggerName, CustomLoggerProviderConfiguration loggerConfig)
    {
        _loggerName = loggerName;
        _loggerConfig = loggerConfig;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == _loggerConfig.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?,
            string> formatter)
    {
        string mensagem = $"{logLevel.ToString()}: {eventId.Id} - {formatter(state, exception)}";
        EscreverTextoNoArquivo(mensagem);
    }

    private static void EscreverTextoNoArquivo(string mensagem)
    {
        using StreamWriter streamWriter = new(CaminhoRepositorioLog, append: true);
        try
        {
            streamWriter.WriteLine(mensagem);
            streamWriter.Close();
        }
        catch (Exception e)
        {
            using StreamWriter streamWriterException = new(CaminhoRepositorioException, append: true);
            streamWriterException.WriteLine(e.Message);
            streamWriterException.Close();
        }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}