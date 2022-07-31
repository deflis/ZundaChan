using ZundaChan.Core;
using ZundaChan.Core.BouyomiIpc;
using ZundaChan.Core.Aivoice;

{
    var config = new NLog.Config.LoggingConfiguration();

    // Targets where to log to: File and Console
    var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
    // Rules for mapping loggers to targets            
    config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logconsole);
    // Apply config           
    NLog.LogManager.Configuration = config;
}
var logger = NLog.LogManager.GetCurrentClassLogger();


var proxy = new AivoiceProxy();
try
{
    var ipcServer = new IpcServer(proxy);
    logger.Info("IPCサーバを起動しました");
}
catch (Exception ex)
{
    logger.Fatal("IPCサーバの起動に失敗しました", ex);
    return 1;
}

while (true)
{
    var key = Console.ReadKey();
    if (key.KeyChar == 'q' || key.Key == ConsoleKey.Escape)
    {
        return 0;
    }
    else if (key.KeyChar == 'r')
    {
        Config.Reload();
    }
}