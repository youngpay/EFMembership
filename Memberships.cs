using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFMembership
{
    public class Memberships
    {
        [Key]
        public System.Guid UserId { get; set; }
        public string Password { get; set; }
        public int PasswordFormat { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<DateTime> LastLoginDate { get; set; }
        public Nullable<DateTime> LastPasswordChangedDate { get; set; }
        public Nullable<DateTime> LastLockoutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public Nullable<DateTime> FailedPasswordAttemptWindowStart { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public Nullable<DateTime> FailedPasswordAnswerAttemptWindowsStart { get; set; }
        public string Comment { get; set; }
    }
}
