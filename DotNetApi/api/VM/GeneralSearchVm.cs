using System.Collections.Generic;

namespace VM
{
    public class GeneralSearchVm
    {
        public string Title { get; set; }
        public List<GeneralSearchResult> Results { get; set; }
        public long Total;
    }

    public class GeneralSearchResult
    {
        public string Id;
        public string Text { get; set; }
        public string Url { get; set; }
        public string ClickEvent { get; set; }
    }
}