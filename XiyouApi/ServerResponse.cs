using System.Text.Json.Serialization;

namespace XiyouApi
{
    public class ServerResponse<T>
    {
        public T? Data { get; set; }
        public string? Note { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] public int State { get; set; }

        internal ServerResponse() { }

        internal ServerResponse(string errorMsg)
        {
            Note = errorMsg;
        }
    }
}
