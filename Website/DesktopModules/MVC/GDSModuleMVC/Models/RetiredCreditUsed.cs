using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moslem.Modules.GDSModuleMVC.Models
{
    public class RetiredCreditUsed
    {
        public int ID { get; set; }
        public string NationalCode { get; set; }
        public string DaftarNumber { get; set; }
        public DateTime DateOfUsed { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool Finalized { get; set; }
        public bool HasPartner { get; set; }
        public int ReserveID { get; set; }
    }
}
