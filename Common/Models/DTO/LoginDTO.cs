using Common.Models;

namespace Common.Models
{
    public class LoginDTO : Model
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
