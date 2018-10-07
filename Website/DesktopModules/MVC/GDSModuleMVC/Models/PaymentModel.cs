using System;
namespace Moslem.Modules.GDS.GDSModuleMVC.Models
{
	public class PaymentModel
	{
		public bool Successful { get; set; }
		public string Amount { get; set; }
		public string Message { get; set; }
		public string State { get; set; }
		public string StateCode { get; set; }
		public string ResNum { get; set; }
		public string MID { get; set; }
		public string RefNum { get; set; }
		public string CID { get; set; }
		public string TRACENO { get; set; }
		public string RRN { get; set; }
		public string SecurePan { get; set; }
		public PaymentModel()
		{
			this.Successful = false;
			this.Amount = string.Empty;
			this.Message = string.Empty;
			this.State = string.Empty;
			this.StateCode = string.Empty;
			this.ResNum = string.Empty;
			this.MID = string.Empty;
			this.RefNum = string.Empty;
			this.CID = string.Empty;
			this.TRACENO = string.Empty;
			this.RRN = string.Empty;
			this.SecurePan = string.Empty;
		}
	}
}
