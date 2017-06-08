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
using System.Globalization;
using System.Linq;
using System.Xml;
using Hotcakes.Web;

namespace Hotcakes.Shipping.USPostal.v4
{
    [Serializable]
    public class DomesticPackage
    {
        public DomesticPackage()
        {
            Init();
        }

        public DomesticPackage(XmlNode n)
        {
            Init();
            ParseNode(n);
        }

        public string Id { get; set; }
        public DomesticServiceType Service { get; set; }
        public string ZipOrigination { get; set; }
        public string ZipDestination { get; set; }
        public int Pounds { get; set; }
        public decimal Ounces { get; set; }
        public DomesticPackageType Container { get; set; }

        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }

        public decimal DeclaredValue { get; set; }
        public decimal AmountToCollect { get; set; }
        public string Zone { get; set; }
        public List<DomesticPostage> Postages { get; set; }
        public List<DomesticSpecialServiceType> SpecialServices { get; set; }

        private void Init()
        {
            Id = "0";
            Service = DomesticServiceType.All;
            ZipOrigination = string.Empty;
            ZipDestination = string.Empty;
            Pounds = 0;
            Ounces = 0;
            Container = DomesticPackageType.Ignore;
            DeclaredValue = 0m;
            AmountToCollect = 0m;
            Zone = string.Empty;
            Postages = new List<DomesticPostage>();
            SpecialServices = new List<DomesticSpecialServiceType>();
        }

        private void SortDimensions()
        {
            var dimensions = new List<decimal> {Length, Width, Height};

            var sorted = dimensions.OrderByDescending(d => d).ToList();
            Length = sorted[0];
            Width = sorted[1];
            Height = sorted[2];
        }

        public decimal Girth()
        {
            if (HasDimensions())
            {
                return 2*Width + 2*Height;
            }
            return 1;
        }

        public DomesticPackageSize DeterminePackageSize()
        {
            if (Length > 12 || Width > 12 || Height > 12)
            {
                return DomesticPackageSize.Large;
            }
            if (Container == DomesticPackageType.NonRectangular)
            {
                return DomesticPackageSize.Large;
            }
            return DomesticPackageSize.Regular;
        }

        public bool HasDimensions()
        {
            return Length > 0 &&
                   Height > 0 &&
                   Width > 0;
        }

        public DomesticMachinable IsMachinable()
        {
            var required = false;

            if (Service == DomesticServiceType.FirstClass ||
                Service == DomesticServiceType.FirstClassHoldForPickupCommercial)
            {
                if (Container == DomesticPackageType.FirstClassFlat ||
                    Container == DomesticPackageType.FirstClassLetter)
                {
                    required = true;
                }
            }
            if (Service == DomesticServiceType.ParcelPost ||
                Service == DomesticServiceType.All ||
                Service == DomesticServiceType.Online)
            {
                required = true;
            }

            if (required)
            {
                if (Pounds < 1 && Ounces < 6) return DomesticMachinable.No;
                if (Pounds > 35) return DomesticMachinable.No;
                if (HasDimensions())
                {
                    if (Length > 34) return DomesticMachinable.No;
                    if (Width > 17 || Height > 17) return DomesticMachinable.No;
                    if (Container == DomesticPackageType.FirstClassParcel && Length < 6) return DomesticMachinable.No;
                    if (Container == DomesticPackageType.FirstClassParcel && Height < 3) return DomesticMachinable.No;
                }
                return DomesticMachinable.Yes;
            }

            return DomesticMachinable.Ignored;
        }

        public void ParseNode(XmlNode n)
        {
            if (n == null) return;

            if (n.Attributes.GetNamedItem("ID") != null)
            {
                Id = n.Attributes.GetNamedItem("ID").InnerText;
            }
            ZipOrigination = Xml.ParseInnerText(n, "ZipOrigination");
            Pounds = Xml.ParseInteger(n, "Pounds");
            Ounces = Xml.ParseInteger(n, "Ounces");
            Zone = Xml.ParseInnerText(n, "Zone");
            Postages.Clear();
            foreach (XmlNode n2 in n.SelectNodes("Postage"))
            {
                var p = new DomesticPostage(n2);
                Postages.Add(p);
            }
        }

        public void WriteToXml(ref XmlTextWriter xw)
        {
            SortDimensions();

            xw.WriteStartElement("Package");
            xw.WriteAttributeString("ID", Id);

            // Package Info
            xw.WriteElementString("Service", TranslateServiceCodeToString(Service));

            if (Service == DomesticServiceType.FirstClass
                || Service == DomesticServiceType.FirstClassHoldForPickupCommercial)
            {
                xw.WriteElementString("FirstClassMailType", TranslateContainerCode(Container));
            }

            xw.WriteElementString("ZipOrigination", ZipOrigination);
            xw.WriteElementString("ZipDestination", ZipDestination);
            xw.WriteElementString("Pounds", Pounds.ToString(CultureInfo.InvariantCulture));
            xw.WriteElementString("Ounces", Math.Round(Ounces, 1).ToString(CultureInfo.InvariantCulture));

            // Container and First Class Types
            if (Service == DomesticServiceType.FirstClass
                || Service == DomesticServiceType.FirstClassHoldForPickupCommercial)
            {
                xw.WriteElementString("Container", "");
            }
            else
            {
                xw.WriteElementString("Container", TranslateContainerCode(Container));
            }

            // Size
            if (DeterminePackageSize() == DomesticPackageSize.Large)
            {
                xw.WriteElementString("Size", "LARGE");
            }
            else
            {
                xw.WriteElementString("Size", "REGULAR");
            }

            // Dimesions here
            if (DeterminePackageSize() == DomesticPackageSize.Large)
            {
                if (HasDimensions())
                {
                    xw.WriteElementString("Width", Math.Round(Width, 1).ToString(CultureInfo.InvariantCulture));
                    xw.WriteElementString("Length", Math.Round(Length, 1).ToString(CultureInfo.InvariantCulture));
                    xw.WriteElementString("Height", Math.Round(Height, 1).ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    xw.WriteElementString("Width", Math.Round(3.0, 1).ToString(CultureInfo.InvariantCulture));
                    xw.WriteElementString("Length", Math.Round(6.0, 1).ToString(CultureInfo.InvariantCulture));
                    xw.WriteElementString("Height", Math.Round(0.25, 1).ToString(CultureInfo.InvariantCulture));
                }

                if (Container == DomesticPackageType.NonRectangular
                    || Container == DomesticPackageType.Variable
                    || Container == DomesticPackageType.Ignore)
                {
                    xw.WriteElementString("Girth", Math.Round(Girth(), 1).ToString(CultureInfo.InvariantCulture));
                }
            }

            // Machinable
            if (IsMachinable() == DomesticMachinable.Yes)
            {
                xw.WriteElementString("Machinable", "true");
            }
            if (IsMachinable() == DomesticMachinable.No)
            {
                xw.WriteElementString("Machinable", "false");
            }

            if (DeclaredValue > 0)
            {
                xw.WriteElementString("Value", Math.Round(DeclaredValue, 2).ToString(CultureInfo.InvariantCulture));
            }
            if (AmountToCollect > 0)
            {
                xw.WriteElementString("AmountToCollect",
                    Math.Round(AmountToCollect, 2).ToString(CultureInfo.InvariantCulture));
            }

            // Special Services
            if (SpecialServices.Count > 0)
            {
                xw.WriteStartElement("SpecialServices");
                foreach (var s in SpecialServices)
                {
                    xw.WriteElementString("SpecialService", ((int) s).ToString(CultureInfo.InvariantCulture));
                }
                xw.WriteEndElement();
            }

            // End Package
            xw.WriteEndElement();
        }

        public string TranslateServiceCodeToString(DomesticServiceType service)
        {
            switch (service)
            {
                case DomesticServiceType.All:
                    return "ALL";
                case DomesticServiceType.ExpressMail:
                    return "EXPRESS";
                case DomesticServiceType.ExpressMailCommerceial:
                    return "EXPRESS COMMERCIAL";
                case DomesticServiceType.ExpressMailHoldForPickup:
                    return "EXPRESS HFP";
                case DomesticServiceType.ExpressMailHoldForPickupCommercial:
                    return "EXPRESS HFP COMMERCIAL";
                case DomesticServiceType.ExpressMailSundayHoliday:
                    return "EXPRESS SH";
                case DomesticServiceType.ExpressMailSundayHolidayCommercial:
                    return "EXPRESS SH COMMERCIAL";
                case DomesticServiceType.FirstClass:
                    return "FIRST CLASS";
                case DomesticServiceType.LibraryMaterial:
                    return "LIBRARY";
                case DomesticServiceType.MediaMail:
                    return "MEDIA";
                case DomesticServiceType.Online:
                    return "ONLINE";
                case DomesticServiceType.ParcelPost:
                    return "PARCEL";
                case DomesticServiceType.PriorityMail:
                    return "PRIORITY";
            }

            return "ALL";
        }

        public DomesticServiceType TranslateServiceCode(string service)
        {
            switch (service.Trim().ToUpperInvariant())
            {
                case "ALL":
                    return DomesticServiceType.All;
                case "EXPRESS":
                    return DomesticServiceType.ExpressMail;
                case "EXPRESS COMMERCIAL":
                    return DomesticServiceType.ExpressMailCommerceial;
                case "EXPRESS HFP":
                    return DomesticServiceType.ExpressMailHoldForPickup;
                case "EXPRESS HFP COMMERCIAL":
                    return DomesticServiceType.ExpressMailHoldForPickupCommercial;
                case "EXPRESS SH":
                    return DomesticServiceType.ExpressMailSundayHoliday;
                case "EXPRESS SH COMMERCIAL":
                    return DomesticServiceType.ExpressMailSundayHolidayCommercial;
                case "FIRST CLASS":
                    return DomesticServiceType.FirstClass;
                case "LIBRARY":
                    return DomesticServiceType.LibraryMaterial;
                case "MEDIA":
                    return DomesticServiceType.MediaMail;
                case "ONLINE":
                    return DomesticServiceType.Online;
                case "PARCEL":
                    return DomesticServiceType.ParcelPost;
                case "PRIORITY":
                    return DomesticServiceType.PriorityMail;
            }

            return DomesticServiceType.All;
        }

        public string TranslateContainerCode(DomesticPackageType package)
        {
            switch (package)
            {
                case DomesticPackageType.FirstClassFlat:
                    return "FLAT";
                case DomesticPackageType.FirstClassLetter:
                    return "LETTER";
                case DomesticPackageType.FirstClassParcel:
                    return "PARCEL";
                case DomesticPackageType.FirstClassPostCard:
                    return "POSTCARD";
                case DomesticPackageType.FlatRateBox:
                    return "FLAT RATE BOX";
                case DomesticPackageType.FlatRateBoxLarge:
                    return "LG FLAT RATE BOX";
                case DomesticPackageType.FlatRateBoxMedium:
                    return "MD FLAT RATE BOX";
                case DomesticPackageType.FlatRateBoxSmall:
                    return "SM FLAT RATE BOX";
                case DomesticPackageType.FlatRateEnvelope:
                    return "FLAT RATE ENVELOPE";
                case DomesticPackageType.FlatRateEnvelopePadded:
                    return "PADDED FLAT RATE ENVELOPE";
                case DomesticPackageType.FlatRateEnvelopeLegal:
                    return "LEGAL FLAT RATE ENVELOPE";
                case DomesticPackageType.FlatRateEnvelopeWindow:
                    return "WINDOW FLAT RATE ENVELOPE";
                case DomesticPackageType.FlatRateEnvelopeGiftCard:
                    return "GIFT CARD FLAT RATE ENVELOPE";
                case DomesticPackageType.RegionalBoxRateA:
                    return "REGIONAL BOX RATE A";
                case DomesticPackageType.RegionalBoxRateB:
                    return "REGIONAL BOX RATE B";
                case DomesticPackageType.Ignore:
                    return "VARIABLE";
                case DomesticPackageType.NonRectangular:
                    return "NONRECTANGULAR";
                case DomesticPackageType.Rectangular:
                    return "RECTANGULAR";
                case DomesticPackageType.Variable:
                    return "VARIABLE";
            }
            return "VARIABLE";
        }
    }
}