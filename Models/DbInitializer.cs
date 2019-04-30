
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using STOApi.Models;
using STOApi.Entities;

namespace STOApi.Models
{
    public class DbInitializer
    {
        public static void Initialize(STOContext context)
        {
            if (context.Database.EnsureCreated())
            {
                int numberOfStaticUsers = 64;
                for (int i = 0; i < numberOfStaticUsers; ++i)
                {
                    string staticInfo = $"Participant{i}";
                    context.Users.Add(new User()
                    {
                        Email = staticInfo,
                        Password = staticInfo,
                        AboutMeInfo = staticInfo,
                        Role = "participant"
                    });
                }
                context.SaveChanges();
                context.Users.Add(new User()
                {
                    Email = "admin@nure.ua",
                    Password = "admin",
                    AboutMeInfo = "admin",
                    Role = "admin"
                });
                context.SaveChanges();
                context.EventFormats.Add(new EventFormat()
                {
                    Name = "Round-robin"
                });
                context.EventFormats.Add(new EventFormat()
                {
                    Name = "Single elimination"
                });
                context.EventFormats.Add(new EventFormat()
                {
                    Name = "Double elimination"
                });
                context.EventFormats.Add(new EventFormat()
                {
                    Name = "Group stage"
                });
                context.SaveChanges();
                context.Sports.Add(new Sport()
                {
                    Name = "Football"
                });
                context.Sports.Add(new Sport()
                {
                    Name = "Basketball"
                });
                context.Sports.Add(new Sport()
                {
                    Name = "Volleyball"
                });
                context.SaveChanges();
            }
        }
    }
}