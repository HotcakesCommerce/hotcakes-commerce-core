using System;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class reqgetEarlyLateCapacity
	{
		public string sessionId { get; set; }
		public string checkin { get; set; }
		public int? night { get; set; }
		public int? hotelId { get; set; }
	}
}
