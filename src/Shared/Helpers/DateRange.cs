using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Helpers
{
    public class DateRange
    {
        public static DateTime StartDate(int? TypeId = 1)
        {
            DateTime curDate = DateTime.Now;
            DateTime res = new DateTime();
            switch (TypeId)
            {
                case 1:
                    DateTime prevMonth = curDate.AddMonths(-1);
                    res = new DateTime(prevMonth.Year, prevMonth.Month, 1);
                    break;
                case 2:
                    res = new DateTime(curDate.Year, curDate.Month, 1);
                    break;
                case 3:
                    res = curDate.AddDays(-30);
                    break;
                default:
                    break;
            }
            return res;
        }

        public static DateTime EndDate(int? TypeId = 1)
        {
            DateTime curDate = DateTime.Now;
            DateTime res = new DateTime();
            switch (TypeId)
            {
                case 1:
                    DateTime prevMonth = curDate.AddMonths(-1);
                    res = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month), 23, 59, 59);
                    break;
                case 2:
                    res = curDate;
                    break;
                case 3:
                    res = curDate;
                    break;
                default:
                    break;
            }
            return res;
        }
    }
}
