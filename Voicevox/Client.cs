using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web;
using System.Media;
using NAudio.Wave;

namespace ZundaChan.Voicevox
{
    public class Client
    {
        public Client(string baseUrl, WaveOutEvent outputDevice)
        {
            this.client = new HttpClient();
            this.BaseUrl = baseUrl;
            this.outputDevice = outputDevice;
        }

        public string BaseUrl { get; }
        private HttpClient client { get; }
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly WaveOutEvent outputDevice;


        public async Task<Stream> CreateAsync(string text, int speakerId)
        {
            var query = await BuildAudioQueryJsonAsync(text, speakerId);
            return await SynthesisAsync(query, speakerId);
        }

        public async Task<string> BuildAudioQueryJsonAsync(string text, int speakerId)
        {
            var audioQueryUriBuilder = new UriBuilder(BaseUrl);
            audioQueryUriBuilder.Path = "/audio_query";
            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "text", text},
                    { "speaker", speakerId.ToString()},
                }))
            {
                audioQueryUriBuilder.Query = await content.ReadAsStringAsync();
            }
            Logger.Debug($"audio_quqery: {audioQueryUriBuilder}");
            using (var emptyContent = new StringContent("", Encoding.UTF8, "application/json"))
            using (var audioQueryResult = await client.PostAsync(audioQueryUriBuilder.Uri, emptyContent))
            {
                return await audioQueryResult.Content.ReadAsStringAsync();
            }
        }

        public async Task<Stream> SynthesisAsync(string audioQueryJson, int speakerId)
        {
            var synthesisUriBuilder = new UriBuilder(BaseUrl);
            synthesisUriBuilder.Path = "/synthesis";
            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "speaker", speakerId.ToString()},
            }))
            {
                synthesisUriBuilder.Query = await content.ReadAsStringAsync();
            }
            Logger.Debug($"synthesis: {synthesisUriBuilder}");

            using (var audioQueryContent = new StringContent(audioQueryJson, Encoding.UTF8, "application/json"))
            {
                var synthesisResult = await client.PostAsync(synthesisUriBuilder.Uri, audioQueryContent);
                Logger.Debug($"synthesis: {synthesisResult.StatusCode}");
                if (synthesisResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException($"{await synthesisResult.Content.ReadAsStringAsync()}");
                }
                return await synthesisResult.Content.ReadAsStreamAsync();
            }
        }
    }
}
