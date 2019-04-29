using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STOApi.Entities
{
    public class DateRange
    {
        public DateTime Start { set; get; }
        public DateTime End { set; get; }
    }
}