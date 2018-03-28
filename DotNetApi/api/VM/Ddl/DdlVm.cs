using System.Linq;
using System.Collections;
using System.Web.Script.Serialization;

namespace VM.Ddl
{
    public class DdlVm
    {
        public static string FormatResult(IList list)
        {
            var rows = (from dynamic entry in list
                        select new DdlOption
                        {
                            value = entry.value,
                            label = entry.label
                        }).ToList();

            return new JavaScriptSerializer().Serialize(rows);
        }
        public struct DdlOption
        {
            public string label;
            public string value;

            public DdlOption(string label, string value)
            {
                this.label = label;
                this.value = value;
            }
        }
    }
}
