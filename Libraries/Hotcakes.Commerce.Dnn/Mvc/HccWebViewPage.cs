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
using System.Web.Mvc;
using DotNetNuke.UI.Modules;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Dnn.Mvc
{
    /// <summary>
    /// Exposes Hotcakes and DNN context to a Razor view page through <see cref="IHccWebViewPage"/>.
    /// </summary>
    public interface IHccWebViewPage
    {
        /// <summary>
        /// Gets the DNN module instance context associated with the view.
        /// </summary>
        ModuleInstanceContext ModuleContext { get; }
    }

    /// <summary>
    /// Provides Hotcakes services and DNN module context to an untyped Razor view.
    /// </summary>
    [Serializable]
    public abstract class HccWebViewPage : WebViewPage, IHccWebViewPage
    {
        /// <summary>
        /// Gets or sets the localization helper for the current view resource file.
        /// </summary>
        public ILocalizationHelper Localization { get; set; }

        /// <summary>
        /// Gets the current Hotcakes application instance.
        /// </summary>
        public HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        /// <summary>
        /// Gets the DNN module instance context associated with the view.
        /// </summary>
        public ModuleInstanceContext ModuleContext
        {
            get
            {
                var dataTokens = Context.Request.RequestContext.RouteData.DataTokens;
                return dataTokens["moduleContext"] as ModuleInstanceContext;
            }
        }

        /// <summary>
        /// Initializes MVC helpers, view localization, and the current content culture.
        /// </summary>
        public override void InitHelpers()
        {
            base.InitHelpers();

            var viewNamePos = VirtualPath.LastIndexOf('/');
            var localResourceFile = VirtualPath.Insert(viewNamePos, "/App_LocalResources") + ".resx";
            Localization = Factory.Instance.CreateLocalizationHelper(localResourceFile);

            HccRequestContextUtils.UpdateUserContentCulture(HccRequestContext.Current);
        }
    }

    /// <summary>
    /// Provides Hotcakes services and DNN module context to a strongly typed Razor view through <see cref="IHccWebViewPage"/>.
    /// </summary>
    [Serializable]
    public abstract class HccWebViewPage<TModel> : WebViewPage<TModel>, IHccWebViewPage
    {
        /// <summary>
        /// Gets or sets the localization helper for the current view resource file.
        /// </summary>
        public ILocalizationHelper Localization { get; set; }

        /// <summary>
        /// Gets the current Hotcakes application instance.
        /// </summary>
        public HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        /// <summary>
        /// Gets the DNN module instance context associated with the view.
        /// </summary>
        public ModuleInstanceContext ModuleContext
        {
            get
            {
                var dataTokens = Context.Request.RequestContext.RouteData.DataTokens;
                return dataTokens["moduleContext"] as ModuleInstanceContext;
            }
        }

        /// <summary>
        /// Initializes MVC helpers, view localization, and the current content culture.
        /// </summary>
        public override void InitHelpers()
        {
            base.InitHelpers();

            var viewNamePos = VirtualPath.LastIndexOf('/');
            var localResourceFile = VirtualPath.Insert(viewNamePos, "/App_LocalResources") + ".resx";
            Localization = Factory.Instance.CreateLocalizationHelper(localResourceFile);

            HccRequestContextUtils.UpdateUserContentCulture(HccRequestContext.Current);
        }
    }
}
