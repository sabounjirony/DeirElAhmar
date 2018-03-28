namespace VM.Tree
{
    public class TreeNodeAjax
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool children { get; set; }
        public string icon { get; set; }
        public bool allowView { get; set; }
        public bool allowAdd { get; set; }
        public bool allowEdit { get; set; }
        public bool allowDelete { get; set; }
        public JsTreeNodeModelState state { get; set; }
        public JsTreeNodeModelLiAttr li_attr { get; set; }
        public string data { get; set; }
    }
    public class JsTreeNodeModelState
    {
        public bool opened { get; set; }
    }
    public class JsTreeNodeModelLiAttr
    {
        public string Key { get; set; }
        public string Class { get; set; }
    }
}
