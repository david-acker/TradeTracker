using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace TradeTracker.Application.Common.Helpers
{
    public static class EntityTagHelper
    {
        public static string Generate(object content)
        {
            var serializedContent = JsonConvert.SerializeObject(content);

            return Generate(Encoding.UTF8.GetBytes(serializedContent));
        }

        public static string Generate(string content)
        {
            return Generate(Encoding.UTF8.GetBytes(content));
        }

        public static string Generate(byte[] contentBytes)
        {
            byte[] hash = ComputeHash(contentBytes);

            return FormatAsEntityTag(hash);
        }

        public static string Generate(string key, object content)
        {
            var serializedContent = JsonConvert.SerializeObject(content);
            
            return Generate(key, Encoding.UTF8.GetBytes(serializedContent));
        }

        public static string Generate(string key, string content)
        {
            return Generate(key, Encoding.UTF8.GetBytes(content));
        }

        public static string Generate(string key, byte[] contentBytes)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] combinedBytes = Combine(keyBytes, contentBytes);

            byte[] hash = ComputeHash(combinedBytes);
            
            return FormatAsEntityTag(hash);
        }

        private static byte[] Combine(params byte[][] input)
        {
            if (input?.Count() > 1)
            {
                return input
                    .SelectMany(i => i)
                    .ToArray();
            }
            else
            {
                return input?.SingleOrDefault();
            }
        }

        private static byte[] ComputeHash(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(data);
            }
        }

        private static string FormatAsEntityTag(byte[] data)
        {
            return BitConverter
                .ToString(data)
                .Replace("-", "");
        }
    }
}