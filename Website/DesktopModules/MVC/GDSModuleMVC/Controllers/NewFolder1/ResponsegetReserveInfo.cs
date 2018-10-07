using System;
using System.Collections.Generic;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class ResponsegetReserveInfo
	{
		public string id { get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }
		public string email { get; set; }
		public string tel { get; set; }
		public string mobile { get; set; }
		public string agency { get; set; }
		public string agencyTelForPassenger { get; set; }
		public string hotelId { get; set; }
		public string hotel { get; set; }
		public string hotelTelForPassenger { get; set; }
		public string hotelSmoking { get; set; }
		public string hotelPetsAllow { get; set; }
		public string hotelCheckInTime { get; set; }
		public string hotelCheckOutTime { get; set; }
		public string hotelGeneralRule { get; set; }
		public List<PassengerReserveInfo> passenger { get; set; }
	}
}
