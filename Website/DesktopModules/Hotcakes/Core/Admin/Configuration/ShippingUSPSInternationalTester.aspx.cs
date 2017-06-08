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
using System.Text;
using System.Web;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Shipping;
using Hotcakes.Shipping.USPostal;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class ShippingUSPSInternationalTester : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("USPostalServiceInternationalRatesTester");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadServices();
                LoadCountries();
                FromZipField.Text = HccApp.ContactServices.Addresses.FindStoreContactAddress().PostalCode;
            }
        }

        private void LoadCountries()
        {
            lstCountries.DataSource = HccApp.GlobalizationServices.Countries.FindAll();
            lstCountries.DataTextField = "DisplayName";
            lstCountries.DataValueField = "Bvin";
            lstCountries.DataBind();
        }

        private void LoadServices()
        {
            var uspostal = AvailableServices.FindById(InternationalProvider.ServiceId, HccApp.CurrentStore);
            lstServiceTypes.DataSource = uspostal.ListAllServiceCodes();
            lstServiceTypes.DataTextField = "DisplayName";
            lstServiceTypes.DataValueField = "Code";
            lstServiceTypes.DataBind();
        }

        protected void btnGetRates_Click(object sender, EventArgs e)
        {
            pnlRates.Visible = true;

            var address = new Address {CountryBvin = lstCountries.SelectedValue};

            var shipment = new Shipment
            {
                DestinationAddress = address,
                SourceAddress = {PostalCode = FromZipField.Text.Trim()}
            };

            // box
            var item = new Shippable();

            var length = 0m;
            decimal.TryParse(LengthField.Text.Trim(), out length);
            var height = 0m;
            decimal.TryParse(HeightField.Text.Trim(), out height);
            var width = 0m;
            decimal.TryParse(WidthField.Text.Trim(), out width);
            var weightPounds = 0m;
            decimal.TryParse(WeightPoundsField.Text.Trim(), out weightPounds);
            var weightOunces = 0m;
            decimal.TryParse(WeightOuncesField.Text.Trim(), out weightOunces);
            item.BoxLength = length;
            item.BoxHeight = height;
            item.BoxWidth = width;
            item.BoxLengthType = LengthType.Inches;
            item.BoxWeight = weightPounds + Conversions.OuncesToDecimalPounds(weightOunces);
            item.BoxWeightType = WeightType.Pounds;
            item.QuantityOfItemsInBox = 1;

            shipment.Items.Add(item);

            // Global Settings
            var globalSettings = new USPostalServiceGlobalSettings
            {
                DiagnosticsMode = true,
                IgnoreDimensions = false
            };

            // Settings
            var settings = new USPostalServiceSettings();
            var code = new ServiceCode
            {
                Code = lstServiceTypes.SelectedItem.Value,
                DisplayName = lstServiceTypes.SelectedItem.Text
            };

            var codes = new List<IServiceCode> {code};

            settings.ServiceCodeFilter = codes;

            // Provider
            var logger = new TextLogger();

            var provider = new InternationalProvider(globalSettings, logger) {Settings = settings};

            var rates = provider.RateShipment(shipment);

            var sbRates = new StringBuilder();

            sbRates.Append("<ul>");
            foreach (var rate in rates)
            {
                sbRates.Append("<li>");
                sbRates.Append(rate.EstimatedCost.ToString("c"));
                sbRates.Append(" - ");
                sbRates.Append(rate.DisplayName);
                sbRates.Append("</li>");
            }
            sbRates.Append("</ul>");

            litRates.Text = sbRates.ToString();

            var sbMessages = new StringBuilder();

            sbMessages.Append("<ul>");
            foreach (var msg in provider.LatestMessages)
            {
                sbMessages.Append("<li>");
                switch (msg.MessageType)
                {
                    case ShippingServiceMessageType.Diagnostics:
                        sbMessages.Append(Localization.GetString("Diagnostics"));
                        break;
                    case ShippingServiceMessageType.Information:
                        sbMessages.Append(Localization.GetString("Info"));
                        break;
                    case ShippingServiceMessageType.Error:
                        sbMessages.Append(Localization.GetString("Error"));
                        break;
                }
                sbMessages.Append(":");

                sbMessages.Append(HttpUtility.HtmlEncode(string.Concat(msg.Description, " ", msg.Code)));
                sbMessages.Append("</li>");
            }
            sbMessages.Append("</ul>");

            litMessages.Text = sbMessages.ToString();

            litXml.Text = string.Empty;

            litXml.Text = string.Empty;
            while (logger.Messages.Count > 0)
            {
                var tempXml = logger.Messages.Dequeue();
                tempXml = tempXml.Replace("\n", "<br />");
                tempXml = tempXml.Replace("\r", "<br />");
                tempXml = tempXml.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                litXml.Text += "<li>" + HttpUtility.HtmlEncode(tempXml) + "</li>";
            }
        }
    }
}