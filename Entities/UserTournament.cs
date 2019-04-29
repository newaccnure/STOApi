
using System.Collections.Generic;

namespace STOApi.Entities
{
    public class UserTournament
    {
        public int UserId { set; get; }
        public User User { set; get; }
        public int TournamentId { set; get; }
        public Tournament Tournament { set; get; }
        public bool Joined { set; get; }
        
    }
}
