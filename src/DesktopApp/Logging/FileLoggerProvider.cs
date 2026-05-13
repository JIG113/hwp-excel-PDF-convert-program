using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DocToPdf.DesktopApp.Logging;

public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logFilePath;

    public FileLoggerProvider()
    {
        var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DocToPdfConverter", "logs");
        Directory.CreateDirectory(baseDir);
        _logFilePath = Path.Combine(baseDir, $"app-{DateTime.Now:yyyyMMdd}.log");
    }

    public ILogger CreateLogger(string categoryName) => new FileLogger(_logFilePath, categoryName);
    public void Dispose() { }

    private sealed class FileLogger : ILogger
    {
        private readonly string _path;
        private readonly string _category;

        public FileLogger(string path, string category)
        {
            _path = path;
            _category = category;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var line = $"{DateTime.Now:O} [{logLevel}] {_category} {formatter(state, exception)} {exception}";
            File.AppendAllText(_path, line + Environment.NewLine);
        }
    }
}
