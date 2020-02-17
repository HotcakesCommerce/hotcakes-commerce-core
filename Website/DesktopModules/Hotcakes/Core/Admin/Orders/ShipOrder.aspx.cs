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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Shipping;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    partial class ShipOrder : BaseOrderPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "Shipping";
            CurrentTab = AdminTabType.Orders;
            ValidateCurrentUserHasPermission(SystemPermissions.OrdersEdit);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ucOrderStatusDisplay.CurrentOrder = CurrentOrder;
            lstTrackingProvider.SelectedIndexChanged += lstTrackingProvider_SelectedIndexChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                PopulateShippingProviders();
                LoadOrder();
            }
        }

        private void LoadOrder()
        {
            if (CurrentOrder != null)
            {
                lblOrderNumber.Text = "Order " + CurrentOrder.OrderNumber + " ";
                lblShippingAddress.Text = CurrentOrder.ShippingAddress.ToHtmlString();
                lblShippingTotal.Text = CurrentOrder.TotalShippingAfterDiscounts.ToString("c");

                ItemsGridView.DataSource = CurrentOrder.Items;
                ItemsGridView.DataBind();

                if (!CurrentOrder.Items.Any())
                {
                    pnlShip.Visible = false;
                }

                var packages = CurrentOrder.FindShippedPackages();
                packages.ForEach(p => { p.ShipDateUtc = DateHelper.ConvertUtcToStoreTime(HccApp, p.ShipDateUtc); });
                PackagesGridView.DataSource = packages;
                PackagesGridView.DataBind();

                if (packages == null || !packages.Any())
                {
                    hPackage.Visible = false;
                }
                else
                {
                    hPackage.Visible = true;
                }

                lblUserSelectedShippingMethod.Text = "User Selected Shipping Method: <strong>" +
                                                     CurrentOrder.ShippingMethodDisplayName + "</strong>";
                if (lstTrackingProvider.Items.FindByValue(CurrentOrder.ShippingMethodId) != null)
                {
                    lstTrackingProvider.ClearSelection();
                    lstTrackingProvider.Items.FindByValue(CurrentOrder.ShippingMethodId).Selected = true;
                    lstTrackingProvider.SelectedValue = CurrentOrder.ShippingMethodId;
                }

                ShippingProviderServices();
                if (lstTrackingProviderServices.Items.FindByValue(CurrentOrder.ShippingProviderServiceCode) != null)
                {
                    lstTrackingProviderServices.ClearSelection();
                    lstTrackingProviderServices.Items.FindByValue(CurrentOrder.ShippingProviderServiceCode).Selected =
                        true;
                    lstTrackingProviderServices.SelectedValue = CurrentOrder.ShippingProviderServiceCode;
                }

                CheckShippedQty(CurrentOrder.Items);
            }
            else
            {
                pnlShip.Visible = false;
            }
        }

        private void CheckShippedQty(List<LineItem> items)
        {
            var totalqty = 0;
            foreach (var li in items)
            {
                if (!li.IsNonShipping)
                {
                    var q = li.Quantity - li.QuantityShipped;
                    if (q > 0)
                    {
                        totalqty += q;
                    }
                }
            }

            if (totalqty <= 0)
            {
                pnlShip.Visible = false;
            }
            else
            {
                pnlShip.Visible = true;
            }
        }


        private void ReloadOrder(OrderShippingStatus previousShippingStatus, bool SendEmail = true)
        {
            CurrentOrder.EvaluateCurrentShippingStatus();
            HccApp.OrderServices.Orders.Update(CurrentOrder);
            var context = new OrderTaskContext
            {
                Order = CurrentOrder,
                UserId = CurrentOrder.UserID
            };
            context.Inputs.Add("hcc", "PreviousShippingStatus", previousShippingStatus.ToString());

            Workflow.RunByName(context, WorkflowNames.ShippingChanged);

            LoadOrder();
        }

        private void PopulateShippingProviders()
        {
            var shippingProviders = HccApp.OrderServices.ShippingMethods.FindAll(HccApp.CurrentStore.Id);

            if (shippingProviders != null && shippingProviders.Count > 0)
            {
                lstTrackingProvider.DataSource = shippingProviders;
                lstTrackingProvider.DataTextField = "Name";
                lstTrackingProvider.DataValueField = "Bvin";
                lstTrackingProvider.DataBind();
            }
            else
            {
                MessageBox1.ShowInformation("We suggest to add shipping method to serve you better.");
                lstTrackingProvider.Visible = false;
                lblShippingBy.Visible = false;
            }
        }

        protected void ItemsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lineItem = e.Row.DataItem as LineItem;

                var SKUField = (Label) e.Row.FindControl("SKUField");
                if (SKUField != null)
                {
                    SKUField.Text = lineItem.ProductSku;
                }

                var description = (Label) e.Row.FindControl("DescriptionField");
                if (description != null)
                {
                    description.Text = lineItem.ProductName;

                    var associatedProduct = lineItem.GetAssociatedProduct(HccApp);
                    if (associatedProduct != null)
                    {
                        if (lineItem.ShippingStatus == OrderShippingStatus.NonShipping)
                        {
                            description.Text += "<br />Non-Shipping ";
                        }
                        if (associatedProduct.ShippingMode == ShippingMode.ShipFromManufacturer)
                        {
                            description.Text += "<br />Ships From Manufacturer ";
                        }
                        else if (associatedProduct.ShippingMode == ShippingMode.ShipFromVendor)
                        {
                            description.Text += "<br />Ships From Vendor ";
                        }
                        if (associatedProduct.ShippingDetails.ShipSeparately)
                        {
                            description.Text += "<br /> Ships Separately ";
                        }
                    }
                }

                var QtyToShip = (TextBox) e.Row.FindControl("QtyToShip");

                if (QtyToShip != null)
                {
                    if (lineItem.ShippingStatus == OrderShippingStatus.NonShipping)
                    {
                        QtyToShip.Visible = false;
                    }
                    else
                    {
                        QtyToShip.Visible = true;
                    }

                    var q = lineItem.Quantity - lineItem.QuantityShipped;
                    if (q > 0)
                    {
                        QtyToShip.Text = string.Format("{0:#}", q);
                    }
                }

                if (lineItem.IsNonShipping)
                {
                    QtyToShip.Visible = false;
                }

                if (lineItem.QuantityShipped > 0m)
                {
                    var shipped = (Label) e.Row.FindControl("shipped");
                    if (shipped != null)
                    {
                        shipped.Text = string.Format("{0:#}", lineItem.QuantityShipped);
                    }
                }
                if (lineItem.QuantityShipped == lineItem.Quantity)
                {
                    QtyToShip.ReadOnly = true;
                    QtyToShip.Enabled = false;
                }
            }
        }

        protected void btnShipItems_Click(object sender, EventArgs e)
        {
            ShipOrPackageItems(false);
        }

        protected void btnCreatePackage_Click(object sender, EventArgs e)
        {
            ShipOrPackageItems(true);
        }

        private void ShipOrPackageItems(bool dontShip)
        {
            var previousShippingStatus = CurrentOrder.ShippingStatus;
            var serviceCode = string.Empty;
            var serviceProviderId = string.Empty;
            var shippinMethodBvin = string.Empty;
            var shippingMethod = HccApp.OrderServices.ShippingMethods.Find(lstTrackingProvider.SelectedValue);
            if (shippingMethod != null)
            {
                serviceProviderId = shippingMethod.ShippingProviderId;
                shippinMethodBvin = shippingMethod.Bvin;
            }
            if (lstTrackingProviderServices.Visible)
            {
                serviceCode = lstTrackingProviderServices.SelectedValue;
            }

            var p = ShipItems(CurrentOrder, TrackingNumberField.Text.Trim(), serviceProviderId, serviceCode, dontShip,
                shippinMethodBvin);
            ReloadOrder(previousShippingStatus, true);
        }

        private OrderPackage ShipItems(Order order, string trackingNumber, string serviceProvider, string serviceCode,
            bool dontShip, string shippingMethodId = null)
        {
            var p = new OrderPackage
            {
                ShipDateUtc = DateTime.UtcNow,
                TrackingNumber = trackingNumber,
                ShippingProviderId = serviceProvider,
                ShippingProviderServiceCode = serviceCode,
                OrderId = order.bvin,
                ShippingMethodId = string.IsNullOrEmpty(shippingMethodId) == false ? shippingMethodId : string.Empty
            };

            foreach (GridViewRow gvr in ItemsGridView.Rows)
            {
                if (gvr.RowType == DataControlRowType.DataRow)
                {
                    var lineItemId = (long) ItemsGridView.DataKeys[gvr.RowIndex].Value;
                    var lineItem = order.GetLineItem(lineItemId);
                    if (lineItem == null || lineItem.IsNonShipping)
                        continue;

                    var qty = 0;
                    var QtyToShip = (TextBox) gvr.FindControl("QtyToShip");
                    if (QtyToShip != null)
                    {
                        if (!int.TryParse(QtyToShip.Text, out qty))
                        {
                            qty = 0;
                        }
                    }

                    // Prevent shipping more than ordered if order is not recurring
                    if (!order.IsRecurring && qty > lineItem.Quantity - lineItem.QuantityShipped)
                    {
                        qty = lineItem.Quantity - lineItem.QuantityShipped;
                    }

                    if (qty > 0)
                    {
                        p.Items.Add(new OrderPackageItem(lineItem.ProductId, lineItem.Id, qty));
                        p.Weight += lineItem.ProductShippingWeight*qty;
                    }
                }
            }
            p.WeightUnits = WebAppSettings.ApplicationWeightUnits;

            if (p.Items.Count > 0)
            {
                order.Packages.Add(p);
                if (!dontShip)
                {
                    HccApp.OrdersShipPackage(p, order);
                }
                HccApp.OrderServices.Orders.Update(order);
            }
            return p;
        }

        protected void PackagesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var p = (OrderPackage) e.Row.DataItem;
            if (p != null)
            {
                var provider = AvailableServices.FindById(p.ShippingProviderId, HccApp.CurrentStore);
                var ShippedByField = (Label) e.Row.FindControl("ShippedByField");
                if (ShippedByField != null)
                {
                    if (provider != null)
                    {
                        var codes = provider.ListAllServiceCodes();
                        var serviceCode = codes.FirstOrDefault(c => c.Code == p.ShippingProviderServiceCode);

                        if (serviceCode != null)
                        {
                            ShippedByField.Text = serviceCode.DisplayName;
                        }
                        else
                        {
                            ShippedByField.Text = provider.Name;
                        }
                    }
                }

                var TrackingLink = (HyperLink) e.Row.FindControl("TrackingLink");
                var TrackingText = (Label) e.Row.FindControl("TrackingText");
                if (TrackingLink != null)
                {
                    TrackingLink.Text = p.TrackingNumber;
                    TrackingText.Text = p.TrackingNumber;
                    if (provider != null)
                    {
                        if (provider.IsSupportsTracking)
                        {
                            TrackingLink.NavigateUrl = provider.GetTrackingUrl(p.TrackingNumber);
                            TrackingLink.Visible = true;
                            TrackingText.Visible = false;
                        }
                        else
                        {
                            TrackingLink.Visible = false;
                            TrackingText.Visible = true;
                        }
                    }
                    else
                    {
                        TrackingLink.Visible = false;
                        TrackingText.Visible = false;
                    }
                }

                var TrackingTextBox = (TextBox) e.Row.FindControl("TrackingNumberTextBox");
                if (TrackingTextBox != null)
                {
                    TrackingTextBox.Text = p.TrackingNumber;
                }

                var items = (Label) e.Row.FindControl("items");
                if (items != null)
                {
                    items.Text = string.Empty;
                    foreach (var pi in p.Items)
                    {
                        if (pi.Quantity > 0)
                        {
                            items.Text += pi.Quantity.ToString("#") + " - ";

                            foreach (var li in CurrentOrder.Items)
                            {
                                if (li.Id == pi.LineItemId)
                                {
                                    items.Text += li.ProductSku + ": " + li.ProductName + "<br />";
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void PackagesGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var previousShippingStatus = CurrentOrder.ShippingStatus;

            var Id = (long) PackagesGridView.DataKeys[e.RowIndex].Value;

            var p = CurrentOrder.Packages.SingleOrDefault(y => y.Id == Id);
            if (p != null)
            {
                HccApp.OrdersUnshipItems(p, CurrentOrder);
                CurrentOrder.Packages.Remove(p);
                HccApp.OrderServices.Orders.Update(CurrentOrder);
            }
            ReloadOrder(previousShippingStatus);
        }

        protected void PackagesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UpdateTrackingNumber")
            {
                if (e.CommandSource is Control)
                {
                    var row = (GridViewRow) ((Control) e.CommandSource).NamingContainer;
                    var trackingNumberTextBox = (TextBox) row.FindControl("TrackingNumberTextBox");
                    if (trackingNumberTextBox != null)
                    {
                        var idstring = (string) e.CommandArgument;
                        long pid = 0;
                        long.TryParse(idstring, out pid);

                        var package = CurrentOrder.Packages.SingleOrDefault(y => y.Id == pid);
                        if (package != null)
                        {
                            package.TrackingNumber = trackingNumberTextBox.Text;
                            HccApp.OrderServices.Orders.Update(CurrentOrder);
                        }
                    }
                }
            }
            LoadOrder();
        }

        protected void lstTrackingProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShippingProviderServices();
        }

        private void ShippingProviderServices()
        {
            var selectedValue = lstTrackingProvider.SelectedValue;
            var services = new List<IServiceCode>();
            if (!string.IsNullOrEmpty(selectedValue))
            {
                var shippingMethod = HccApp.OrderServices.ShippingMethods.Find(selectedValue);
                var provider = AvailableServices.FindById(shippingMethod.ShippingProviderId, HccApp.CurrentStore);
                services = provider.ListAllServiceCodes();
            }

            if (services != null && services.Any())
            {
                var removeItem = services.FirstOrDefault(x => x.DisplayName == "All Available Services");
                if (removeItem != null)
                {
                    services.Remove(removeItem);
                }
            }

            if (services != null && services.Any())
            {
                lstTrackingProviderServices.ClearSelection();
                lblShippingServices.Visible = true;
                lstTrackingProviderServices.Visible = true;
                lstTrackingProviderServices.DataSource = services;
                lstTrackingProviderServices.DataTextField = "DisplayName";
                lstTrackingProviderServices.DataValueField = "Code";
                lstTrackingProviderServices.DataBind();
            }
            else
            {
                lstTrackingProviderServices.Items.Clear();
                lstTrackingProviderServices.Visible = false;
                lblShippingServices.Visible = false;
            }
        }
    }
}