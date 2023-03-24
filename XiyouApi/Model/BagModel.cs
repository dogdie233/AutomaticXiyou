namespace XiyouApi.Model
{
    public class BagModel
    {
        public string Category { get; set; } = null!;
        public string ClazzId { get; set; } = null!;
        public string CreateAt { get; set; } = null!;
        public string DateEnd { get; set; } = null!;
        public string DateStart { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string ItemIds { get; set; } = null!; 
        public int ItemNum { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string TeacherId { get; set; } = null!;
        public string UpdateAt { get; set; } = null!;
    }
}
