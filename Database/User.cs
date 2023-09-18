using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationApp_Test.Database
{
    public class User
    {
        private int IdUser { get; set; }

        private string UserLogin, UserPassword;

        public User() { }

        public User (string UserLogin, string UserPassword)
        {
            this.UserLogin = UserLogin;
            this.UserPassword = UserPassword;
        }
    }
}
