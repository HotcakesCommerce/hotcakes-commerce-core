using System;
using System.Collections.Generic;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class RoomsesearchHotel
	{
		public int? roomId { get; set; }
		public string extraBed { get; set; }
		public int? breakfast { get; set; }
		public string capacity { get; set; }
		public int? rackRate { get; set; }
		public List<DetailFreeCapacitysesearchHotel> detailFreeCapacity { get; set; }
		public int? price { get; set; }
		public string freeCapacity { get; set; }
		public List<DetailPricesesearchHotel> detailPrice { get; set; }
	}
}
