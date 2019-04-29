using System.Collections.Generic;

namespace STOApi.Entities
{
    public class EventFormat
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public List<Tournament> Tournaments { set; get; }
    }
}