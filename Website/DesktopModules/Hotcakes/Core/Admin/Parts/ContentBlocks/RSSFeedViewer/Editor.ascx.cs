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
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.RSSFeedViewer
{
    partial class Editor : HccContentBlockPart
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);
            if (b != null)
            {
                FeedField.Text = b.BaseSettings.GetSettingOrEmpty("FeedUrl");
                chkShowDescription.Checked = b.BaseSettings.GetBoolSetting("ShowDescription");
                chkShowTitle.Checked = b.BaseSettings.GetBoolSetting("ShowTitle");
                MaxItemsField.Text = b.BaseSettings.GetIntegerSetting("MaxItems", 50).ToString();
            }
        }

        private void SaveData()
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);
            if (b != null)
            {
                b.BaseSettings.AddOrUpdate("FeedUrl", FeedField.Text.Trim());
                b.BaseSettings.SetBoolSetting("ShowDescription", chkShowDescription.Checked);
                b.BaseSettings.SetBoolSetting("ShowTitle", chkShowTitle.Checked);
                b.BaseSettings.SetIntegerSetting("MaxItems", MaxItemsField.Text.ConvertTo<int>());
                HccApp.ContentServices.Columns.UpdateBlock(b);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
            NotifyFinishedEditing();
        }
    }
}