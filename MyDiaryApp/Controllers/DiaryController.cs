using MyDiaryApp.Models;
using MyDiaryApp.Models.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MyDiaryApp.Controllers
{
    public class DiaryController : Controller
    {
        #region 取得月曆
        // GET: Diary
        public ActionResult GetCalendar(int? year, int? month)
        {
            DateTime currentDate = DateTime.Now;
            int selectedYear = year ?? currentDate.Year;
            int selectedMonth = month ?? currentDate.Month;

            if (selectedMonth < 1)
            {
                selectedMonth = 12;
                selectedYear--;
            }
            else if (selectedMonth > 12)
            {
                selectedMonth = 1;
                selectedYear++;
            }

            var calendarModel = GetMonthCalendar(selectedYear, selectedMonth);

            var vm = new CalendarViewModel
            {
                Year = selectedYear,
                Month = selectedMonth,
                CalendarDays = calendarModel
            };
            return View(vm);
        }

        [HttpGet]
        public ActionResult GetPartialCalendar(int year, int month)
        {

            if (month < 1)
            {
                month = 12;
                year--;
            }
            else if (month > 12)
            {
                month = 1;
                year++;
            }

            var calendarModel = GetMonthCalendar(year, month);

            var vm = new CalendarViewModel
            {
                Year = year,
                Month = month,
                CalendarDays = calendarModel
            };
            return PartialView("_CalendarPartial", vm);
        }
        #endregion

        public ActionResult GetDiary(string date = "2025-01-01")
        {
            if (!DateTime.TryParse(date, out DateTime diaryDate))
            {
                return HttpNotFound("無效的日期");
            }

            DiaryEntry diaryEntry = GetDiaryEntry(diaryDate);

            if (diaryEntry == null)
            {
                diaryEntry = new DiaryEntry
                {
                    Date = diaryDate,
                    Content = "",
                    Transactions = new List<Transaction>()
                };
            }

            return View(diaryEntry);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SaveDiary([System.Web.Http.FromBody] DiaryEntry diaryEntry)
        {
            if (diaryEntry == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "無效的請求");
            }

            var entries = LoadDiaryEntries();
            var existedEntry = entries.FirstOrDefault(e => e.Date.Date == diaryEntry.Date.Date);

            if (existedEntry != null)//如果已存在內容就更新
            {
                existedEntry.Content = diaryEntry.Content;
                existedEntry.Transactions = diaryEntry.Transactions;
            }
            else
            {
                entries.Add(diaryEntry);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        private string DiaryFilePath => Server.MapPath("~/App_Data/diary.json");

        private List<DiaryEntry> LoadDiaryEntries()
        {
            if (!System.IO.File.Exists(DiaryFilePath))
            {
                return new List<DiaryEntry>();
            }

            var json = System.IO.File.ReadAllText(DiaryFilePath, System.Text.Encoding.UTF8);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<DiaryEntry>>(json);
        }

        private DiaryEntry GetDiaryEntry(DateTime diaryDate)
        {
            var entries = LoadDiaryEntries();
            return entries.FirstOrDefault(e => e.Date.Date == diaryDate.Date);
        }

        private void SaveDiaryEntry(List<DiaryEntry> diaryEntries)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(diaryEntries);
            System.IO.File.WriteAllText(DiaryFilePath, json, System.Text.Encoding.UTF8);
        }



        #region 生成月曆功能
        private List<CalendarDay> GetMonthCalendar(int year, int month)
        {
            var calendarDays = new List<CalendarDay>();
            DateTime fstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = fstDayOfMonth.AddMonths(1).AddDays(-1);

            int startDayOfWeek = (int)fstDayOfMonth.DayOfWeek;

            for (int i = 0; i < startDayOfWeek; i++)
            {
                calendarDays.Add(new CalendarDay { Date = null, IsCurrentMonth = false });
            }

            for (int day = 1; day <= lastDayOfMonth.Day; day++)
            {
                calendarDays.Add(new CalendarDay
                {
                    Date = new DateTime(year, month, day),
                    IsCurrentMonth = true
                });
            }

            while (calendarDays.Count % 7 != 0)
            {
                calendarDays.Add(new CalendarDay { Date = null, IsCurrentMonth = false });
            }

            return calendarDays;
        }
        #endregion


    }
}