using System.Text.Json.Serialization;
using System.Text.Json;

namespace XiyouApi.Converter
{
    internal class BoolConverter : JsonConverter<bool>
    {
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
            writer.WriteBooleanValue(value);

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.Number:
                    return reader.TryGetInt64(out long l) ? Convert.ToBoolean(l) : reader.TryGetDouble(out double d) ? Convert.ToBoolean(d) : false;
                case JsonTokenType.String:
                    var str = reader.GetString();
                    if (str == null)
                        return false;
                    if (bool.TryParse(str, out var result))
                        return result;
                    if (long.TryParse(str, out var number))
                        return Convert.ToBoolean(number);
                    if (double.TryParse(str, out var db))
                        return Convert.ToBoolean(db);
                    throw new JsonException();
                default:
                    throw new JsonException();
            }
        }
    }
}
