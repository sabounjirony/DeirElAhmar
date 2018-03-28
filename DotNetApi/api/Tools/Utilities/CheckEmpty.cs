using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools.Utilities
{
    public static class CheckEmpty
    {
        public static string String(object val)
        {
            if (val == null) return "";
            return val == DBNull.Value ? "" : val.ToString().Trim();
        }

        public static double Double(object val, bool notMinus1 = false)
        {
            if (val == null) return 0;
            double v;
            if (!double.TryParse(val.ToString().Trim(), out v)) return 0;
            if (notMinus1)
            {
                if (v == -1) return 0;
            }
            return v;
        }

        public static long Numeric(object val, bool notMinus1 = false)
        {
            if (val == null) return 0;
            long v;
            if (!long.TryParse(val.ToString().Trim(), out v)) return 0;
            if (notMinus1)
            {
                if (v == -1) return 0;
            }
            return v;
        }

        public static bool Boolean(object val)
        {
            if (val == null) return false;
            if (val == DBNull.Value) return false;
            if (val.ToString() == "0") return false;
            if (val.ToString() == "1") return true;

            return Convert.ToBoolean(val);
        }

        public static DateTime Date(object val)
        {
            if (val == null) return DateUtilities.GetInvariantDate(DateTime.MinValue);
            if (val == DBNull.Value) return DateUtilities.GetInvariantDate(DateTime.MinValue);
            try
            {
                DateTime dateTime;
                DateTime.TryParse(val.ToString(), out dateTime);
                return DateUtilities.GetInvariantDate(DateTime.Parse(val.ToString()));
            }
            catch (Exception)
            {
                return DateUtilities.GetInvariantDate(new DateTime(1900, 1, 1));
            }
        }

        public static List<long> ArrayLong(object val)
        {
            if (!Convert.ToString(val).Contains(','))
            {
                var v = Numeric(val);
                return v == 0 ? null : new List<long> { v };
            }
            var ar = val.ToString().Split(',');
            return ar.ToList().ConvertAll(Convert.ToInt64);
        }

        public static long Numeric(ref Dictionary<string, object> dict, string key)
        {
            if (dict == null) return 0;
            return dict.ContainsKey(key) ? Numeric(dict[key]) : 0;
        }

        public static bool Boolean(ref Dictionary<string, object> dict, string key)
        {
            if (dict == null) return false;
            return dict.ContainsKey(key) && Boolean(dict[key]);
        }

        public static string String(ref Dictionary<string, object> dict, string key)
        {
            if (dict == null) return "";
            return dict.ContainsKey(key) ? String(dict[key]) : "";
        }
    }
}