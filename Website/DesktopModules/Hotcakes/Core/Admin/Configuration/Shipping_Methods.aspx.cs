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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class Shipping : BaseAdminPage
    {
        #region Properties

        private string SelectedMethodId
        {
            get
            {
                if (ViewState["SMI"] == null)
                {
                    return null;
                }
                return (string) ViewState["SMI"];
            }
            set { ViewState["SMI"] = value; }
        }

        #endregion

        #region Page Events

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("ShippingMethods");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnSaveVisibility.Click += btnSaveVisibility_Click;
            gvShippingMethods.RowCommand += gvShippingMethods_RowCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadMethods();
                LoadProviders();
                BindVisibilityTypes();
            }

            pnlEditVisibility.Visible = false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            cvAmount.Text = Localization.GetString("Decimal");
            cvAmount2.Text = Localization.GetString("GreaterThan");
            rfvAmount.Text = Localization.GetString("Required");

            cvAmountSb.Text = Localization.GetString("Decimal");
            cvAmountSb2.Text = Localization.GetString("GreaterThan");
            rfvAmount2.Text = Localization.GetString("Required");
        }

        #endregion

        #region Event Handlers

        private void gvShippingMethods_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRule")
            {
                var labelSubtotal = Localization.GetString("SubtotalAmountItem");
                var labelWeight = Localization.GetString("TotalWeightItem");

                var script = @"hcShowDialog(); 
                    $('#" + ddlTypes.ClientID + @"')
                        .change(function () { 
                            if( $(this).val() > 2 )
                            { 
                                $('.hcSubtotalAmount').show();
                                $('#" + pnlSecondDisplayRule.ClientID + @"').show();

                                if( $(this).val() == 4 )
                                {
                                    $('.hc-label-amount').html('" + labelWeight + @"');          
                                }
                                else if( $(this).val() == 3 )
                                {
                                    $('.hc-label-amount').html('" + labelSubtotal + @"');                         
                                }
                            }
                            else
                            {
                                $('#" + pnlSecondDisplayRule.ClientID + @"').hide();
                                $('.hcSubtotalAmount').hide();
                                $('.hc-textbox-amount').val('0'); 
                            }
                        });
                    if( $('#" + ddlTypes.ClientID + @"').val() > 2 )
                    { 
                        $('.hcSubtotalAmount').show();
                        $('#" + pnlSecondDisplayRule.ClientID + @"').show();

                        if ( $('#" + ddlTypes.ClientID + @"').val() == 4)
                        {
                            $('.hc-label-amount').html('" + labelWeight + @"');          
                        }
                        else if($('#" + ddlTypes.ClientID + @"').val() == 3 )
                        {
                            $('.hc-label-amount').html('" + labelSubtotal + @"');                         
                        }
                    }
                    else
                    {
                        $('#" + pnlSecondDisplayRule.ClientID + @"').hide();
                        $('.hcSubtotalAmount').hide();                              
                        $('.hc-textbox-amount').val('0');   
                    }";

                pnlEditVisibility.Visible = true;
                LoadSelectedItem(e.CommandArgument.ToString());
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "startupScr", script, true);
            }
        }

        protected void gvShippingMethods_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var bvin = (string) gvShippingMethods.DataKeys[e.RowIndex].Value;
            HccApp.OrderServices.ShippingMethods.Delete(bvin);
            LoadMethods();
        }

        protected void gvShippingMethods_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Response.Redirect(string.Format("Shipping_Methods_Edit.aspx?id={0}",
                gvShippingMethods.DataKeys[e.NewEditIndex].Value));
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            NewMethod();
        }

        protected void gvShippingMethods_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Text = Localization.GetString("Name");
                e.Row.Cells[2].Text = Localization.GetString("ShowWhen");
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var sm = (ShippingMethod) e.Row.DataItem;
                var lb = (LinkButton) e.Row.FindControl("btnShowWhen");
                lb.CommandArgument = sm.Bvin;
                e.Row.Attributes["id"] = sm.Bvin;

                switch (sm.VisibilityMode)
                {
                    case ShippingVisibilityMode.Always:
                        lb.Text = Localization.GetString("Always");
                        break;
                    case ShippingVisibilityMode.Never:
                        lb.Text = Localization.GetString("Never");
                        break;
                    case ShippingVisibilityMode.NoRates:
                        lb.Text = Localization.GetString("MethodsAbove");
                        break;
                    case ShippingVisibilityMode.SubtotalAmount:
                        lb.Text = string.Format(Localization.GetString("SubtotalAmount"), sm.VisibilityAmount);
                        break;
                    case ShippingVisibilityMode.TotalWeight:
                        lb.Text = string.Format(Localization.GetString("TotalWeight"), sm.VisibilityAmount);
                        break;
                    case ShippingVisibilityMode.TotalWeightLessThan:
                        lb.Text = string.Format(Localization.GetString("TotalWeightLessThan"), sm.VisibilityAmount);
                        break;
                }
                switch (sm.VisibilityModeSecondary)
                {
                    case ShippingVisibilityMode.SubtotalAmount:
                        lb.Text += string.Format(Localization.GetString("SubtotalAmountSecondary"), sm.VisibilityAmountSecondary);
                        break;
                    case ShippingVisibilityMode.TotalWeight:
                        lb.Text += string.Format(Localization.GetString("TotalWeightSecondary"), sm.VisibilityAmountSecondary);
                        break;
                    case ShippingVisibilityMode.TotalWeightLessThan:
                        lb.Text += string.Format(Localization.GetString("TotalWeightLessThanSecondary"), sm.VisibilityAmountSecondary);
                        break;
                }
            }
        }

        protected void btnEdit_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Edit");
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("DeleteConfirmation"));
        }

        protected void btnSort_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Sort");
        }

        private void btnSaveVisibility_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && SelectedMethodId != null)
            {
                var item = HccApp.OrderServices.ShippingMethods.Find(SelectedMethodId);

                if (item != null)
                {
                    var mode = int.Parse(ddlTypes.SelectedValue);
                    item.VisibilityMode = (ShippingVisibilityMode) mode;
                    if (mode < 3)
                    {
                        item.VisibilityAmount = null;
                    }
                    else
                    {
                        item.VisibilityAmount = decimal.Parse(txtSubtotal.Text);
                    }

                    var mode2 = int.Parse(ddlTypes2.SelectedValue);
                    item.VisibilityModeSecondary = (ShippingVisibilityMode)mode2;
                    if (mode2 > 0)
                    {
                        item.VisibilityAmountSecondary = decimal.Parse(txtSubtotal2.Text);
                    }
                    else
                    {
                        item.VisibilityAmountSecondary = null;
                    }

                    HccApp.OrderServices.ShippingMethods.Update(item);

                    LoadMethods();
                }
            }
        }

        #endregion

        #region Implementations

        private void LoadSelectedItem(string bvin)
        {
            var item = HccApp.OrderServices.ShippingMethods.Find(bvin);
            SelectedMethodId = bvin;
            lblMethodName.Text = item.Name;

            if (item.VisibilityMode == ShippingVisibilityMode.SubtotalAmount ||
                item.VisibilityMode == ShippingVisibilityMode.TotalWeight ||
                item.VisibilityMode == ShippingVisibilityMode.TotalWeightLessThan)
            {
                if (item.VisibilityAmount.HasValue)
                {
                    pnlSecondDisplayRule.Visible = true;
                    txtSubtotal.Text = item.VisibilityAmount.Value.ToString("0.00");
                }
                else
                {
                    txtSubtotal.Text = "0.00";
                }
            }
            else
            {
                txtSubtotal.Text = "0.00";
            }

            if (item.VisibilityModeSecondary == ShippingVisibilityMode.SubtotalAmount ||
                item.VisibilityModeSecondary == ShippingVisibilityMode.TotalWeight ||
                item.VisibilityModeSecondary == ShippingVisibilityMode.TotalWeightLessThan)
            {
                if (item.VisibilityAmountSecondary.HasValue && item.VisibilityAmountSecondary.Value > 0)
                {
                    txtSubtotal2.Text = item.VisibilityAmountSecondary.Value.ToString("0.00");
                }
                else
                {
                    txtSubtotal2.Text = "0.00";
                }
            }
            else
            {
                txtSubtotal2.Text = "0.00";
            }

            ddlTypes.SelectedIndex = (int) item.VisibilityMode;

            try
            {
                ddlTypes2.SelectedValue = ((int)item.VisibilityModeSecondary).ToString();
            }
            catch { ddlTypes2.SelectedValue = "-1"; }
        }

        private void LoadMethods()
        {
            List<ShippingMethod> methods;
            methods = HccApp.OrderServices.ShippingMethods.FindAll(HccApp.CurrentStore.Id);
            methods = methods.OrderBy(s => s.SortOrder).ToList();

            if (methods.Count > 0 && methods[0].VisibilityMode == ShippingVisibilityMode.NoRates)
            {
                // Update visibility mode for first item if it has wrong mode.
                methods[0].VisibilityMode = ShippingVisibilityMode.Never;
                HccApp.OrderServices.ShippingMethods.Update(methods[0]);
            }

            gvShippingMethods.DataSource = methods;
            gvShippingMethods.DataBind();
        }

        private void LoadProviders()
        {
            lstProviders.ClearSelection();
            foreach (var p in AvailableServices.FindAll(HccApp.CurrentStore))
            {
                lstProviders.Items.Add(new ListItem(p.Name, p.Id));
            }
        }

        private void NewMethod()
        {
            var m = new ShippingMethod();

            m.ShippingProviderId = lstProviders.SelectedValue;
            m.Name = lstProviders.SelectedItem.Text;

            if (HccApp.OrderServices.ShippingMethods.Create(m))
            {
                Response.Redirect("Shipping_Methods_Edit.aspx?id=" + m.Bvin + "&doc=1");
            }
        }

        private void BindVisibilityTypes()
        {
            var list = new List<ListItem>();
            var listSecondary = new List<ListItem>();

            var liNone = new ListItem
            {
                Text = Localization.GetString("None"),
                Value = "-1"
            };

            var li = new ListItem
            {
                Text = Localization.GetString("Always"),
                Value = ((int) ShippingVisibilityMode.Always).ToString()
            };

            var li2 = new ListItem
            {
                Text = Localization.GetString("Never"),
                Value = ((int) ShippingVisibilityMode.Never).ToString()
            };

            var li3 = new ListItem
            {
                Text = Localization.GetString("MethodsAbove"),
                Value = ((int) ShippingVisibilityMode.NoRates).ToString()
            };

            var li4 = new ListItem
            {
                Text = string.Format(Localization.GetString("SubtotalAmount"), string.Empty),
                Value = ((int) ShippingVisibilityMode.SubtotalAmount).ToString()
            };

            var li5 = new ListItem
            {
                Text = string.Format(Localization.GetString("TotalWeight"), string.Empty),
                Value = ((int) ShippingVisibilityMode.TotalWeight).ToString()
            };

            var li6 = new ListItem
            {
                Text = string.Format(Localization.GetString("TotalWeightLessThan"), string.Empty),
                Value = ((int) ShippingVisibilityMode.TotalWeightLessThan).ToString()
            };

            list.Add(li);
            list.Add(li2);
            list.Add(li3);
            list.Add(li4);
            list.Add(li5);
            list.Add(li6);

            ddlTypes.DataValueField = "Value";
            ddlTypes.DataTextField = "Text";
            ddlTypes.DataSource = list;
            ddlTypes.DataBind();

            listSecondary.Add(liNone);
            listSecondary.Add(li4);
            listSecondary.Add(li5);
            listSecondary.Add(li6);

            ddlTypes2.DataValueField = "Value";
            ddlTypes2.DataTextField = "Text";
            ddlTypes2.DataSource = listSecondary;
            ddlTypes2.DataBind();
        }

        #endregion
    }
}