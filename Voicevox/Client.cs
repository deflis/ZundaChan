using System.Text;
using System.Text.Json;

namespace ZundaChan.Voicevox
{
    /// <summary>
    /// VOICEVOX ENGINEクライアント
    /// </summary>
    internal class Client
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private HttpClient client { get; }

        public Client()
        {
            client = new HttpClient();
        }

        /// <summary>
        /// AudioQuery + Synthesis を自動で行います
        /// </summary>
        /// <param name="text"></param>
        /// <param name="speakerId"></param>
        /// <returns></returns>
        public async Task<Stream> CreateAsync(string text, int speakerId)
        {
            var query = await BuildAudioQueryJsonAsync(text, speakerId);
            return await SynthesisAsync(query, speakerId);
        }

        public async Task<Speaker[]> GetSpeakersAsync()
        {
            var speakersUriBuilder = new UriBuilder(Config.BaseUrl);
            speakersUriBuilder.Path = "/speakers";
            using (var speakersResult = await client.GetAsync(speakersUriBuilder.Uri))
            {
                if (speakersResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException($"{await speakersResult.Content.ReadAsStringAsync()}");
                }
                var result = JsonSerializer.Deserialize<Speaker[]>(await speakersResult.Content.ReadAsStringAsync());
                if (result == null)
                {
                    throw new Exception("JSONがパースできませんでした。");
                }
                return result;
            }

        }

        public async Task<string> BuildAudioQueryJsonAsync(string text, int speakerId)
        {
            var audioQueryUriBuilder = new UriBuilder(Config.BaseUrl);
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
                if (audioQueryResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException($"{await audioQueryResult.Content.ReadAsStringAsync()}");
                }
                return await audioQueryResult.Content.ReadAsStringAsync();
            }
        }

        public async Task<Stream> SynthesisAsync(string audioQueryJson, int speakerId)
        {
            var synthesisUriBuilder = new UriBuilder(Config.BaseUrl);
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

        internal class Speaker
        {
            public string name { get; set; } = "";
            public string speaker_uuid { get; set; } = "";
            public Style[] styles { get; set; } = new Style[] { };

            internal class Style
            {
                public string name { get; set; } = "";
                public int id { get; set; }

            }
        }
    }
}
