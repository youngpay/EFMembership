using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace EFMembership
{
    public class EFRoleProvider : RoleProvider
    {
        private EFMembershipDbContext db;

        private string _applicationName;

        private string connStringName;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);
            if (config["connectionStringName"] == null)
            {
                config["connectionStringName"] = "DefalutCollection";
            }

            this.connStringName = config["connectionStringName"];
            config.Remove("connectionStringName");

            db = new EFMembershipDbContext(Utils.GetConfigDataBaseName(connStringName));
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            if (usernames == null)
                throw new ArgumentNullException("usernames");

            if (roleNames == null)
                throw new ArgumentNullException("roleNames");



            var q1 = from u in db.Users
                     where usernames.Contains(u.UserName)
                     select u.UserId;

            var q2 = from r in db.Roles
                     where roleNames.Contains(r.RoleName)
                     select r.RoleId;

            var users = q1.ToArray();
            var roles = q2.ToArray();

            for (int i = 0; i < users.Length; i++)
            {
                for (int j = 0; j < roles.Length; j++)
                {
                    var item = new UsersInRoles { RoleId = roles[j], UserId = users[i] };
                    db.UsersInRoles.Add(item);
                }
            }
            db.SaveChanges();
        }

        public override string ApplicationName
        {
            get
            {
                return this._applicationName;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            if (roleName == null)
                throw new ArgumentNullException("roleName");


            var role = new Roles { RoleName = roleName };
            db.Roles.Add(role);
            db.SaveChanges();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (roleName == null)
                throw new ArgumentNullException("roleName");

            var query = from r in db.Roles
                        where r.RoleName == roleName
                        select r;
            var role = query.FirstOrDefault();
            if (role != null)
            {
                var q1 = from ur in db.UsersInRoles
                         where ur.RoleId == role.RoleId
                         select ur;

                var userInRoles = q1.ToList();
                if (userInRoles != null)
                {
                    if (throwOnPopulatedRole)
                    {
                        throw new Exception("roleName:" + roleName);
                    }

                    foreach (var item in userInRoles)
                    {
                        db.UsersInRoles.Remove(item);
                    }
                    db.SaveChanges();
                }
                return true;
            }
            return false;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (roleName == null)
                throw new ArgumentNullException("roleName");

            if (usernameToMatch == null)
                throw new ArgumentNullException("usernameToMatch");



            var query = from ur in db.UsersInRoles
                        from r in db.Roles
                        from u in db.Users
                        where r.RoleId == ur.RoleId &&
                        ur.UserId == u.UserId &&
                        r.RoleName == roleName &&
                        u.UserName.Contains(usernameToMatch)
                        select u.UserName;
            return query.ToArray();
        }

        public override string[] GetAllRoles()
        {
            var query = from r in db.Roles
                        select r.RoleName;

            var list = query.ToArray();
            return list;
        }

        public override string[] GetRolesForUser(string username)
        {
            if (username == null)
                throw new ArgumentNullException("username");



            var query = from u in db.Users
                        from ur in db.UsersInRoles
                        from r in db.Roles
                        where ur.UserId == u.UserId &&
                        ur.RoleId == r.RoleId &&
                        u.UserName == username
                        select r.RoleName;
            var list = query.ToArray();
            return list;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (roleName == null)
                throw new ArgumentNullException("roleName");



            var query = from u in db.Users
                        from ur in db.UsersInRoles
                        from r in db.Roles
                        where ur.UserId == u.UserId &&
                        ur.RoleId == r.RoleId &&
                        r.RoleName == roleName
                        select u.UserName;
            var list = query.ToArray();
            return list;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            if (roleName == null)
                throw new ArgumentNullException("roleName");


            var query = from u in db.Users
                        from ur in db.UsersInRoles
                        from r in db.Roles
                        where ur.UserId == u.UserId &&
                        ur.RoleId == r.RoleId &&
                        u.UserName == username &&
                        r.RoleName == roleName
                        select ur.UsersInRolesId;
            return query.Count() > 0;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var query = from u in db.Users
                        from ur in db.UsersInRoles
                        from r in db.Roles
                        where ur.UserId == u.UserId &&
                        ur.RoleId == r.RoleId &&
                        usernames.Contains(u.UserName) &&
                        roleNames.Contains(r.RoleName)
                        select ur;

            var list = query.ToList();
            if (list != null)
            {
                foreach (var item in list)
                {
                    db.UsersInRoles.Remove(item);
                }
                db.SaveChanges();
            }
        }

        public override bool RoleExists(string roleName)
        {
            if (roleName == null)
                throw new ArgumentNullException("roleName");



            return (from r in db.Roles where r.RoleName == roleName select r.RoleId).Count() > 0;
        }
    }
}
