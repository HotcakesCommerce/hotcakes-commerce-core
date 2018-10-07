using System;
using System.Collections.Generic;
using Moslem.Modules.GDS.GDSModuleMVC.Controllers;
namespace Moslem.Modules.GDS.GDSModuleMVC.Models
{
	public class VoucherModel
	{
		public PaymentModel PaymentModel { get; set; }
		public respgetReserveInfo ReserveInfo { get; set; }
		public List<ReservePassanger> lstPassangers { get; set; }
		public List<ReservedRoom> lstRooms { get; set; }
		public resfinalize finalize { get; set; }
		public resreserve RequestReserve { get; set; }
	}
}
