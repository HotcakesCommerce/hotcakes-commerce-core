using System;
using System.Collections.Generic;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class PassengerReserveInfo
	{
		public int rank { get; set; }
		public string name { get; set; }
		public string family { get; set; }
		public string extraBedNo { get; set; }
		public string description { get; set; }
		public string roomId { get; set; }
		public string roomName { get; set; }
		public string checkin { get; set; }
		public string night { get; set; }
		public string infantNo { get; set; }
		public string childNo { get; set; }
		public int earlyCheckin { get; set; }
		public int lateCheckout { get; set; }
		public List<DayPriceReserveInfo> dayPrice { get; set; }
		public List<object> halfDayPrice { get; set; }
	}
}
