
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Week6.Club.DataDomain
{
    public class ClubContext : DbContext
    {
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Member> ClubMembers { get; set; }
        public DbSet<Student> Students { get; set; }


        public ClubContext():base(nameOrConnectionString: "Week6Connection")
        {
            
        }

        public static ClubContext Create()
        {
            return new ClubContext();
        }
    }
}
