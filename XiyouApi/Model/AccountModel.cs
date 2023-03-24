namespace XiyouApi.Model
{
    public class AccountModel
    {
        public UserInfoModel UserInfo { get; set; } = new();
    }

    public class UserInfoModel
    {
        public string AppVersion { get; set; } = string.Empty;
        public string Belong { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty; 
        public string Brief { get; set; } = string.Empty;
        public string CityId { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string ClazzId { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime CreateAtStr { get; set; }
        public string Device { get; set; } = string.Empty;
        public string DistrictId { get; set; } = string.Empty;
        public string Expire { get; set; } = string.Empty;  // 我获取到的数据是0
        public string ExpireAt { get; set; } = string.Empty;  // 我获取到的数据是2023-03-31
        public bool FirstLogin { get; set; }
        public string GradeId { get; set; } = string.Empty;
        public bool HasPhone { get; set; }
        public bool HasPwd { get; set; }
        public string Id { get; set; } = string.Empty;
        public int Info { get; set; }
        public bool IsRealName { get; set; }
        public bool IsSignUp { get; set; }
        public string LoginAccount { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public int MemberType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string OsVersion { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string ProvinceId { get; set; } = string.Empty;
        public string ProvinceName { get; set; } = string.Empty;
        public string RealName { get; set; } = string.Empty;
        public bool SchoolChange { get; set; }
        public string SchoolId { get; set; } = string.Empty;
        public string SchoolName { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public int Status { get; set; }
        public int ThirdPartyType { get; set; }
        public int Type { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime UpdateAtStr { get; set; }
    }
}
