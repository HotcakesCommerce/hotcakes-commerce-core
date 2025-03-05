#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Hotcakes.Commerce.Catalog
{
    public class OptionSelections
    {
        public OptionSelections()
        {
            OptionSelectionList = new OptionSelectionList();
            BundleSelectionList = new Dictionary<long, OptionSelectionList>();
        }

        /// <summary>
        /// Represents a collection of product choices or variant selections for bundled products.
        /// </summary>
        public Dictionary<long, OptionSelectionList> BundleSelectionList { get; set; }

        /// <summary>
        /// Represents a collection of product choices or variant selections for the current product.
        /// </summary>
        public OptionSelectionList OptionSelectionList { get; set; }

        public void DeserializeFromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return;

            try
            {
                Clear();

                var sr = new StringReader(xml);
                var doc = XDocument.Load(sr);

                var selections = doc.XPathSelectElements("/OptionSelections/OptionSelection").
                    Select(os => new
                    {
                        OptionBvin = os.Element("OptionBvin").Value,
                        SelectionData = os.Element("SelectionData").Value
                    });

                foreach (var selection in selections)
                {
                    var sel = new OptionSelection();
                    sel.OptionBvin = selection.OptionBvin;
                    sel.SelectionData = selection.SelectionData;
                    OptionSelectionList.Add(sel);
                }

                var bundleSelections = doc.XPathSelectElements("/OptionSelections/OptionSelections");

                foreach (var bundleSelection in bundleSelections)
                {
                    var bundledProductIdString = bundleSelection.Attribute("BundledProductId").Value;
                    var bundledProductId = long.Parse(bundledProductIdString);
                    BundleSelectionList[bundledProductId] = new OptionSelectionList();

                    selections = bundleSelection.XPathSelectElements("OptionSelection").
                        Select(os => new
                        {
                            OptionBvin = os.Element("OptionBvin").Value,
                            SelectionData = os.Element("SelectionData").Value
                        });

                    foreach (var selection in selections)
                    {
                        var sel = new OptionSelection();
                        sel.OptionBvin = selection.OptionBvin;
                        sel.SelectionData = selection.SelectionData;
                        GetSelections(bundledProductId).Add(sel);
                    }
                }
            }
            catch (Exception ex)
            {
                Clear();
                EventLog.LogEvent(ex);
            }
        }

        public string SerializeToXml()
        {
            using (var sw = new StringWriter())
            {
                using (var xw = new XmlTextWriter(sw))
                {
                    xw.WriteStartElement("OptionSelections");

                    SerializeOptionsListToXml(xw, OptionSelectionList);

                    foreach (var bundleSelection in BundleSelectionList)
                    {
                        var bundledProductId = bundleSelection.Key;
                        var optionSelectionList = bundleSelection.Value;

                        xw.WriteStartElement("OptionSelections");
                        xw.WriteAttributeString("BundledProductId", bundledProductId.ToString());
                        SerializeOptionsListToXml(xw, optionSelectionList);
                        xw.WriteEndElement();
                    }

                    xw.WriteEndElement();
                }
                return sw.ToString();
            }
        }

        private void SerializeOptionsListToXml(XmlTextWriter xw, OptionSelectionList optionSelectionList)
        {
            foreach (var sel in optionSelectionList)
            {
                xw.WriteStartElement("OptionSelection");
                xw.WriteElementString("OptionBvin", sel.OptionBvin);
                xw.WriteElementString("SelectionData", sel.SelectionData);
                xw.WriteEndElement();
            }
        }

        public void Clear()
        {
            OptionSelectionList.Clear();
            BundleSelectionList.Clear();
        }

        public bool HasEmptySelection(Product product)
        {
            foreach (var os in OptionSelectionList)
            {
                if (HasEmptySelection(os, product.Options))
                {
                    return true;
                }
            }
            foreach (var bundleSelections in BundleSelectionList)
            {
                var bundlProduct = product.BundledProducts.FirstOrDefault(bp => bp.Id == bundleSelections.Key);
                var optionSelectionList = bundleSelections.Value;
                if (bundlProduct != null)
                {
                    foreach (var os in optionSelectionList)
                    {
                        if (HasEmptySelection(os, bundlProduct.BundledProduct.Options))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool HasEmptySelection(OptionSelection os, List<Option> options)
        {
            var opt = options.FirstOrDefault(o => o.Bvin.Replace("-", string.Empty) == os.OptionBvin);

            if (opt != null)
            {
                return opt.IsRequired && string.IsNullOrEmpty(os.SelectionData);
            }

            return false;
        }

        public OptionSelectionList GetSelections(long bundledProductId)
        {
            if (BundleSelectionList.Keys.Contains(bundledProductId))
            {
                return BundleSelectionList[bundledProductId];
            }
            return new OptionSelectionList();
        }

        public void AddBundleSelections(long bundledProductId, OptionSelection option)
        {
            if (!BundleSelectionList.Keys.Contains(bundledProductId))
            {
                BundleSelectionList[bundledProductId] = new OptionSelectionList();
            }

            BundleSelectionList[bundledProductId].Add(option);
        }

        /// <summary>
        ///     Return true if both objects are equal. Otherwise false.
        /// </summary>
        /// <param name="other">The other option selections</param>
        /// <returns></returns>
        public bool Equals(OptionSelections other)
        {
            var areEqual = OptionSelectionList.Equals(other.OptionSelectionList);
            if (areEqual)
            {
                foreach (var opSel in BundleSelectionList)
                {
                    var otherOpSelValue =
                        other.BundleSelectionList.Where(os => os.Key == opSel.Key)
                            .Select(os => os.Value)
                            .FirstOrDefault();
                    if (otherOpSelValue == null)
                    {
                        areEqual = false;
                        break;
                    }
                    areEqual &= opSel.Value.Equals(otherOpSelValue);
                    if (!areEqual)
                        break;
                }
            }
            return areEqual;
        }
    }
}