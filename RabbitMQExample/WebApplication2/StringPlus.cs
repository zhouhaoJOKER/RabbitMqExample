using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text; 
using Newtonsoft.Json;
using WebApplication2.Dto;

namespace Utility
{
    public static class StringPlus
    {
        private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly Random Random = new Random();

        public static string GenerateRandomString(string prefix, int length = 15)
        {
            StringBuilder stringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int index = Random.Next(Characters.Length);
                stringBuilder.Append(Characters[index]);
            }
            return prefix + stringBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // 转换为十六进制字符串
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 将DateTime转换为字符串格式
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo? field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string str)
        {
            return JsonConvert.DeserializeObject<List<T>>(str) ?? new List<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string str) where T : class, new()
        {
            return JsonConvert.DeserializeObject<T>(str) ?? new T();
        }

        public static string ToJson(this object obj)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRequireRecourceTimeRange(DateTime nowTime)
        { 
            List<string> timeRanges = [];
            if (nowTime.Minute >= 45)
            {// 50m=>30m:30m
                timeRanges.Add(nowTime.Hour.ToString("") + ":30" + "-" + (nowTime.Hour + 1) + ":00");
                timeRanges.Add((nowTime.Hour + 1) + ":00" + "-" + (nowTime.Hour + 1) + ":30");
            }
            else if (nowTime.Minute > 30)
            {// 35m=>30m:60m
                timeRanges.Add(nowTime.Hour.ToString("") + ":30" + "-" + (nowTime.Hour + 1) + ":00");
            }
            else if (nowTime.Minute > 15)
            {// 20m => 0m:60
                timeRanges.Add(nowTime.Hour.ToString("") + ":00" + "-" + nowTime.Hour + ":30");
                timeRanges.Add(nowTime.Hour.ToString("") + ":30" + "-" + (nowTime.Hour + 1) + ":00");
            }
            else
            {// 8m=>0m:30m
                timeRanges.Add(nowTime.Hour.ToString("") + ":00" + "-" + nowTime.Hour + ":30");
            }
            return timeRanges;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ArraySegment<byte> ToSocketMessage(this WSMessage message)
        {
            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            return new ArraySegment<byte>(buffer);
        }

        /// <summary>
        /// 获取ZteUser密码
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static string GetZteUserPwd(string account)
        {
            return "Fm!" + account.Substring(account.Length - 6) + "999";
        }
    } 
}
