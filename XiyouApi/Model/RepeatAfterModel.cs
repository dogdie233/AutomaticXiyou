using System.Text.Json.Serialization;

namespace XiyouApi.Model
{
    public class RepeatAfterModel
    {
        public class SentenseModel
        {
            public string AudioText { get; set; } = null!;
            public float BeginTime { get; set; }
            public float EndTime { get; set; }
            public XiyouID Id { get; set; }
            [JsonPropertyName("seq")] public int Sequence { get; set; }
            public string Translate { get; set; } = null!;
        }

        public string AudioUrl { get; set; } = null!;
        public string BackAudioUrl { get; set; } = null!;
        public string CityId { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string ContentText { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public string Creator { get; set; } = null!;
        public int Difficult { get; set; }
        public int GradeType { get; set; }
        public XiyouID Id { get; set; }
        public string Introduce { get; set; } = null!;
        public string LibContent { get; set; } = null!;
        public SentenseModel[] List { get; set; } = Array.Empty<SentenseModel>();
        public int Material { get; set; }
        public string Name { get; set; } = null!;
        public int PassageType { get; set; }
        public string PhotoUrl { get; set; } = null!;
        public int ProvinceId { get; set; }
        public string QuestionLabel { get; set; } = null!;
        public string Resource { get; set; } = null!;
        public string ShortName { get; set; } = null!;
        public int State { get; set; }
        public string Translate { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int Unit { get; set; }
        public DateTime UpdateAt { get; set; }
        public string Updator { get; set; } = null!;
        public int Version { get; set; }
        public string VideoUrl { get; set; } = null!;
    }
}