
using System.Collections.Generic;

namespace STOApi.Models
{
    public class Tournament
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public Sport Sport { set; get; }
        public EventFormat Event { set; get; }
        public string Password { set; get; }
        public DateRange DateRange { set; get; }
        public List<User> Organizers { set; get; }
    }
}
