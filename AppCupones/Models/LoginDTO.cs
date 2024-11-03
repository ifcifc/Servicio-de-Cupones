using Common.Models;

namespace AppCupones.Models
{
    public class LoginDTO : Model
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
