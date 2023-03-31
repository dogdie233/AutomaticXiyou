namespace XiyouApi.Model
{
    public class AccountModel
    {
        public UserInfoModel UserInfo { get; set; } = new();
    }

    public class UserInfoModel
    {
        public string AppVersion { get; set; } = null!;
        public string Belong { get; set; } = null!;
        public string Brand { get; set; } = null!; 
        public string Brief { get; set; } = null!;
        public string CityId { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string ClassName { get; set; } = null!;
        public XiyouID ClazzId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime CreateAtStr { get; set; }
        public string Device { get; set; } = null!;
        public string DistrictId { get; set; } = null!;
        public string Expire { get; set; } = null!;  // 我获取到的数据是0
        public string ExpireAt { get; set; } = null!;  // 我获取到的数据是2023-03-31
        public bool FirstLogin { get; set; }
        public string GradeId { get; set; } = null!;
        public bool HasPhone { get; set; }
        public bool HasPwd { get; set; }
        public XiyouID Id { get; set; }
        public int Info { get; set; }
        public bool IsRealName { get; set; }
        public bool IsSignUp { get; set; }
        public string LoginAccount { get; set; } = null!;
        public DateTime LoginTime { get; set; }
        public int MemberType { get; set; }
        public string Name { get; set; } = null!;
        public string OsVersion { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string ProvinceId { get; set; } = null!;
        public string ProvinceName { get; set; } = null!;
        public string RealName { get; set; } = null!;
        public bool SchoolChange { get; set; }
        public XiyouID SchoolId { get; set; }
        public string SchoolName { get; set; } = null!;
        public XiyouID SessionId { get; set; }
        public int Status { get; set; }
        public int ThirdPartyType { get; set; }
        public int Type { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime UpdateAtStr { get; set; }
    }
}
