using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STOApi.Models
{
    public class DateRange
    {
        [Key]
        [ForeignKey("Tournament")]
        public int Id { set; get; }

        public DateTime Start { set; get; }
        public DateTime End { set; get; }
    }
}