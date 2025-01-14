using System;
using System.Collections.Generic;

namespace MyDiaryApp.Models
{
    public class DiaryEntry
    {
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    public class Transaction
    {
        public string Type { get; set; }    // 類型: 收入或支出
        public decimal Amount { get; set; } // 金額
        public string Description { get; set; } // 描述
    }
}