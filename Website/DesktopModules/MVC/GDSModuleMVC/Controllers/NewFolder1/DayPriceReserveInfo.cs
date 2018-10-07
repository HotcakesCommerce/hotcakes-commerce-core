using System;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class DayPriceReserveInfo
	{
		public string date { get; set; }
		public string canNotBeCancel { get; set; }
		public int price { get; set; }
		public int extraBedPrice { get; set; }
		public int infantPrice { get; set; }
		public int childPrice { get; set; }
		public int rackRate { get; set; }
		public int extraRackRate { get; set; }
		public int infantRackRate { get; set; }
		public int childRackRate { get; set; }
		public int hotelPrice { get; set; }
		public int hotelExtraBedPrice { get; set; }
		public int hotelInfantPrice { get; set; }
		public int hotelChildPrice { get; set; }
	}
}
