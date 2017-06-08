#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hotcakes.Shipping.USPostal
{
    [Serializable]
    public class USPSServiceCode
    {
        public USPSServiceCode()
        {
            Id = -1;
            FriendlyName = "All Available Services";
            XmlName = "ALL";
            ClassId = -1;
            InternationalClassId = -1;
        }

        public int Id { get; set; }
        public string FriendlyName { get; set; }
        public string XmlName { get; set; }
        public int ClassId { get; set; }
        public int InternationalClassId { get; set; }

        public static List<USPSServiceCode> ListAll()
        {
            var result = new List<USPSServiceCode>
            {
                new USPSServiceCode {FriendlyName = "Global Express Guaranteed", ClassId = 10},
                new USPSServiceCode
                {
                    FriendlyName = "Global Express Guaranteed Non-Document Rectangular",
                    ClassId = 101
                },
                new USPSServiceCode
                {
                    FriendlyName = "Global Express Guaranteed Non-Document Non-Rectangular",
                    ClassId = 102
                },
                new USPSServiceCode {FriendlyName = "First-Class Mail International", ClassId = 100},
                // 64 max pounds
                new USPSServiceCode {FriendlyName = "Express Mail International (EMS)", ClassId = 9},
                new USPSServiceCode
                {
                    FriendlyName = "Express Mail International (EMS) Flat Rate Envelope",
                    ClassId = 109
                },
                new USPSServiceCode {FriendlyName = "Priority Mail International", ClassId = 11},
                new USPSServiceCode
                {
                    FriendlyName = "Priority Mail International Flat Rate Envelope",
                    ClassId = 110
                },
                new USPSServiceCode {FriendlyName = "Priority Mail International Flat Rate Box", ClassId = 111},
                new USPSServiceCode {FriendlyName = "Intl. Airmail Letter Post", ClassId = 50},
                new USPSServiceCode {FriendlyName = "Intl. Airmail Parcel Post", ClassId = 51},
                new USPSServiceCode {FriendlyName = "Intl. Economy Letter Post", ClassId = 52},
                new USPSServiceCode {FriendlyName = "Intl. Economy Parcel Post", ClassId = 53}
            };

            return result;
        }

        public static USPSServiceCode FindById(int id)
        {
            var x = ListAll().SingleOrDefault(y => y.Id == id);
            return x;
        }

        public static USPSServiceCode FindByClassId(int classId)
        {
            var x = ListAll().SingleOrDefault(y => y.ClassId == classId);
            return x;
        }

        public static USPSServiceCode FindByInternationalClassId(int classId)
        {
            var x = ListAll().SingleOrDefault(y => y.InternationalClassId == classId);
            return x;
        }

        public static USPSServiceCode FindByXmlName(string xmlName)
        {
            var x = ListAll().SingleOrDefault(y => y.XmlName == xmlName);
            return x;
        }
    }
}