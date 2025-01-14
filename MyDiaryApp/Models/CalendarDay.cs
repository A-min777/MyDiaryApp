using System;

namespace MyDiaryApp.Models
{
    public class CalendarDay
    {
        public DateTime? Date { get; set; }
        public bool IsCurrentMonth { get; set; }
    }
}