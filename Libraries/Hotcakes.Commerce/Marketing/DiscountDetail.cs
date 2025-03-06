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
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Hotcakes.CommerceDTO.v1.Marketing;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Marketing
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of DiscountDetail in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is DiscountDetailDTO.</remarks>
    [Serializable]
    public class DiscountDetail
    {
        public DiscountDetail()
        {
            Id = Guid.NewGuid();
            Description = string.Empty;
            Amount = 0;
            DiscountType = PromotionType.Unknown;
            PromotionId = -1;
            ActionId = -1;
        }

        /// <summary>
        ///     Unique ID or primary key of the discount.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Description of what the discount is.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The amount of the discount to be applied to the qualifying order/line item(s).
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        ///     Indicates the type of discount that this is.
        /// </summary>
        public PromotionType DiscountType { get; set; }

        public long PromotionId { get; set; }
        public long ActionId { get; set; }

        /// <summary>
        ///     Accepts a listing of DiscountDetail and returns them in XML format.
        /// </summary>
        /// <param name="details">A listing of DiscountDetail to parse.</param>
        /// <returns>An XML representation of the DiscountDetail list.</returns>
        public static string ListToXml(List<DiscountDetail> details)
        {
            var x = new XElement("DiscountDetails",
                from detail in details
                select new XElement("DiscountDetail",
                    new XElement("Id", detail.Id),
                    new XElement("Description", detail.Description),
                    new XElement("Amount", detail.Amount.ToString(CultureInfo.InvariantCulture)),
                    new XElement("DiscountType", (int) detail.DiscountType),
                    new XElement("PromotionId", detail.PromotionId),
                    new XElement("ActionId", detail.ActionId)
                    )
                );
            return x.ToString(SaveOptions.None);
        }

        /// <summary>
        ///     Accepts an XML version of a DiscountDetail list and returns them in a DiscountDetail list.
        /// </summary>
        /// <param name="xml">XML version of DiscountDetail list to parse.</param>
        /// <returns>List of DiscountDetail</returns>
        public static List<DiscountDetail> ListFromXml(string xml)
        {
            var result = new List<DiscountDetail>();
            if (string.IsNullOrWhiteSpace(xml))
                return result;

            var doc = XDocument.Parse(xml, LoadOptions.None);

            if (doc.Descendants("DiscountDetail") != null &&
                doc.Descendants("DiscountDetail").Count() > 0)
            {
                var query = doc.Descendants("DiscountDetail").ToList();
                if (query != null)
                {
                    foreach (var xItem in query)
                    {
                        var detail = new DiscountDetail();
                        var id = new Guid();
                        Guid.TryParse(xItem.Element("Id").Value, out id);
                        detail.Id = id;
                        detail.Description = xItem.Element("Description").Value ?? string.Empty;
                        decimal amount = 0;
                        decimal.TryParse(xItem.Element("Amount").Value, NumberStyles.Number,
                            CultureInfo.InvariantCulture, out amount);

                        detail.Amount = amount;

                        detail.DiscountType = (PromotionType) Xml.ParseInteger(xItem, "DiscountType");
                        detail.PromotionId = Xml.ParseLong(xItem, "PromotionId");
                        detail.ActionId = Xml.ParseLong(xItem, "ActionId");

                        result.Add(detail);
                    }
                }
            }
            return result;
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current discount detail object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of DiscountDetailDTO</returns>
        public DiscountDetailDTO ToDto()
        {
            var dto = new DiscountDetailDTO();

            dto.Id = Id;
            dto.Description = Description ?? string.Empty;
            dto.Amount = Amount;
            dto.DiscountType = (int) DiscountType;
            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current DiscountDetail object using a DiscountDetailDTO instance
        /// </summary>
        /// <param name="dto">An instance of the discount detail from the REST API</param>
        public void FromDto(DiscountDetailDTO dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            Description = dto.Description ?? string.Empty;
            Amount = dto.Amount;
            DiscountType = (PromotionType) dto.DiscountType;
        }

        #endregion
    }
}