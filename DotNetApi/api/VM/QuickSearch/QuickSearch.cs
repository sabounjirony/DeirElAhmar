using System.Web;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace VM.QuickSearch
{
    public class QuickSearchResult
    {
        public string id { get; set; }
        public string text { get; set; }
    }
    public class QuickSearchPagedResult
    {
        public long total { get; set; }
        public List<QuickSearchResult> results { get; set; }

        public QuickSearchPagedResult()
        {
            results = new List<QuickSearchResult>();
        }

    }
    public class QuickSearch
    {
        public static void FormatResult(ref HttpContext context, QuickSearchPagedResult results)
        {
            var js = new JavaScriptSerializer();
            var jSonResults = js.Serialize(results);
            context.Response.ContentType = "application/json";
            context.Response.Write(jSonResults);
        }

        public static void FormatResult(ref HttpContext context, QuickSearchResult[] results)
        {
            var js = new JavaScriptSerializer();
            var jSonResults = js.Serialize(results);
            context.Response.ContentType = "application/json";
            context.Response.Write(jSonResults);
        }

        public static void FormatResult(ref HttpContext context, QuickSearchResult results)
        {
            var js = new JavaScriptSerializer();
            var jSonResults = js.Serialize(results);
            context.Response.ContentType = "application/json";
            context.Response.Write(jSonResults);
        }
    }
}