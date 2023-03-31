using System.Text.Json.Serialization;

using XiyouApi.Converter;

namespace XiyouApi.Model
{
    public class HomeworkModel
    {
        public int AnswerTimes { get; set; }
        public XiyouID BagId { get; set; }
        public int Category { get; set; }
        public XiyouID ClazzId { get; set; }
        public DateTime CreateAt { get; set; }
        public XiyouID Creator { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateStart { get; set; }
        public int ExpireRedo { get; set; }
        public int Flag { get; set; }
        public bool HasReview { get; set; }
        public XiyouID Id { get; set; }
        public bool IsLock { get; set; }
        public XiyouID ModuleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public XiyouID PaperGroupId { get; set; }
        [JsonConverter(typeof(XiyouIDListConverter))] public List<XiyouID> PaperQuestionIds { get; set; } = new List<XiyouID>();
        public int QuestionsNum { get; set; }
        public double Rate { get; set; }
        public string ReadingType { get; set; } = string.Empty;  // "0"
        public double Score { get; set; }
        public string SecondName { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;  // "0"
        [JsonConverter(typeof(EnumIndexButStringConverter<HomeworkStatus>))] public HomeworkStatus Status { get; set; }  // 貌似是和完成情况有关
        public XiyouID TeacherId { get; set; }
        public long Time { get; set; }  // Unknown type
        public bool ToPublic { get; set; }
        public double TotalScore { get; set; }
        public string Type { get; set; } = string.Empty;
        public string TypeList { get; set; } = string.Empty;
        public DateTime UpdateAt { get; set; }
        public int Version { get; set; }
        public int Week { get; set; }
    }
}
