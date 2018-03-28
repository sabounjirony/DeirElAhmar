using System.Collections.Generic;
using Tools;

namespace VM.System.MultiLanguage
{
    public class DescriptionVm
    {
        public Dictionary<string, string> Languages { get; set; }
        public Dictionary<string, string> DescriptionParents { get; set; }
        public DM.Models.System.MultiLanguage.Description Description { get; set; }
        public Enumerations.ActionMode ActionMode { get; set; }
    }
}