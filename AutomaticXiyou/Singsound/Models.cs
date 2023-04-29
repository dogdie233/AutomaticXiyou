using System.Text.Json.Serialization;

namespace AutomaticXiyou.Singsound
{
    public class AppModel
    {
        [JsonPropertyName("applicationId")] public string ApplicationId { get; set; } = "a227";
        [JsonPropertyName("clientId")] public string ClientId { get; set; } = string.Empty;
        [JsonPropertyName("timestamp")] public string Timestamp { get; set; } = GetCurrentTimestamp().ToString();
        [JsonPropertyName("sig")] public string Sig { get; set; } = "default";
        [JsonPropertyName("userId")] public string UserId { get; set; } = "guest";
        [JsonPropertyName("connect_id")] public Guid ConnectId { get; set; } = Guid.NewGuid();

        private static long GetCurrentTimestamp() => (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public class AudioModel
    {
        [JsonPropertyName("audioType")] public string AudioType { get; set; } = "wav";
        [JsonPropertyName("channel")] public byte Channel { get; set; } = 1;
        [JsonPropertyName("sampleBytes")] public uint SampleBytes { get; set; } = 2;
        [JsonPropertyName("sampleRate")] public uint SampleRate { get; set; } = 16000;
    }

    public class RequestModel
    {
        [JsonPropertyName("attachAudioUrl")] public bool AttachAudioUrl { get; set; } = true;
        [JsonPropertyName("audioUrlScheme")] public string AudioUrlScheme { get; set; } = "https";
        [JsonPropertyName("compress")] public string Compress { get; set; } = "raw";
        [JsonPropertyName("coreType")] public string CoreType { get; set; }
        [JsonPropertyName("evalTime")] public int EvalTime { get; set; } = 156000;
        [JsonPropertyName("first")] public bool First { get; set; } = true;
        [JsonPropertyName("refText")] public string RefText { get; set; }
        [JsonPropertyName("request_id")] public Guid RequestId { get; set; } = Guid.NewGuid();
        [JsonPropertyName("sampleRate")] public uint SampleRate { get; set; } = 16000;
        [JsonPropertyName("saveAudio")] public bool SaveAudio { get; set; } = true;
        [JsonPropertyName("tokenId")] public Guid TokenId { get; set; } = Guid.NewGuid();

        public RequestModel()
        {
            CoreType = null!;
            RefText = null!;
        }

        public RequestModel(string coreType, string refText)
        {
            CoreType = coreType;
            RefText = refText;
        }
    }

    public class OperationModel
    {
        [JsonPropertyName("cmd")] public string Cmd { get; set; } = string.Empty;
        [JsonPropertyName("param")] public Dictionary<string, object> Param { get; set; } = new Dictionary<string, object>();
    }
}
