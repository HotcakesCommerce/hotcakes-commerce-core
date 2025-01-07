using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    [DataContract]
    public enum UpchargeAmountTypesDTO
    {
    
        [EnumMember] Percent = 1,
       
        [EnumMember] Amount = 0,
    }
}
