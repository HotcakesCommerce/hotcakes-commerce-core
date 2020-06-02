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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    partial class PrintOrder : BaseAdminPage
    {

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Orders;
            ValidateCurrentUserHasPermission(SystemPermissions.OrdersView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                LoadTemplates();
                LoadMode();

                if (Request.QueryString["templateid"] != null)
                {
                    SetTemplate(Request.QueryString["templateid"]);
                }
                if (Request.QueryString["autoprint"] == "1")
                {
                    Generate();
                }
            }
        }

        private void LoadTemplates()
        {
            TemplateField.DataSource = HccApp.ContentServices.GetAllOrderTemplates();
            TemplateField.DataTextField = "DisplayName";
            TemplateField.DataValueField = "Id";
            TemplateField.DataBind();
        }

        private void SetTemplate(HtmlTemplateType templType)
        {
            var templ = HccApp.ContentServices.GetHtmlTemplateOrDefault(templType);

            SetTemplate(templ.Id.ToString());
        }

        private void SetTemplate(string templId)
        {
            TemplateField.SelectedValue = templId;
        }

        private void LoadMode()
        {
            if (Request.QueryString["mode"] != null)
            {
                switch (Request.QueryString["mode"])
                {
                    case "pack":
                        SetTemplate(HtmlTemplateType.OrderShipment);
                        Generate();
                        break;
                    case "receipt":
                        SetTemplate(HtmlTemplateType.NewOrderForAdmin);
                        Generate();
                        break;
                    case "invoice":
                        SetTemplate(HtmlTemplateType.NewOrder);
                        Generate();
                        break;
                    default:
                        SetTemplate(HtmlTemplateType.NewOrder);
                        Generate();
                        break;
                }
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            Generate();
        }

        protected void btnContinue2_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["id"].Contains(","))
            {
                Response.Redirect("default.aspx");
            }
            else
            {
                Response.Redirect("ViewOrder.aspx?id=" + Request.QueryString["id"]);
            }
        }

        private void Generate()
        {
            var id = Request.QueryString["id"];
            id = id.TrimEnd(',');
            var os = id.Split(',');
            rpOrder.DataSource = os;
            rpOrder.DataBind();
        }

        protected void rpOrder_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.AlternatingItem | e.Item.ItemType == ListItemType.Item)
            {
                long templateId = 0;
                long.TryParse(TemplateField.SelectedValue, out templateId);
                var t = HccApp.ContentServices.HtmlTemplates.Find(templateId);
                if (t != null)
                {
                    var orderId = (string)e.Item.DataItem;
                    var o = HccApp.OrderServices.Orders.FindForCurrentStore(orderId);
                    var litTemplate = (Literal)e.Item.FindControl("litTemplate");
                    if (litTemplate != null)
                    {
                        t = t.ReplaceTagsInTemplateForOrder(HccApp.CurrentRequestContext, o);
                        litTemplate.Text = t.Body;
                    }
                }

                var id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                {
                    var count = id.TrimEnd(',').Split(',').Length;
                    if (e.Item.ItemIndex != count - 1)
                    {
                        System.Web.UI.HtmlControls.HtmlGenericControl div = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("pagebreak");
                        div.Attributes.Add("style", "page-break-after: always;");
                    }
                }

            }
        }
    }
}