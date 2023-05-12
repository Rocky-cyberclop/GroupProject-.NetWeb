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
        static int code;
        public static void setCode()
        {
            code = rd.Next(1000, 9999);
        }
        public static int getCode()
        {
            return code;
        }

        public static bool SendMail(string _from, string _to, string _password, bool type)
        {
            setCode();
            //var _body = "";
            MailMessage massage = new MailMessage();
            massage.IsBodyHtml = true;
            massage.ReplyToList.Add(_from);
            massage.From = new MailAddress(_from, "Rau Củ Sạch Cần Thơ");
            massage.To.Add(_to);
            if(type == true)
            {
                massage.Subject = getCode() + " là mã khôi phục tài khoản của bạn";
                massage.Body = "<html><body>Xin chào,<br><p>Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu của bạn.<br> Nhập mã đặt lại mật khẩu sau đây:</p>" +
                    "<div style=\"font-size:15px;padding:10px;background-color:#f2f2f2;text-align:center;border-radius:7px;border: 1px solid #1877f2;background:#e7f3ff\">" + getCode() + "</body></html>";
            }
            else
            {
                massage.Subject = getCode() + " là mã xác thực tài khoản của bạn";
                massage.Body = "<html><body>Xin chào,<br><p>Chúng tôi đã nhận được yêu cầu tạo tài khoản của bạn.<br> Nhập mã xác thực tài khoản sau đây:</p>" +
                    "<div style=\"font-size:15px;padding:10px;background-color:#f2f2f2;text-align:center;border-radius:7px;border: 1px solid #1877f2;background:#e7f3ff\">" + getCode() + "</body></html>";
            }
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
    }
}