﻿using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Web_shop.Utility
{
    public static class SessionExtentions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value is null 
                ? default 
                : JsonSerializer.Deserialize<T>(value);
        }
    }
}
