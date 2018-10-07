using System;
using System.Globalization;
namespace Extensions
{
	public static class PersianDate
	{
		public static DateTime ToGeorgianDateTime(this string persianDate)
		{
			int year = Convert.ToInt32(persianDate.Substring(0, 4));
			int month = Convert.ToInt32(persianDate.Substring(5, 2));
			int day = Convert.ToInt32(persianDate.Substring(8, 2));
			DateTime result = new DateTime(year, month, day, new PersianCalendar());
			return result;
		}
		public static string ToPersianDateString(this DateTime georgianDate)
		{
			PersianCalendar persianCalendar = new PersianCalendar();
			string arg = persianCalendar.GetYear(georgianDate).ToString();
			string arg2 = persianCalendar.GetMonth(georgianDate).ToString().PadLeft(2, '0');
			string arg3 = persianCalendar.GetDayOfMonth(georgianDate).ToString().PadLeft(2, '0');
			return string.Format("{0}/{1}/{2}", arg, arg2, arg3);
		}
		public static string AddDaysToShamsiDate(this string persianDate, int days)
		{
			DateTime georgianDate = persianDate.ToGeorgianDateTime().AddDays((double)days);
			return georgianDate.ToPersianDateString();
		}
	}
}
