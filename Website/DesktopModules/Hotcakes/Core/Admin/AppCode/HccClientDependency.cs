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
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using ClientDependency.Core.Config;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.Client.Providers;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    public class CssInclude : DnnCssInclude
    {
        public CssInclude()
        {
            ForceProvider = HccPageHeaderProvider.ProviderName;
        }
    }

    public class JsInclude : DnnJsInclude
    {
        public JsInclude()
        {
            ForceProvider = HccBodyProvider.ProviderName;
        }
    }

    public class HccPageHeaderProvider : DnnPageHeaderProvider
    {
        internal const string ProviderName = "HccPageHeaderProvider";

        public override void Initialize(string name, NameValueCollection config)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = ProviderName;
            }

            base.Initialize(name, config);
        }

        protected override void RegisterDependencies(HttpContextBase http, string js, string css)
        {
            if (!(http.CurrentHandler is Page))
            {
                throw new InvalidOperationException(
                    "The current HttpHandler in a WebFormsFileRegistrationProvider must be of type Page");
            }
            var page = (Page) http.CurrentHandler;
            if (page.Header == null)
            {
                throw new NullReferenceException(
                    "HccPageHeaderProvider requires a runat='server' tag in the page's header tag");
            }
            var child = new LiteralControl(css.Replace("&", "&amp;"));
            var child2 = new LiteralControl(js.Replace("&", "&amp;"));
            page.Header.Controls.AddAt(0, child);
            page.Header.Controls.Add(child2);
        }

        public static void Register(ClientDependencySettings cdSettings)
        {
            if (cdSettings.FileRegistrationProviderCollection[ProviderName] == null)
            {
                var provider = new HccPageHeaderProvider();
                provider.Initialize(ProviderName, null);
                cdSettings.FileRegistrationProviderCollection.Add(provider);
            }
        }
    }

    public class HccBodyProvider : DnnBodyProvider
    {
        internal const string ProviderName = "HccBodyProvider";

        public override void Initialize(string name, NameValueCollection config)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = ProviderName;
            }

            base.Initialize(name, config);
        }

        protected override void RegisterDependencies(HttpContextBase http, string js, string css)
        {
            if (!(http.CurrentHandler is Page))
            {
                throw new InvalidOperationException(
                    "The current HttpHandler in a WebFormsFileRegistrationProvider must be of type Page");
            }
            var page = (Page) http.CurrentHandler;

            if (page.Header == null)
                throw new NullReferenceException(
                    "HccBodyProvider requires a runat='server' tag in the page's header tag");

            var jsScriptBlock = new LiteralControl(js.Replace("&", "&amp;"));
            var cssStyleBlock = new LiteralControl(css.Replace("&", "&amp;"));

            var holderControl = page.Master.FindControl(DnnBodyPlaceHolderName);
            holderControl.Controls.Add(jsScriptBlock);
            holderControl.Controls.Add(cssStyleBlock);

            var form = page.FindControl("Form");
            if (form != null)
            {
                form.Controls.Remove(holderControl);
                form.Controls.AddAt(0, holderControl);
            }
        }

        public static void Register(ClientDependencySettings cdSettings)
        {
            if (cdSettings.FileRegistrationProviderCollection[ProviderName] == null)
            {
                var provider = new HccBodyProvider();
                provider.Initialize(ProviderName, null);
                cdSettings.FileRegistrationProviderCollection.Add(provider);
            }
        }
    }
}