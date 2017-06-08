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
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing.PromotionActions;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Actions
{
    public partial class LineItemFreeShippingEditor : BaseActionControl
    {
        private LineItemFreeShipping TypedAction
        {
            get { return Action as LineItemFreeShipping; }
        }

        public override void LoadAction()
        {
            var action = TypedAction;
            var allMethods = HccApp.OrderServices.ShippingMethods.FindAll(HccApp.CurrentStore.Id);
            var selMethodIds = action.MethodIds();

            lstFreeShippingMethods.Items.Clear();
            foreach (var method in allMethods.Where(m => !selMethodIds.Contains(m.Bvin.ToUpperInvariant())))
            {
                var li = new ListItem {Text = method.Name, Value = method.Bvin.ToUpperInvariant()};
                lstFreeShippingMethods.Items.Add(li);
            }

            Func<string, string> getMethodName = bvin =>
            {
                var method = allMethods.FirstOrDefault(m => m.Bvin.ToUpperInvariant() == bvin.ToUpperInvariant());
                return method != null ? method.Name : bvin;
            };
            gvFreeShippingMethods.DataSource =
                selMethodIds.Select(id => new {DisplayName = getMethodName(id), bvin = id});
            gvFreeShippingMethods.DataBind();
        }

        public override bool SaveAction()
        {
            return true;
        }

        protected void btnAddFreeShippingMethod_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lstFreeShippingMethods.SelectedValue))
            {
                var id = lstFreeShippingMethods.SelectedValue;

                var t = TypedAction;
                t.AddItemId(id);
                UpdatePromotion();
                LoadAction();
            }
        }

        protected void gvFreeShippingMethods_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var t = TypedAction;
            var bvin = (string) e.Keys[0];
            t.RemoveItemId(bvin);
            UpdatePromotion();
            LoadAction();
        }

        protected void btnDeleteFreeShippingMethod_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}