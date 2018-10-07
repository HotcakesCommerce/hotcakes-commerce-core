using Moslem.Modules.GDSModuleMVC.Models;
using System;
using System.Collections.Generic;

namespace Moslem.Modules.GDS.GDSModuleMVC.Controllers
{
	public class SearchViewModel
	{
		public ResponsegetHotel responsegetHotel { get; set; }
		public List<RoomsesearchHotel> responsesearchHotel { get; set; }
        public GiftCard GiftCard { get; set; }
        public bool IsValidRetired { get; set; }
    }
}
