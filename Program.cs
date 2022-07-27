using NAudio.Wave;
using ZundaChan.BouyomiServer;
using ZundaChan;

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

var client = new ZundaChan.Voicevox.Client();
Console.WriteLine($"AudioDevices:");
for (int n = -1; n < WaveOut.DeviceCount; n++)
{
    var caps = WaveOut.GetCapabilities(n);
    Console.WriteLine($"{n}:{(Config.DeviceNumber == n ? "*" : " ")}{caps.ProductName}");
}
Console.WriteLine($"Speakers:");
var speakers = await client.GetSpeakersAsync();
foreach (var speaker in speakers.SelectMany((speaker) => speaker.styles.Select(style => new { Name = $"{speaker.name}({style.name})", Id = (int)style.id })))
{
    Console.WriteLine($"{speaker.Id}:{(Config.SpeakerId == speaker.Id ? "*" : " ")}{speaker.Name}");
}

var proxy = new Proxy(client);
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