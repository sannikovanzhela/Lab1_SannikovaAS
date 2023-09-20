using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationApp_Test.Database
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }

        public string UserLogin { get; set; }
        public string UserPassword { get; set; }

        public User() { }

        public User (string UserLogin, string UserPassword)
        {
            this.UserLogin = UserLogin;
            this.UserPassword = UserPassword;
        }
    }
}
