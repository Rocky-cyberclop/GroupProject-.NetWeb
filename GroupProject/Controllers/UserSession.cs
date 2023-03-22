using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupProject.Controllers
{
    [Serializable]
    public class UserSession
    {
        private string UserName { get; set; }
        private string Ten { get; set; }
        public UserSession(string UserName, string Ten)
        {
            this.UserName = UserName;
            this.Ten = Ten;
        }
        public string getUserName()
        {
            return UserName;
        }
    }
}