using System.Text.Json.Serialization;
using System.Text.Json;

namespace XiyouApi.Converter
{
    internal class XiyouIDConverter : JsonConverter<XiyouID>
    {
        public override void Write(Utf8JsonWriter writer, XiyouID value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString());

        public override XiyouID Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException();
            return new XiyouID(reader.GetString()!);
        }
    }
}
