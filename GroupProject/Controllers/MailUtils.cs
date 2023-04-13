using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace GroupProject.Controllers
{
    public class MailUtils
    {
        static Random rd = new Random();
        static int code = rd.Next(1000, 9999);
        public static bool SendMail(string _from, string _to, string _subject, string _body, string _password)
        {
            MailMessage massage = new MailMessage(_from, _to, _subject, _body);

            var smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(_from, _password);
            try
            {
                smtpClient.Send(massage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static int getCode()
        {
            return code;
        }
        public static void reCode()
        {
            code = rd.Next(1000, 9999);
        }
    }
}