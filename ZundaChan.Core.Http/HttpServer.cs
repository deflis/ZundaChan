using Microsoft.AspNetCore.Builder;
using System.Text.Json;

namespace ZundaChan.Core.Http
{
    public class HttpServer
    {
        public HttpServer(IProxy proxy, int port)
        {
            this.Proxy = proxy;
            var app = WebApplication.Create();
            app.Urls.Add($"http://0.0.0.0:{port}");

            app.MapGet("/", () => "It works!");

            app.MapGet("/Talk", async (context) =>
            {
                if (!context.Request.Query.ContainsKey("text"))
                {
                    context.Response.StatusCode = 400;
                    return;
                }
                var text = context.Request.Query["text"];
                var taskId = proxy.AddTalkTask(text);
                context.Response.StatusCode = 200;
                var json = JsonSerializer.Serialize(new { taskId });
                if (!context.Request.Query.ContainsKey("callback"))
                {
                    var callback = context.Request.Query["callback"];
                    using (var stream = context.Response.BodyWriter.AsStream())
                    using (var writer = new StreamWriter(stream))
                    {
                        await writer.WriteAsync($"{callback}({json});");
                    }
                }
            });

            app.Run();
        }

        public IProxy Proxy { get; }
    }
}