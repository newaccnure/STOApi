
using System;
using System.Collections.Generic;

namespace STOApi.Entities
{
    public class Schedule
    {
        public int Id { set; get; }
        public DateRange TournamentSchedule { set; get; }
        public TimeSpan GameDayStart { set; get; }
        public TimeSpan GameDayEnd { set; get; }
        public int BreakTime { set; get; }
        public int GameTime { set; get; }
        public List<Game> Games { set; get; }
        public int TournamentId { set; get; }
        public Tournament Tournament { set; get; }
    }
}
