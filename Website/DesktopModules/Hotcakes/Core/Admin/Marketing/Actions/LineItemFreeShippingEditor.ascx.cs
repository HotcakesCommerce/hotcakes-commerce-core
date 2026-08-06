#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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

            var allMethods = HccApp?.OrderServices?.ShippingMethods?.FindAll(HccApp.CurrentStore.Id)
                             ?? Enumerable.Empty<dynamic>();

            var selMethodIds = new System.Collections.Generic.HashSet<string>(
                (action?.MethodIds() ?? Enumerable.Empty<string>()),
                StringComparer.OrdinalIgnoreCase);

            lstFreeShippingMethods.Items.Clear();
            foreach (var method in allMethods.Where(m => !selMethodIds.Contains((m?.Bvin ?? string.Empty))))
            {
                var li = new ListItem { Text = method.Name, Value = (method.Bvin ?? string.Empty) };
                lstFreeShippingMethods.Items.Add(li);
            }

            string GetMethodName(string bvin)
            {
                if (string.IsNullOrEmpty(bvin)) return bvin;
                var method = allMethods.FirstOrDefault(m => string.Equals(m?.Bvin, bvin, StringComparison.OrdinalIgnoreCase));
                return method != null ? method.Name : bvin;
            }

            gvFreeShippingMethods.DataSource =
                (action?.MethodIds() ?? Enumerable.Empty<string>())
                .Select(id => new { DisplayName = GetMethodName(id), bvin = id });
            gvFreeShippingMethods.DataBind();
        }

        public override bool SaveAction()
        {
            return true;
        }

        protected void btnAddFreeShippingMethod_Click(object sender, EventArgs e)
        {
            var id = lstFreeShippingMethods?.SelectedValue?.Trim();
            var t = TypedAction;
            if (string.IsNullOrWhiteSpace(id) || t == null) return;

            t.AddItemId(id);
            UpdatePromotion();
            LoadAction();
        }

        protected void gvFreeShippingMethods_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var t = TypedAction;
            if (t == null || e?.Keys == null || e.Keys.Count == 0) return;

            var bvin = e.Keys[0] as string;
            if (string.IsNullOrWhiteSpace(bvin)) return;

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