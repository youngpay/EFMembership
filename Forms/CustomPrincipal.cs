using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Web.Core.Forms
{
    public class CustomPrincipal : IPrincipal
    {
        private IIdentity _identity;

        public IIdentity Identity
        {
            get { return _identity; }
        }

        public bool IsInRole(string role)
        {
            if (Identity == null || !Identity.IsAuthenticated || string.IsNullOrEmpty(Identity.Name))
                return false;

            return System.Web.Security.Roles.IsUserInRole(Identity.Name, role);
        }

        public CustomPrincipal(IIdentity identity)
        {
            _identity = identity;
        }
    }
}
