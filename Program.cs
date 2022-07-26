using NAudio.Wave;
using Tomlyn;
using ZundaChan.BouyomiServer;
var config = new NLog.Config.LoggingConfiguration();

string appFilePath = System.Reflection.Assembly.GetEntryAssembly().Location;
var tomlFile = System.Text.RegularExpressions.Regex.Replace(
    appFilePath,
    "\\.exe|dll$",
    ".toml",
    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

string toml;
using (var tomlStream = new FileStream(tomlFile, FileMode.Open))
using (var tomlReader = new StreamReader(tomlStream))
{
    toml = tomlReader.ReadToEnd();
}
var configFile = Toml.ToModel(toml);

// Targets where to log to: File and Console
var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

// Rules for mapping loggers to targets            
config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logconsole);

// Apply config           
NLog.LogManager.Configuration = config;
for (int n = -1; n < WaveOut.DeviceCount; n++)
{
    var caps = WaveOut.GetCapabilities(n);
    Console.WriteLine($"{n}: {caps.ProductName}");
}

var client = new ZundaChan.Voicevox.Client("http://localhost:50021/");
var ipcServer = new IpcServer(client, (int)(long)configFile["device"], (int)(long)configFile["speaker"]);
Console.WriteLine("Hello World!");
while (Console.ReadKey().KeyChar != 'q') { }
