using System.Collections.Generic;

namespace MyDiaryApp.Models.Vms
{
    public class CalendarViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<CalendarDay> CalendarDays { get; set; }
    }
}