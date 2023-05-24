namespace DemoWebApp.Models.Requests
{
    public class LoginRequest
    {
        public string Identity { get; set; }
        public string Secret { get; set; }
    }
}