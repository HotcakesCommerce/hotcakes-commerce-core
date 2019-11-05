#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
    public class InternationalPackage
    {
        public InternationalPackage()
        {
            Init();
        }

        public InternationalPackage(XmlNode n)
        {
            Init();
            ParseNode(n);
        }

        // Properties for Request and Response
        public string Id { get; set; }
        public int Pounds { get; set; }
        public decimal Ounces { get; set; }
        
        // Machinable is function
        public InternationalPackageType MailType { get; set; }
        public bool GxgToPoBox { get; set; }
        public bool GxgGift { get; set; }
        public decimal ValueOfContents { get; set; }
        public string DestinationCountry { get; set; }
        public InternationalContainerType Container { get; set; }
        
        // Package Size is function
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        
        // Girth is Function
        public string ZipOrigination { get; set; } // Used for GXG mailability 
        public bool CommercialRates { get; set; }
        public List<InternationalExtraServiceType> ExtraServices { get; set; }
        
        // Values on Return
        public string Prohibitions { get; set; }
        public string Restrictions { get; set; }
        public string Observations { get; set; }
        public string CustomsForms { get; set; }
        public string ExpressMail { get; set; }
        public string AreasServed { get; set; }
        public List<InternationalPostage> Postages { get; set; }

        private void Init()
        {
            Id = "0";
            Pounds = 0;
            Ounces = 0m;
            Length = 0m;
            Width = 0m;
            Height = 0m;
            MailType = InternationalPackageType.All;
            GxgGift = false;
            GxgToPoBox = false;
            ValueOfContents = 0m;
            DestinationCountry = string.Empty;
            Container = InternationalContainerType.Rectangular;
            ZipOrigination = string.Empty;
            CommercialRates = false;
            ExtraServices = new List<InternationalExtraServiceType>();

            Prohibitions = string.Empty;
            Restrictions = string.Empty;
            Observations = string.Empty;
            CustomsForms = string.Empty;
            ExpressMail = string.Empty;
            AreasServed = string.Empty;
            Postages = new List<InternationalPostage>();
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

        public bool HasDimensions()
        {
            return Length > 0 &&
                   Height > 0 &&
                   Width > 0;
        }

        public InternationalPackageSize DeterminePackageSize()
        {
            if (Length > 12 || Width > 12 || Height > 12)
            {
                return InternationalPackageSize.Large;
            }
            if (Container == InternationalContainerType.NonRectangular)
            {
                return InternationalPackageSize.Large;
            }
            return InternationalPackageSize.Regular;
        }

        public InternationalMachinable IsMachinable()
        {
            SortDimensions();
            var aspectRatio = Length/Height;
            if (aspectRatio < 1.3m || aspectRatio > 2.5m)
            {
                return InternationalMachinable.No;
            }
            return InternationalMachinable.Yes;
        }

        public void ParseNode(XmlNode n)
        {
            if (n == null) return;

            if (n.Attributes.GetNamedItem("ID") != null)
            {
                Id = n.Attributes.GetNamedItem("ID").InnerText;
            }
            Prohibitions = Xml.ParseInnerText(n, "Prohibitions");
            Restrictions = Xml.ParseInnerText(n, "Restrictions");
            Observations = Xml.ParseInnerText(n, "Observations");
            CustomsForms = Xml.ParseInnerText(n, "CustomsForms");
            ExpressMail = Xml.ParseInnerText(n, "ExpressMail");
            AreasServed = Xml.ParseInnerText(n, "AreasServed");
            Postages.Clear();
            foreach (XmlNode n2 in n.SelectNodes("Service"))
            {
                var p = new InternationalPostage(n2);
                Postages.Add(p);
            }
        }

        public void WriteToXml(ref XmlTextWriter xw)
        {
            SortDimensions();

            xw.WriteStartElement("Package");
            xw.WriteAttributeString("ID", Id);

            // Weight Info
            xw.WriteElementString("Pounds", Pounds.ToString(CultureInfo.InvariantCulture));
            xw.WriteElementString("Ounces", Math.Round(Ounces, 1).ToString(CultureInfo.InvariantCulture));

            // Machinable
            if (IsMachinable() == InternationalMachinable.Yes)
            {
                xw.WriteElementString("Machinable", "true");
            }
            if (IsMachinable() == InternationalMachinable.No)
            {
                xw.WriteElementString("Machinable", "false");
            }

            // Mail Type
            xw.WriteElementString("MailType", TranslateMailType(MailType));

            // Gxg
            xw.WriteStartElement("GXG");
            xw.WriteElementString("POBoxFlag", GxgToPoBox ? "Y" : "N");
            xw.WriteElementString("GiftFlag", GxgGift ? "Y" : "N");
            xw.WriteEndElement();

            // Value
            xw.WriteElementString("ValueOfContents",
                Math.Round(ValueOfContents, 2).ToString(CultureInfo.InvariantCulture));

            // Country
            xw.WriteElementString("Country", DestinationCountry);
            
            // Determine Size
            var _size = DeterminePackageSize();

            // Container
            if (_size == InternationalPackageSize.Large)
            {
                if (Container == InternationalContainerType.NonRectangular)
                {
                    xw.WriteElementString("Container", "NONRECTANGULAR");
                }
                else
                {
                    xw.WriteElementString("Container", "RECTANGULAR");
                }
                xw.WriteElementString("Size", "LARGE");
            }
            else
            {
                xw.WriteElementString("Container", "RECTANGULAR");
                xw.WriteElementString("Size", "REGULAR");
            }

            // Dimesions here
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
                xw.WriteElementString("Height", Math.Round(1.0, 1).ToString(CultureInfo.InvariantCulture));
            }
            xw.WriteElementString("Girth", Math.Round(Girth(), 1).ToString(CultureInfo.InvariantCulture));

            // Origin Zip
            xw.WriteElementString("OriginZip", ZipOrigination);

            // Commercial Flag
            xw.WriteElementString("CommercialFlag", CommercialRates ? "Y" : "N");

            // Extra Services
            if (ExtraServices.Count > 0)
            {
                xw.WriteStartElement("ExtraServices");
                foreach (var s in ExtraServices)
                {
                    xw.WriteElementString("ExtraService", ((int) s).ToString());
                }
                xw.WriteEndElement();
            }

            // End Package
            xw.WriteEndElement();
        }

        private string TranslateMailType(InternationalPackageType internationalPackageType)
        {
            switch (internationalPackageType)
            {
                case InternationalPackageType.All:
                    return "All";
                case InternationalPackageType.Envelope:
                    return "Envelope";
                case InternationalPackageType.MatterForTheBlind:
                    return "Matter for the blind";
                case InternationalPackageType.Package:
                    return "Package";
                case InternationalPackageType.PostCards:
                    return "Postcards or aerogrammes";
            }
            return "All";
        }
    }
}