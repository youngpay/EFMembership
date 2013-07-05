using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Web.Core.Forms
{
    public class CustomIdentity : IIdentity
    {
        public string AuthenticationType
        {
            get 
            {
                return "CustomIdentity";
            }
        }
        private bool isAuthenticated;
        public bool IsAuthenticated
        {
            get { return this.isAuthenticated; }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
        }

        public CustomIdentity(string username, bool isAuthenticated)
        {
            this.name = username;
            this.isAuthenticated = isAuthenticated;
        }

        public CustomIdentity(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            if (!Membership.ValidateUser(username, password)) { return; }

            this.isAuthenticated = true;
            this.name = username;
        }
    }
}
