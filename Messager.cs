using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationApp_Test
{
    public interface IMessage
    {
        void SentMessage(string login, bool flag);
    }

    public class Messager
    {
       
        public void SentMessage(string login, bool flag)
        {
            string info;

            if (flag) info = "data is got from database";
            else info = "data is sent to database";

            MailAddress from = new MailAddress("");
            MailAddress to = new MailAddress(login);
            MailMessage message = new MailMessage(from, to) 
            {
                Subject = info
            };
            SmtpClient smtp = new SmtpClient("", 0)
            {

            };
            smtp.Send(message);
        }
    }
}
