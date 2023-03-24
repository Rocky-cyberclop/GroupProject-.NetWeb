using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupProject.Controllers
{
    public class StaffSession
    {
        private string UserName { get; set; }
        private string Ten { get; set; }
        private string Quyen { get; set; }
        public StaffSession(string UserName, string Ten, string Quyen)
        {
            this.UserName = UserName;
            this.Ten = Ten;
            this.Quyen = Quyen;
        }

        public string GetRight()
        {
            return this.Quyen;
        }

        public string GetID()
        {
            return this.UserName;
        }
    }
}