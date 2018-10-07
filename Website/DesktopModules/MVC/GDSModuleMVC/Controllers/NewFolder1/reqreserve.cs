using System;
using System.Collections.Generic;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class reqreserve
	{
		public string sessionId { get; set; }
		public List<Passengerreserve> passenger { get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }
		public string email { get; set; }
		public string tel { get; set; }
		public string mobile { get; set; }
		public int hotelId { get; set; }
		public string checkin { get; set; }
		public int night { get; set; }
	}
}
