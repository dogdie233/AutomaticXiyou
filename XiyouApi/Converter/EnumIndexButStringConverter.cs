using System.Text.Json.Serialization;
using System.Text.Json;

namespace XiyouApi.Converter
{
    internal class EnumIndexButStringConverter<T> : JsonConverter<T> where T : struct
    {
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.GetHashCode().ToString());

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.TryGetInt32(out var number) ? (T)Enum.ToObject(typeof(T), number) : throw new JsonException(),
                JsonTokenType.String => int.TryParse(reader.GetString(), out var number) ? (T)Enum.ToObject(typeof(T), number) : Enum.TryParse<T>(reader.GetString(), out var e) ? e : throw new JsonException(),
                _ => throw new JsonException()
            };
        }
    }
}
