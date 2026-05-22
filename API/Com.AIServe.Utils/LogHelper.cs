namespace Com.AIServe.Utils;

public static class LogHelper
{
    private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");

    static LogHelper()
    {
        if (!Directory.Exists(LogPath))
        {
            Directory.CreateDirectory(LogPath);
        }
    }

    public static void Info(string message) => WriteLog("INFO", message);
    public static void Error(string message, Exception? ex = null) => WriteLog("ERROR", $"{message} {ex?.ToString()}");
    public static void Warn(string message) => WriteLog("WARN", message);

    private static void WriteLog(string level, string message)
    {
        try
        {
            var fileName = Path.Combine(LogPath, $"log_{DateTime.Now:yyyyMMdd}.txt");
            var logMsg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
            File.AppendAllText(fileName, logMsg);
        }
        catch
        {
            // ignore logging errors
        }
    }
}
