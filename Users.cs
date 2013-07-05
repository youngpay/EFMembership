using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFMembership
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public System.Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public Nullable<DateTime> LastActivityDate { get; set; }
    }
}
