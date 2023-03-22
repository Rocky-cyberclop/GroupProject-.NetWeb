using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupProject.Controllers
{
    public class SessionHelper
    {
        public static void SetSession(UserSession session)
        {
            HttpContext.Current.Session["UserSession"] = session;
        }
        public static void SetSession(StaffSession session)
        {
            HttpContext.Current.Session["StaffSession"] = session;
        }

        public static UserSession GetUserSession()
        {
            var session = HttpContext.Current.Session["UserSession"];
            if (session == null)
            {
                return null;
            }
            else return session as UserSession;
        }

        public static StaffSession GetStaffSession()
        {
            var session = HttpContext.Current.Session["StaffSession"];
            if (session == null)
            {
                return null;
            }
            else return session as StaffSession;
        }
    }
}