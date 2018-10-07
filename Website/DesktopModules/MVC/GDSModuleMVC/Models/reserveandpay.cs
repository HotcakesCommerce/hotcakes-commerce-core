using System;
using System.Collections.Generic;
namespace Moslem.Modules.GDS.GDSModuleMVC.Models
{
	public class reserveandpay
	{
		public string firstName { get; set; }
		public string lastname { get; set; }
		public string email { get; set; }
		public string roomCount { get; set; }
        public string tel { get; set; }
		public string mobile { get; set; }
		public string hotelId { get; set; }
		public string checkin { get; set; }
		public long idno { get; set; }
		public string night { get; set; }
		public string discritption { get; set; }
		public string RoomCapacities { get; set; }
		public string roomid { get; set; }
		public string Description { get; set; }
		public decimal amount { get; set; }
        public bool ValidRetired { get; set; }
        public bool Retired { get; set; }
        public bool hasPartner { get; set; }
        public decimal AmountAfterDiscount { get; set; }
        public long DaftarNumber { get; set; }

        public List<Passenger> Passengers { get; set; }
	}
}
