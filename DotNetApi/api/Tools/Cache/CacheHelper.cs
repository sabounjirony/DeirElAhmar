using System;
using System.Web;
using System.Web.Caching;

namespace Tools.Cache
{
    public class CacheHelper
    {
        public static TimeSpan NoSlidingExpiration = new TimeSpan(24, 0, 0);
        public static DateTime NoAbsoluteExpiration = DateTime.Now.AddDays(1);

        public static void Add<T>(T o, string key, double cacheInSeconds) where T : class
        {
            // NOTE: Apply expiration parameters as you see fit.
            // In this example, I want an absolute
            // timeout so changes will always be reflected
            // at that time. Hence, the NoSlidingExpiration.
            HttpRuntime.Cache.Insert(
                key,
                o,
                null,
                DateTime.Now.AddSeconds(cacheInSeconds), NoSlidingExpiration);
        }

        public static bool Exists(string key)
        {
            return HttpRuntime.Cache[key] != null;
        }

        public static bool ExistsLike(string key)
        {
            const bool toRet = false;
            var ie = HttpRuntime.Cache.GetEnumerator();
            while (ie.MoveNext())
            {
                var k = ie.Key.ToString();
                if (!string.IsNullOrWhiteSpace(key) && k.Contains(key))
                    return true;
            }
            return toRet;
        }

        public static object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public static bool Get<T>(string key, out T value)
        {
            try
            {
                if (!Exists(key))
                {
                    value = default(T);
                    return false;
                }

                value = (T)HttpRuntime.Cache[key];
            }
            catch
            {
                value = default(T);
                return false;
            }

            return true;
        }

        public static T Get<T>(string key) where T : class
        {
            try
            {
                return (T)HttpRuntime.Cache[key];
            }
            catch
            {
                return null;
            }
        }

        public static object Remove(string key)
        {
            return HttpRuntime.Cache.Remove(key);
        }

        public static void Add(string key, object value)
        {
            HttpRuntime.Cache.Insert(key, value, null, DateTime.MaxValue, NoSlidingExpiration);
        }

        public static void Add(string key, object value, int minutes)
        {
            if (minutes > 0)
                HttpRuntime.Cache.Insert(key, value, null, DateTime.MaxValue, new TimeSpan(0, minutes, 0));
        }

        public static void Add(string key, object value, DateTime end)
        {
            HttpRuntime.Cache.Insert(key, value, null, end, new TimeSpan(0, 15, 0));
        }

        public static void Add(string key, object value, string fileDependency, double minutes)
        {
            HttpRuntime.Cache.Insert(key, value, new CacheDependency(fileDependency), NoAbsoluteExpiration, TimeSpan.FromMinutes(minutes));
        }

        public static void Add(string key, object value, string[] fileDependencies, double minutes)
        {
            HttpRuntime.Cache.Insert(key, value, new CacheDependency(fileDependencies), NoAbsoluteExpiration, TimeSpan.FromMinutes(minutes));
        }

        public static void Add(string key, object value, CacheDependency dep)
        {

            HttpRuntime.Cache.Insert(key, value, dep, DateTime.MaxValue, TimeSpan.Zero);

        }

        public static void Clear(string search)
        {
            var ie = HttpRuntime.Cache.GetEnumerator();
            while (ie.MoveNext())
            {

                var k = ie.Key.ToString();
                if ((!string.IsNullOrWhiteSpace(search) && k.StartsWith(search)) || search == null)
                {
                    //Exclude users tickets timestamps
                    if (search == null && (k.StartsWith("User_DateStamp_") || k.StartsWith("User_ValidTill_")))
                    { continue; }

                    HttpRuntime.Cache.Remove(k);
                }
                //else
                //{
                //    HttpRuntime.Cache.Remove(_k);
                //}
            }
        }

        public static void Clear()
        {
            Clear(null);
        }

    }
}