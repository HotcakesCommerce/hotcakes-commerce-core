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
using Hotcakes.Commerce.Taxes.Providers;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class TaxProviderEditor : HccPart
    {
        protected HccTaxProviderPart Editor { get; set; }

        public void LoadEditor(string providerID)
        {
            Visible = true;
            var provider = TaxProviders.Find(providerID, HccApp.CurrentStore);
            Editor = HccPartController.LoadTaxProviderEditor(provider.ProviderName, Page) as HccTaxProviderPart;

            if (Editor != null)
            {
                ulActions.Visible = !Editor.HasOwnButtons;

                Editor.ID = "TaxProviderEditor_" + provider.ProviderName;
                Editor.ProviderId = provider.ProviderId;
                phEditor.Controls.Clear();
                phEditor.Controls.Add(Editor);
                Editor.EditingComplete += Editor_EditingComplete;
                Editor.LoadData();
            }
            else
            {
                msg.ShowError(Localization.GetString("TaxProviderEditorError"));
            }
        }

        public void RemoveEditor()
        {
            phEditor.Controls.Clear();
            Visible = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Editor != null)
                    Editor.SaveData();
                NotifyFinishedEditing();
            }
        }

        private void Editor_EditingComplete(object sender, HccPartEventArgs e)
        {
            NotifyFinishedEditing();
        }
    }
}