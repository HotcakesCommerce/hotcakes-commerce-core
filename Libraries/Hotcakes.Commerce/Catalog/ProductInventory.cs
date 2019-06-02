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
using System.Net.Mail;
using System.Text;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of product inventory
    /// </summary>
    /// <remarks>The REST API equivalent is ProductInventoryyDTO.</remarks>
    [Serializable]
    public class ProductInventory
    {
        public ProductInventory()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdated = DateTime.UtcNow;
            ProductBvin = string.Empty;
            VariantId = string.Empty;
            QuantityOnHand = 0;
            QuantityReserved = 0;
            LowStockPoint = 0;
            OutOfStockPoint = 0;
        }

        /// <summary>
        ///     This is the unique ID or primary key of the product inventory record.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the product inventory was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The unique ID or Bvin of the product that this inventory relates to.
        /// </summary>
        public string ProductBvin { get; set; }

        /// <summary>
        ///     When populated, the variant ID specifies that this record relates to a specific variant of the product.
        /// </summary>
        public string VariantId { get; set; }

        /// <summary>
        ///     The total physical count of items on hand.
        /// </summary>
        public int QuantityOnHand { get; set; }

        /// <summary>
        ///     Count of items in stock but reserved for carts or orders.
        /// </summary>
        public int QuantityReserved { get; set; }

        /// <summary>
        ///     Determines when a product has hit a point to where it is considered to be low on stock.
        /// </summary>
        public int LowStockPoint { get; set; }

        /// <summary>
        ///     The value that signifies that the the product should be considered out of stock.
        /// </summary>
        public int OutOfStockPoint { get; set; }

        /// <summary>
        ///     Calculates the number of products currently available to sale based on inventory levels and settings.
        /// </summary>
        public int QuantityAvailableForSale
        {
            get
            {
                var result = QuantityOnHand - OutOfStockPoint - QuantityReserved;
                return result;
            }
        }

        /// <summary>
        ///     Use this method to send an email to the merchant about the low stock level.
        /// </summary>
        /// <param name="State">This parameter is not used. Pass null to it.</param>
        /// <param name="app">An active instance of HotcakesApplication, used to access store settings.</param>
        /// <remarks>This method is currently not called in the application.</remarks>
        public static void EmailLowStockReport(object State, HotcakesApplication app)
        {
            var context = app.CurrentRequestContext;
            if (context == null) return;

            if (
                !EmailLowStockReport(context.CurrentStore.Settings.MailServer.EmailForGeneral,
                    context.CurrentStore.Settings.FriendlyName, app))
            {
                EventLog.LogEvent("Low Stock Report", "Low Stock Report Failed", EventLogSeverity.Error);
            }
        }

        /// <summary>
        ///     Use this method to send an email to the merchant about the low stock level.
        /// </summary>
        /// <param name="recipientEmail">String - the email address where to send the low stock report to</param>
        /// <param name="storeName">String - the name of the store to use for the email template</param>
        /// <param name="app">An active instance of HotcakesApplication, used to access store settings.</param>
        /// <returns>If true, the email was sent successfully using the given parameters.</returns>
        public static bool EmailLowStockReport(string recipientEmail, string storeName, HotcakesApplication app)
        {
            var result = false;

            try
            {
                var fromAddress = string.Empty;
                fromAddress = recipientEmail;

                var m = new MailMessage(fromAddress, recipientEmail);
                m.IsBodyHtml = false;
                m.Subject = "Low Stock Report From " + storeName;

                var sb = new StringBuilder();

                sb.AppendLine("The following are low in stock or out of stock:");
                sb.Append(Environment.NewLine);

                var inventories = app.CatalogServices.ProductInventories.FindAllLowStock();

                if (inventories.Count < 1)
                {
                    sb.Append("No out of stock items found.");
                }
                else
                {
                    foreach (var item in inventories)
                    {
                        var product = app.CatalogServices.Products.Find(item.ProductBvin);
                        if (product != null)
                        {
                            sb.Append(WebAppSettings.InventoryLowReportLinePrefix);
                            sb.Append(product.Sku);
                            sb.Append(", ");
                            sb.Append(product.ProductName);
                            sb.Append(", ");
                            sb.Append(item.QuantityOnHand);
                            sb.AppendLine(" ");
                        }
                    }
                }
                m.Body = sb.ToString();

                result = MailServices.SendMail(m, app.CurrentStore);
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = false;
            }

            return result;
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current product inventory object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of ProductInventoryDTO</returns>
        public ProductInventoryDTO ToDto()
        {
            var dto = new ProductInventoryDTO();

            dto.Bvin = Bvin;
            dto.LastUpdated = LastUpdated;
            dto.LowStockPoint = LowStockPoint;
            dto.ProductBvin = ProductBvin;
            dto.QuantityOnHand = QuantityOnHand;
            dto.QuantityReserved = QuantityReserved;
            dto.OutOfStockPoint = OutOfStockPoint;
            dto.VariantId = VariantId;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current product inventory object using a ProductInventoryDTO instance
        /// </summary>
        /// <param name="dto">An instance of the product inventory from the REST API</param>
        public void FromDto(ProductInventoryDTO dto)
        {
            if (dto == null) return;

            Bvin = dto.Bvin;
            LastUpdated = dto.LastUpdated;
            LowStockPoint = dto.LowStockPoint;
            ProductBvin = dto.ProductBvin;
            QuantityOnHand = dto.QuantityOnHand;
            QuantityReserved = dto.QuantityReserved;
            OutOfStockPoint = dto.OutOfStockPoint;
            VariantId = dto.VariantId;
        }

        #endregion
    }
}