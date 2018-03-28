namespace VM.System.Security
{
    public class LoginVm
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ComputerName { get; set; }
        public string Language { get; set; }
        public string Browser { get; set; }
        public string RedirectTo { get; set; }
        public string ReturnUrl { get; set; }
        public string SecurityToken { get; set; }
        public bool IsAuthenticated { get; set; }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public bool CrossBranches { get; set; }
        public long PageSize { get; set; }
    }
}