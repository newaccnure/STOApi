using System.Collections.Generic;

namespace STOApi.Entities
{
    public class User
    {
        public int Id { set; get; }
        public string Email { set; get; }
        public string Role { set; get; }
        public string Password { set; get; }
        public byte[] Image { set; get; }
        public List<UserTournament> UserTournaments { set; get; }

        //json version of List<User>
        public string RepresentativeParticipants { set; get; }
        //json version of List<User>
        public string ParticipantRepresentatives { set; get; }

        public User()
        {
            UserTournaments = new List<UserTournament>();
        }
    }
}
