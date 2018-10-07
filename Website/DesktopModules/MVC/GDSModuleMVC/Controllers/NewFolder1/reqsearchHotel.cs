using System;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class reqsearchHotel
	{
		public string sessionId { get; set; }
		public string checkin { get; set; }
		public int nights { get; set; }
		public int hotelId { get; set; }
		public int cityId { get; set; }
		public int rate { get; set; }
		public int capacityId { get; set; }
		public int capacity { get; set; }
		public int person { get; set; }
		public int commission { get; set; }
	}
}
