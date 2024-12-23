namespace _21T1020254.DomainModels
{
    public class UserAccount
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";

        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public string RoleNames { get; set; } = "";
        public string Password { get; set; } = ""; // Add this property to match the Password field
    }
}
