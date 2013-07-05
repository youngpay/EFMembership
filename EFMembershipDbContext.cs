using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFMembership
{
    public class EFMembershipDbContext : DbContext
    {
        public EFMembershipDbContext() { }
        public EFMembershipDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
        public DbSet<Memberships> Memberships { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UsersInRoles> UsersInRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EFMembershipDbContext>());
        }
    }
}
