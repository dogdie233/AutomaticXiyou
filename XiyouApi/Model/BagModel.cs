using System.Text.Json.Serialization;

using XiyouApi.Converter;

namespace XiyouApi.Model
{
    public class BagModel
    {
        public string Category { get; set; } = null!;
        public XiyouID ClazzId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateStart { get; set; }
        public XiyouID Id { get; set; }
        [JsonConverter(typeof(XiyouIDListConverter))] public List<XiyouID> ItemIds { get; set; } = new List<XiyouID>();
        public int ItemNum { get; set; }
        public string Name { get; set; } = null!;
        [JsonConverter(typeof(EnumIndexButStringConverter<BagStatus>))] public BagStatus Status { get; set; }
        public XiyouID TeacherId { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
