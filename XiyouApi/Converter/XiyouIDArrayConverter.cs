using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace XiyouApi.Converter
{
    internal class XiyouIDListConverter : JsonConverter<List<XiyouID>>
    {
        public override void Write(Utf8JsonWriter writer, List<XiyouID> value, JsonSerializerOptions options)
        {
            var sb = new StringBuilder();
            foreach (var item in value)
            {
                sb.Append(item.ToString());
                sb.Append(',');
            }
            sb.Remove(sb.Length - 1, 1);
            writer.WriteStringValue(sb.ToString());
        }

        public override List<XiyouID> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException();
            var idStrings = reader.GetString()!.Split(',');
            var ids = new List<XiyouID>(idStrings.Length);
            foreach (var idString in idStrings)
            {
                var span = idString.AsSpan();
                if (idString.Length == 0)
                    continue;
                if (idString.StartsWith('\''))
                    span = span.Slice(1, span.Length - 1);
                if (idString.EndsWith('\''))
                    span = span.Slice(0, span.Length - 1);
                ids.Add(new XiyouID(span));
            }
            return ids;
        }
    }
}
