using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journal.Domain.JsonModels.User
{
    public class LoginUserJsonModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
