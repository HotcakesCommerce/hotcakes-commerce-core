using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moslem.Modules.GDSModuleMVC.Models
{
    public class GiftCard
    {
        public string PassKey { get; set; }
        public bool Status { get; set; }
        public List<string> RoomPackages { get; set; }
    }
}