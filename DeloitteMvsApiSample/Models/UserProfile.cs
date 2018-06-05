namespace DeloitteMvcApiSample.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string UniqueName { get; set; }
        public string[] Roles { get; set; }
    }
}
