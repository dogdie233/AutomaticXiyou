using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace AutomaticXiyou.Singsound
{
    public static class SingsoundHelper
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            Converters = { new BoolNumberConverter(), new GuidConverter() }
        };

        public static async Task<(ClientWebSocket client, AppModel appParams)> CreateSingsoundConnection(string coreType)
        {
            var cts = new CancellationTokenSource();
            var appParams = new AppModel();
            var client = new ClientWebSocket();
            try
            {
                await client.ConnectAsync(new Uri($"wss://gate-01.api.cloud.ssapi.cn/{coreType}?connect_id={appParams.ConnectId.ToString("N")}"), cts.Token);
                var appParamsString = JsonSerializer.Serialize(appParams, options: _options);
                var connectStr = "{\"cmd\":\"connect\",\"param\":{\"sdk\":{\"version\":20200519,\"sdk_version\":\"v2.2.4\",\"arch\":\"x86_64\",\"source\":6,\"protocol\":1,\"os\":\"Windows 10\",\"product_version\":\"113\",\"product\":\"chrome\"},\"app\":" + appParamsString + "}}";
                await client.SendAsync(Encoding.UTF8.GetBytes(connectStr).AsMemory(), WebSocketMessageType.Text, true, cts.Token);
            }
            catch (Exception)
            {
                client.Dispose();
                throw;
            }
            return (client, appParams);
        }

        public static async Task<string> GetSingsoundResult(string coreType, string refText, Stream wavStream)
        {
            var cts = new CancellationTokenSource();
            (var client, var appParams) = await CreateSingsoundConnection(coreType);
            using (client)
            {
                // Send start command
                await client.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new OperationModel()
                {
                    Cmd = "start",
                    Param = new Dictionary<string, object>()
                    {
                        ["app"] = appParams,
                        ["audio"] = new AudioModel(),
                        ["request"] = new RequestModel(coreType, refText)
                    }
                }, options: _options)), WebSocketMessageType.Text, true, cts.Token);

                // Upload audio
                // Better to use Span? I don't know
                var buffer = new byte[1024];
                while (true)
                {
                    var count = wavStream.Read(buffer, 0, buffer.Length);
                    if (count == 0)
                        break;
                    await client.SendAsync(new ArraySegment<byte>(buffer, 0, count), WebSocketMessageType.Binary, false, cts.Token);
                }
                await client.SendAsync(Array.Empty<byte>(), WebSocketMessageType.Binary, true, cts.Token);
                await client.SendAsync(Array.Empty<byte>(), WebSocketMessageType.Binary, true, cts.Token);

                // Recv json result
                using (var recvMs = new MemoryStream())
                using (var reader = new StreamReader(recvMs, Encoding.UTF8))
                {
                    Array.Clear(buffer);
                    while (true)
                    {
                        var result = await client.ReceiveAsync(buffer, cts.Token);
                        recvMs.Write(buffer, 0, result.Count);
                        if (result.EndOfMessage)
                            break;
                    }

                    recvMs.Seek(0, SeekOrigin.Begin);
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
