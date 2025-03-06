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
using System.Web.UI;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public class HccUserControl : UserControl
    {
        private string _localResourceFile;

        public HotcakesApplication HccApp
        {
            get { return HccPage.HccApp; }
        }

        public IHccPage HccPage
        {
            get { return (IHccPage) Page; }
        }

        public virtual string LocalResourceFile
        {
            get
            {
                if (string.IsNullOrEmpty(_localResourceFile))
                {
                    var index = AppRelativeVirtualPath.LastIndexOf('/');
                    _localResourceFile = AppRelativeVirtualPath.Insert(index + 1, "App_LocalResources/");
                }
                return _localResourceFile;
            }
            set { _localResourceFile = value; }
        }

        public ILocalizationHelper Localization { get; set; }

        protected override void OnInit(EventArgs e)
        {
            Localization = Factory.Instance.CreateLocalizationHelper(LocalResourceFile);

            base.OnInit(e);
        }
    }
}