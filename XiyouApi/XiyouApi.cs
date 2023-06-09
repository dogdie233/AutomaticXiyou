﻿using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using XiyouApi.Converter;
using XiyouApi.Model;

namespace XiyouApi
{
    public enum BagStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Ended = 2
    }

    public enum HomeworkStatus
    {
        Unfinished = 0,
        Done = 3,
        OutDatedButCanDo = 7
    }

    public static class Xiyou
    {
        internal struct FormItemComparer : IComparer<KeyValuePair<string, string>>
        {
            public int Compare(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
            {
                return StringComparer.Ordinal.Compare(x.Key, y.Key);
            }
        }

        private static readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new BoolConverter() },
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
        public static readonly string baseUrl = "https://app.xiyouyingyu.com";

        // From https://student.xiyouyingyu.com/static/js/app.85ae7697.js
        private static (string masters, string device, string brand, int belong, string version, string release, string osVersion) _formInfos;

        public static string? SessionId { get; private set; } = null;
        public static string? UserId { get; private set; } = null;
        public static string? ClazzId { get; private set; } = null;

        public static bool IsLogged => SessionId != null && UserId != null && ClazzId != null;

        static Xiyou()
        {
            _formInfos = new("_Sts100#@", "web-pc", "1.7976931348623157e+308", 1, "3.0.0", "3.3.4", "5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36 Edg/112.0.0.0");

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/" + _formInfos.osVersion);
            _httpClient.DefaultRequestHeaders.Add("release", _formInfos.release);
            _httpClient.DefaultRequestHeaders.Add("device", _formInfos.device);
        }

        #region API

        public static async Task<ServerResponse<AccountModel>> Login(string loginAccount, string password)
        {
            var res = await SendFormRequestAsync<ServerResponse<AccountModel>>(baseUrl + "/user/login/account",
                new[] { new KeyValuePair<string, string>("loginAccount", loginAccount), new KeyValuePair<string, string>("password", password) });
            if (res.Data != null)
            {
                SessionId = res.Data.UserInfo.SessionId.ToString();
                UserId = res.Data.UserInfo.Id.ToString();
                ClazzId = res.Data.UserInfo.ClazzId.ToString();
            }
            return res;
        }

        public static Task<ServerResponse<OssParamModel>> GetOssParam() =>
            SendFormRequestAsync<ServerResponse<OssParamModel>>(baseUrl + "/user/getOssParam");

        public static Task<ServerResponse<BagModel[]>> FindBagList(int pageIndex, int pageSize, BagStatus status)
        {
            if (!IsLogged)
                throw new InvalidOperationException("Not logged in yet");
            return SendFormRequestAsync<ServerResponse<BagModel[]>>(baseUrl + "/homework/findBagList", new[]
            {
                new KeyValuePair<string, string>("pageIndex", pageIndex.ToString()),
                new KeyValuePair<string, string>("pageSize", pageSize.ToString()),
                new KeyValuePair<string, string>("clazzId", ClazzId!),
                new KeyValuePair<string, string>("status", ((int)status).ToString())
            });
        }

        public static Task<ServerResponse<HomeworkModel[]>> FindHomeworkListByBagId(XiyouID bagId)
        {
            if (!IsLogged)
                throw new InvalidOperationException("Not logged in yet");
            return SendFormRequestAsync<ServerResponse<HomeworkModel[]>>(baseUrl + "/homework/findListByBagId", new[]
            {
                new KeyValuePair<string, string>("bagId", bagId.ToString())
            });
        }

        public static Task<ServerResponse<RepeatAfterModel>> GetRepeatAfterById(XiyouID groupId)
        {
            if (!IsLogged)
                throw new InvalidOperationException("Not logged in yet");
            return SendFormRequestAsync<ServerResponse<RepeatAfterModel>>(baseUrl + "/repeatAfter/getRepeatAfterById", new[]
            {
                new KeyValuePair<string, string>("repeatAfterId", groupId.ToString())
            });
        }

        public static Task<ServerResponse> SaveRepeatAfterAnswer(XiyouID repeatAfterId, string info, double score, int type, int passageType, XiyouID homeworkId)
        {
            if (!IsLogged)
                throw new InvalidOperationException("Not logged in yet");
            return SendFormRequestAsync<ServerResponse>(baseUrl + "/repeatAfter/saveAnswer", new[]
            {
                new KeyValuePair<string, string>("repeatAfterId", repeatAfterId.ToString()),
                new KeyValuePair<string, string>("info", info),
                new KeyValuePair<string, string>("score", score.ToString("F2")),
                new KeyValuePair<string, string>("type", type.ToString()),
                new KeyValuePair<string, string>("passageType", passageType.ToString()),
                new KeyValuePair<string, string>("itemId", homeworkId.ToString()),
                new KeyValuePair<string, string>("clazzId", ClazzId!),
                new KeyValuePair<string, string>("homeworkType", "1")  // 根据https://student.xiyouyingyu.com/static/js/repeat.dce97e54.js，当homeworkId == null时置0，否则置1
            });
        }

        #endregion

        #region Request Sending

        public static async Task<T> SendV2RequestAsync<T>(string url, object jsonContent)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var jsonString = BuildV2JsonString(jsonContent);
            var sign = CalculateMD5(jsonString + _formInfos.masters);

            request.Headers.Add("sign", sign);
            request.Headers.Add("authToken", SessionId);

            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var res = await _httpClient.SendAsync(request);
            res.EnsureSuccessStatusCode();
            /*if (!res.IsSuccessStatusCode)
                return new ServerResponse<T>("HttpStatusCode vertify failed " + res.StatusCode.ToString());*/

            return (await res.Content.ReadFromJsonAsync<T>(_serializerOptions))!;
        }

        public static Task<T> SendFormRequestAsync<T>(string url) => SendFormRequestAsync<T>(url, Array.Empty<KeyValuePair<string, string>>());

        public static async Task<T> SendFormRequestAsync<T>(string url, IEnumerable<KeyValuePair<string, string>> form)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var dic = new Dictionary<string, string>(form);
            dic.TryAdd("appVersion", _formInfos.version);
            dic.TryAdd("belong", _formInfos.belong.ToString());
            dic.TryAdd("device", _formInfos.device);
            dic.TryAdd("osVersion", _formInfos.osVersion);
            dic.TryAdd("release", _formInfos.release);
            dic.TryAdd("brand", _formInfos.brand);
            dic.TryAdd("timestamp", GetCurrentTimestamp().ToString());
            if (IsLogged)
            {
                dic["sessionId"] = SessionId!;
                dic["userId"] = UserId!;
            }

            var array = dic.ToArray();
            Array.Sort(array, new FormItemComparer());
            var sb = new StringBuilder();
            foreach (var kvp in array)
            {
                sb.Append(kvp.Key);
                sb.Append('=');
                sb.Append(kvp.Value);
                sb.Append('&');
            }
            sb.Remove(sb.Length - 1, 1);  // 移除最后一个&

            var sign = CalculateMD5(CalculateMD5(sb.ToString()) + _formInfos.masters);
            request.Headers.Add("sign", sign);

            request.Content = new FormUrlEncodedContent(array);

            var res = await _httpClient.SendAsync(request);
            res.EnsureSuccessStatusCode();
            /*if (!res.IsSuccessStatusCode)
                return new ServerResponse<T>("HttpStatusCode vertify failed " + res.StatusCode.ToString());*/

            return (await res.Content.ReadFromJsonAsync<T>(_serializerOptions))!;
        }

        #endregion

        #region Utils

        // TODO: 使用Expression进行优化
        private static string BuildV2JsonString(object jsonContent)
        {
            var dic = new Dictionary<string, object?>()
            {
                { "appVersion", _formInfos.version },
                { "belong", _formInfos.belong },
                { "device", _formInfos.device },
                { "osVersion", _formInfos.osVersion },
                { "release", _formInfos.release },
                { "brand", _formInfos.brand },
                { "timestamp", GetCurrentTimestamp() }
            };
            if (IsLogged)
            {
                dic.Add("sessionId", SessionId);
                dic.Add("userId", UserId);
            }

            foreach (var property in jsonContent.GetType().GetProperties())
            {
                if (property.CustomAttributes.Any(d => d.AttributeType == typeof(JsonIgnoreAttribute)))
                    continue;
                dic.Add(property.Name, property.GetValue(jsonContent));
            }

            return JsonSerializer.Serialize(dic, options: _serializerOptions);
        }

        private static long GetCurrentTimestamp() => (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;

        private static string CalculateMD5(string input)
        {
            byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(input));

            var sb = new StringBuilder(32);
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
        #endregion
    }
}