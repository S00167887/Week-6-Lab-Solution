namespace Week6.Club.DataDomain.Migrations
{
    using CsvHelper;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<Week6.Club.DataDomain.ClubContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Week6.Club.DataDomain.ClubContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            seed_Db_students(context);
            seed_Db_model(context);
            seed_Db_ClubMembers(context);
            // Seeding of Club admin and Member roles will be done in the other Applictaion DB context
        }

        private void seed_Db_ClubMembers(ClubContext context)
        {
            Club firstClub = context.Clubs.First();
            // Make an array of members from the first 10 selected students
            Member[] NewMembers = context.Students
                .Take(10).ToList()
                .Select(s => // Making the Member output
                new Member { StudentID =s.StudentID,
                    approved = false,
                    AssociatedClub = firstClub.ClubId  })
                .ToArray() ;
            context.ClubMembers.AddOrUpdate(c => c.StudentID,
                NewMembers);
        }

        private void seed_Db_students(ClubContext context)
        {
            List<Student> students = new List<Student>();
            Assembly assembly = Assembly.GetExecutingAssembly();

            string resourceName = "Week6.Club.DataDomain.StudentList1.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    CsvReader csvReader = new CsvReader(reader);
                    csvReader.Configuration.HasHeaderRecord = false;
                    csvReader.Configuration.MissingFieldFound = null;
                    students = csvReader.GetRecords<Student>().ToList();
                    foreach (var item in students)
                    {
                        context.Students.AddOrUpdate(item);
                    }
                }
            }
            context.SaveChanges();
        }

        private void seed_Db_model(ClubContext context)
        {
            CultureInfo cultureinfo = CultureInfo.CreateSpecificCulture("en-IE");
            context.Clubs.AddOrUpdate(new Club[]
            {
                new Club{ ClubName="The Chess Club",
                    CreationDate = DateTime.ParseExact("25/01/2017","dd/mm/yyyy",cultureinfo),
                }, // End of Club
            } // End of Clubs
            );
            context.SaveChanges();

        }
    }
}
