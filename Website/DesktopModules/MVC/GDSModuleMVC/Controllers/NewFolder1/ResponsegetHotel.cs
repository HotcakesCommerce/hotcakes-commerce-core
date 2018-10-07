using System;
using System.Collections.Generic;
namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class ResponsegetHotel
	{
		public string id { get; set; }
		public string description { get; set; }
		public string typeCode { get; set; }
		public string type { get; set; }
		public string rate { get; set; }
		public string city { get; set; }
		public string province { get; set; }
		public string address1 { get; set; }
		public string pointOnMap { get; set; }
		public string telForPassenger { get; set; }
		public string parkingIndoor { get; set; }
		public string parkingOutdoor { get; set; }
		public string parkingGuarantee { get; set; }
		public string poolAllYear { get; set; }
		public string poolSeasonal { get; set; }
		public string poolIndoor { get; set; }
		public string poolOutdoor { get; set; }
		public string poolMen { get; set; }
		public string poolWomen { get; set; }
		public string poolChildren { get; set; }
		public string poolSuana { get; set; }
		public string poolJacuzzi { get; set; }
		public string poolFitness { get; set; }
		public string poolMassage { get; set; }
		public string poolHotSpring { get; set; }
		public string poolComment { get; set; }
		public string disabledAccessibility { get; set; }
		public string lift { get; set; }
		public string restaurantBreakfastArea { get; set; }
		public string restaurantBreakfastInRoom { get; set; }
		public string restaurantRoofGarden { get; set; }
		public string restaurantSelfService { get; set; }
		public string restaurantRoomService { get; set; }
		public string carRental { get; set; }
		public string airportService { get; set; }
		public string laundry { get; set; }
		public string otherFacilities { get; set; }
		public List<RoomgetHotel> room { get; set; }
		public List<gethotelImage> images { get; set; }
	}
}
