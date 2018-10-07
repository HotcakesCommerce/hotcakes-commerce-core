using System;
using System.Collections.Generic;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class ResponsesearchHotel
	{
		public int hotelId { get; set; }
		public List<ExtraRackRatesesearchHotel> extraRackRate { get; set; }
		public List<DetailExtraPricesesearchHotel> detailExtraPrice { get; set; }
		public int extraPrice { get; set; }
		public List<RoomsesearchHotel> room { get; set; }
	}
}
