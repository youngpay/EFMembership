using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace EFMembership
{
    public class EFMembershipProvider : MembershipProvider
    {
        #region Constants and Fields

        private EFMembershipDbContext db;

        /// <summary>
        /// The application name.
        /// </summary>
        private string applicationName;

        /// <summary>
        /// The conn string name.
        /// </summary>
        private string connStringName;

        /// <summary>
        /// The parm prefix.
        /// </summary>
        private string parmPrefix;

        /// <summary>
        /// The password format.
        /// </summary>
        private MembershipPasswordFormat passwordFormat;

        /// <summary>
        /// The table prefix.
        /// </summary>
        private string tablePrefix;

        #endregion

        #region Properties

        public string ConnectionStringName
        {
            get
            {
                return this.connStringName;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///     Returns the application name as set in the web.config
        ///     otherwise returns BlogEngine.  Set will throw an error.
        /// </summary>
        public override string ApplicationName
        {
            get
            {
                return this.applicationName;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///     Hardcoded to false
        /// </summary>
        public override bool EnablePasswordReset
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Can password be retrieved via email?
        /// </summary>
        public override bool EnablePasswordRetrieval
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Hardcoded to 5
        /// </summary>
        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        ///     Hardcoded to 0
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        ///     Hardcoded to 4
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        ///     Not implemented
        /// </summary>
        public override int PasswordAttemptWindow
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Password format (Clear or Hashed)
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return this.passwordFormat;
            }
        }

        /// <summary>
        ///     Not Implemented
        /// </summary>
        public override string PasswordStrengthRegularExpression
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        ///     Hardcoded to false
        /// </summary>
        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///     Hardcoded to false
        /// </summary>
        public override bool RequiresUniqueEmail
        {
            get
            {
                return false;
            }
        }

        #endregion

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "DbMembershipProvider";
            }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Generic Database Membership Provider");
            }

            base.Initialize(name, config);

            // Connection String
            if (config["connectionStringName"] == null)
            {
                config["connectionStringName"] = "DefalutCollection";
            }



            this.connStringName = config["connectionStringName"];
            config.Remove("connectionStringName");

            // Table Prefix
            if (config["tablePrefix"] == null)
            {
                config["tablePrefix"] = "efm_";
            }

            this.tablePrefix = config["tablePrefix"];
            config.Remove("tablePrefix");

            // Parameter character
            if (config["parmPrefix"] == null)
            {
                config["parmPrefix"] = "@";
            }

            this.parmPrefix = config["parmPrefix"];
            config.Remove("parmPrefix");

            // Application Name
            if (config["applicationName"] == null)
            {
                config["applicationName"] = "/";
            }

            this.applicationName = config["applicationName"];
            config.Remove("applicationName");

            // Password Format
            if (config["passwordFormat"] == null)
            {
                config["passwordFormat"] = "Hashed";
                this.passwordFormat = MembershipPasswordFormat.Hashed;
            }
            else if (string.Compare(config["passwordFormat"], "clear", true) == 0)
            {
                this.passwordFormat = MembershipPasswordFormat.Clear;
            }
            else
            {
                this.passwordFormat = MembershipPasswordFormat.Hashed;
            }

            config.Remove("passwordFormat");

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                var attr = config.GetKey(0);
                if (!string.IsNullOrEmpty(attr))
                {
                    throw new Exception(string.Format("Unrecognized attribute: {0}", attr));
                }
            }
            db = new EFMembershipDbContext(Utils.GetConfigDataBaseName(connStringName));
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            if (oldPassword == null)
                throw new ArgumentNullException("oldPassword");

            if (newPassword == null)
                throw new ArgumentNullException("newPassword");


            oldPassword = passwordFormat == MembershipPasswordFormat.Hashed ? Utils.HashPassword(oldPassword) : oldPassword;
            newPassword = passwordFormat == MembershipPasswordFormat.Hashed ? Utils.HashPassword(newPassword) : oldPassword;

            var userquery = from u in db.Users
                        where u.UserName == username
                        select u;
            var user = userquery.FirstOrDefault();
            if (user == null)
            {
                return false;
            }

            var mquery = from m in db.Memberships
                    where m.UserId == user.UserId && m.Password == oldPassword
                    select m;

            var membership = mquery.FirstOrDefault();
            if (membership == null)
            {
                return false;
            }

            membership.Password = newPassword;
            db.SaveChanges();
            return true;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            if (password == null)
                throw new ArgumentNullException("password");


            var dbuser = new Users { 
                UserName = username, 
                LastActivityDate = DateTime.Now
            };
            dbuser = db.Users.Add(dbuser);
            db.SaveChanges();

            var dbmebr = new Memberships { 
                UserId = dbuser.UserId,
                Email = email,
                Password = passwordFormat == MembershipPasswordFormat.Hashed ? Utils.HashPassword(password) : password,
                PasswordQuestion = passwordQuestion,
                PasswordAnswer = passwordAnswer,
                IsApproved = isApproved,
                PasswordFormat = (int)passwordFormat,
                CreateDate = DateTime.Now
            };
            db.Memberships.Add(dbmebr);
            db.SaveChanges();

            var user = this.GetMembershipUser(username, email, DateTime.Now);
            status = MembershipCreateStatus.Success;
            return user;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (username == null)
                throw new ArgumentNullException("username");



            var query = from u in db.Users where u.UserName == username select u;
            var user = query.FirstOrDefault();
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            if (emailToMatch == null)
                throw new ArgumentNullException("emailToMatch");



            var coll = new MembershipUserCollection();
            var query = from u in db.Users
                        from m in db.Memberships
                        where
                        u.UserId == m.UserId &&
                        m.Email.Contains(emailToMatch)
                        select new { UserName = u.UserName, Email = m.Email, LastLogin = m.LastLoginDate };
            var count = query.Count();
            var list = query.OrderBy(m => m.UserName).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            if (list != null)
            {
                foreach (var item in list)
                {
                    coll.Add(this.GetMembershipUser(item.UserName, item.Email, item.LastLogin.GetValueOrDefault()));
                }
            }
            totalRecords = count;
            return coll;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            if (usernameToMatch == null)
                throw new ArgumentNullException("usernameToMatch");



            var coll = new MembershipUserCollection();
            var query = from u in db.Users
                        from m in db.Memberships
                        where
                        u.UserId == m.UserId &&
                        u.UserName.Contains(usernameToMatch)
                        select new { UserName = u.UserName, Email = m.Email, LastLogin = m.LastLoginDate };
            var count = query.Count();
            var list = query.OrderBy(m => m.UserName).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            if (list != null)
            {
                foreach (var item in list)
                {
                    coll.Add(this.GetMembershipUser(item.UserName, item.Email, item.LastLogin.GetValueOrDefault()));
                }
            }
            totalRecords = count;
            return coll;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var coll = new MembershipUserCollection();
            var query = from u in db.Users
                        from m in db.Memberships
                        where
                        u.UserId == m.UserId
                        select new { UserName = u.UserName, Email = m.Email, LastLogin = m.LastLoginDate };
            var count = query.Count();
            var list = query.OrderBy(m => m.UserName).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            if (list != null)
            {
                foreach (var item in list)
                {
                    coll.Add(this.GetMembershipUser(item.UserName, item.Email, item.LastLogin.GetValueOrDefault()));
                }
            }
            totalRecords = count;
            return coll;
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (username == null)
                throw new ArgumentNullException("username");



            var query = from u in db.Users
                        from m in db.Memberships
                        where
                        u.UserId == m.UserId && u.UserName == username
                        select new { UserName = u.UserName, Email = m.Email, LastLogin = m.LastLoginDate };
            var user = query.FirstOrDefault();
            if (user != null)
            {
                return this.GetMembershipUser(user.UserName, user.Email, user.LastLogin.GetValueOrDefault());
            }
            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
                throw new ArgumentNullException("providerUserKey");



            return this.GetUser(providerUserKey.ToString(), userIsOnline);
        }

        public override string GetUserNameByEmail(string email)
        {
            if (email == null)
                throw new ArgumentNullException("email");


            var query = from m in db.Memberships
                        from u in db.Users
                        where m.UserId == u.UserId &&
                        m.Email == email
                        select u;
            var user = query.FirstOrDefault();
            if (user != null)
            {
                return user.UserName;
            }
            return null;
        }
        public override string ResetPassword(string username, string answer)
        {
            if (username == null)
                throw new ArgumentNullException("username");


            var newPass = Utils.RandomPassword();
            var query = from u in db.Users
                        from m in db.Memberships
                        where m.UserId == u.UserId &&
                        u.UserName == username
                        select m;
            var membership = query.FirstOrDefault();
            if (membership != null)
            {
                membership.Password = passwordFormat == MembershipPasswordFormat.Hashed ? Utils.HashPassword(newPass) : newPass;
                db.SaveChanges();
                return newPass;
            }
            return null;
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            if (username == null)
                throw new ArgumentNullException("username");


            if (password == null)
                throw new ArgumentNullException("password");


            var pass = passwordFormat == MembershipPasswordFormat.Hashed ? Utils.HashPassword(password) : password;
            var query = from u in db.Users
                        from m in db.Memberships
                        where u.UserId == m.UserId &&
                        u.UserName == username &&
                        m.Password == pass
                        select u;

            return query.FirstOrDefault() != null;
        }

        private MembershipUser GetMembershipUser(string userName, string email, DateTime lastLogin)
        {
            var user = new MembershipUser(
                this.Name, // Provider name
                userName, // Username
                userName, // providerUserKey
                email, // Email
                string.Empty, // passwordQuestion
                string.Empty, // Comment
                true, // approved
                false, // isLockedOut
                DateTime.Now, // creationDate
                lastLogin, // lastLoginDate
                DateTime.Now, // lastActivityDate
                DateTime.Now, // lastPasswordChangedDate
                new DateTime(1980, 1, 1)); // lastLockoutDate
            return user;
        }
    }
}
