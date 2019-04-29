
using System.Collections.Generic;

namespace STOApi.Entities
{
    public class Tournament
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public Schedule Schedule { set; get; }
        public int EventFormatId { set; get; }
        public EventFormat EventFormat { set; get; }
        public int SportId { set; get; }
        public Sport Sport { set; get; }
        public List<UserTournament> UserTournaments { set; get; }
        
        public Tournament()
        {
            UserTournaments = new List<UserTournament>();
        }
    }
}
