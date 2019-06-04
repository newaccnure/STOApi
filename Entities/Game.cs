
using System.Collections.Generic;

namespace STOApi.Entities
{
    public class Game
    {
        public int Id { set; get; }
        public DateRange GameSchedule { set; get; }
        public User FirstParticipant { set; get; }
        public User SecondParticipant { set; get; }
        public User Winner { set; get; }
        public Score Score { set; get; }
        public int GroupNumber { set; get; }

        public Schedule Schedule { get; set; }
        public int ScheduleId { get; set; }
    }
}
