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

namespace Hotcakes.Shipping.USPostal.v4
{
    [Serializable]
    internal class DomesticPackageServiceResponse
    {
        public DomesticPackageServiceResponse()
        {
            XmlName = string.Empty;
            XmlClassId = string.Empty;
            ServiceType = DomesticServiceType.All;
        }

        public string XmlName { get; set; }
        public string XmlClassId { get; set; }
        public DomesticServiceType ServiceType { get; set; }

        public static List<DomesticPackageServiceResponse> FindAll()
        {
            var result = new List<DomesticPackageServiceResponse>
            {
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "0",
                    XmlName = "First-Class",
                    ServiceType = DomesticServiceType.FirstClass
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "3",
                    XmlName = "Express Mail",
                    ServiceType = DomesticServiceType.ExpressMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "13",
                    XmlName = "Express Mail Flat-Rate Envelope",
                    ServiceType = DomesticServiceType.ExpressMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "30",
                    XmlName = "Express Mail Legal Flat-Rate Envelope",
                    ServiceType = DomesticServiceType.ExpressMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "23",
                    XmlName = "Express Mail Sunday/Holiday",
                    ServiceType = DomesticServiceType.ExpressMailSundayHoliday
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "25",
                    XmlName = "Express Mail Flat-Rate Envelope Sunday/Holiday",
                    ServiceType = DomesticServiceType.ExpressMailSundayHoliday
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "32",
                    XmlName = "Express Mail Legal Flat-Rate Envelope Sunday/Holiday",
                    ServiceType = DomesticServiceType.ExpressMailSundayHoliday
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "2",
                    XmlName = "Express Mail Hold for Pickup",
                    ServiceType = DomesticServiceType.ExpressMailHoldForPickup
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "27",
                    XmlName = "Express Mail Flat-Rate Envelope Hold For Pickup",
                    ServiceType = DomesticServiceType.ExpressMailHoldForPickup
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "31",
                    XmlName = "Express Mail Legal Flat-Rate Envelope Hold For Pickup",
                    ServiceType = DomesticServiceType.ExpressMailHoldForPickup
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "1",
                    XmlName = "Priority Mail",
                    ServiceType = DomesticServiceType.PriorityMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "16",
                    XmlName = "Priority Mail Flat-Rate Envelope",
                    ServiceType = DomesticServiceType.PriorityMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "28",
                    XmlName = "Priority Mail Small Flat-Rate Box",
                    ServiceType = DomesticServiceType.PriorityMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "17",
                    XmlName = "Priority Mail Regular Flat-Rate Box",
                    ServiceType = DomesticServiceType.PriorityMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "22",
                    XmlName = "Priority Mail Flat-Rate Large Box",
                    ServiceType = DomesticServiceType.PriorityMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "40",
                    XmlName = "Priority Mail Window Flat-Rate Envelope",
                    ServiceType = DomesticServiceType.PriorityMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "44",
                    XmlName = "Priority Mail Legal Flat-Rate Envelope",
                    ServiceType = DomesticServiceType.PriorityMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "4",
                    XmlName = "Parcel Post",
                    ServiceType = DomesticServiceType.ParcelPost
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "6",
                    XmlName = "Media Mail",
                    ServiceType = DomesticServiceType.MediaMail
                },
                new DomesticPackageServiceResponse
                {
                    XmlClassId = "7",
                    XmlName = "Library",
                    ServiceType = DomesticServiceType.LibraryMaterial
                }
            };

            // First Class
            //result.Add(new DomesticPackageServiceResponse() { XmlClassId = "19", XmlName = "First-Class", ServiceType = DomesticServiceType.FirstClass }); 
            //result.Add(new DomesticPackageServiceResponse() { XmlClassId = "12", XmlName = "First-Class Postcard Stamped", ServiceType = DomesticServiceType.FirstClass });
            //result.Add(new DomesticPackageServiceResponse() { XmlClassId = "15", XmlName = "First-Class Large Postcards", ServiceType = DomesticServiceType.FirstClass });
            
            // Priority Mail
            //result.Add(new DomesticPackageServiceResponse() { XmlClassId = "18", XmlName = "Priority Mail Keys and IDs", ServiceType = DomesticServiceType.PriorityMail });

            return result;
        }
    }
}