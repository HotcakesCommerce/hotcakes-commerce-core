#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ClientDependency.Core.Config;
using DotNetNuke.Application;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using Hotcakes.Commerce;
using Hotcakes.Web;
using StackExchange.Profiling;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public class BaseMasterPage : MasterPage
    {
        // TODO: Check for DEBUG setting and use local scripts when true, else use CDN/minified versions of CSS & JS

        private LiteralControl _ltMiniProfiler;
        private readonly PlaceHolder _phDependencies = new PlaceHolder();

        public bool HideAdminControlBar { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var crl = new ClientResourceLoader();
            Page.Form.Controls.Add(crl);
            Page.Form.Controls.Add(_phDependencies);
            _ltMiniProfiler = new LiteralControl();
            Page.Form.Controls.Add(_ltMiniProfiler);
            HccPageHeaderProvider.Register(ClientDependencySettings.Instance);
            HccBodyProvider.Register(ClientDependencySettings.Instance);
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var hostSettings = Hotcakes.Commerce.Factory.CreateHostSettingsProvider();
            var debug = hostSettings.DebugModeEnabled();

            RegisterCssInclude(FileOrder.Css.DefaultCss, "~/DesktopModules/Hotcakes/Core/Admin/default.css");
            RegisterCssInclude(FileOrder.Css.DefaultPriority + 1, "~/DesktopModules/Hotcakes/Core/Admin/admin.css");
            RegisterCssInclude(FileOrder.Css.DefaultPriority + 2, "~/DesktopModules/Hotcakes/Core/Scripts/flatpickr/flatpickr.min.css");

            if (debug) { 
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 1, "~/DesktopModules/Hotcakes/Core/Scripts/jquery.ui.widget.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 2, "~/DesktopModules/Hotcakes/Core/Scripts/jquery.fileupload.js");
            }
            else
            {
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 1, "~/DesktopModules/Hotcakes/Core/Scripts/jquery.ui.widget.min.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 2, "~/DesktopModules/Hotcakes/Core/Scripts/jquery.fileupload.min.js");
            }

            RegisterJsInclude(FileOrder.Js.DefaultPriority + 3, "~/DesktopModules/Hotcakes/Core/Scripts/flatpickr/flatpickr.js"); // already minified

            RegisterJsInclude(FileOrder.Js.DefaultPriority + 4, "~/Resources/Shared/Scripts/knockout.js"); // dnn core (minified already)
            RegisterJsInclude(FileOrder.Js.DefaultPriority + 5, "~/DesktopModules/Hotcakes/Core/Scripts/knockout.mapping.js");

            if (debug)
            {
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 7, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.core.js");
            }
            else
            {
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 7, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.core.min.js");
            }

            RegisterJsInclude(FileOrder.Js.DefaultPriority + 8, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.min.js");

            if (debug)
            {
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 9, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.Milestone.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 10, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.Milestone.Legend.js");

                RegisterJsInclude(FileOrder.Js.DefaultPriority + 11, "~/DesktopModules/Hotcakes/Core/Admin/Admin.js");

                RegisterJsInclude(FileOrder.Js.DefaultPriority + 12, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.product.performance.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 13, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.category.performance.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 14, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.admin.dashboard.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 15, "~/DesktopModules/Hotcakes/Core/Scripts/jquery.switchButton.js");
            }
            else
            {
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 9, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.Milestone.min.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 10, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.Milestone.Legend.min.js");

                RegisterJsInclude(FileOrder.Js.DefaultPriority + 11, "~/DesktopModules/Hotcakes/Core/Admin/Admin.min.js");

                RegisterJsInclude(FileOrder.Js.DefaultPriority + 12, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.product.performance.min.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 13, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.category.performance.min.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 14, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.admin.dashboard.min.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 15, "~/DesktopModules/Hotcakes/Core/Scripts/jquery.switchButton.min.js");
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            _ltMiniProfiler.Text = MiniProfiler.RenderIncludes().ToHtmlString();
        }

        private void RegisterJsInclude(FileOrder.Js priority, string filePath)
        {
            _phDependencies.Controls.Add(new JsInclude
            {
                Priority = (int) priority,
                FilePath = filePath
            });
        }

        private void RegisterCssInclude(FileOrder.Css priority, string filePath)
        {
            _phDependencies.Controls.Add(new CssInclude
            {
                Priority = (int) priority,
                FilePath = filePath
            });
        }

        protected void RegisterScriptVariables(HtmlInputHidden input)
        {
            var httpAlias = PortalSettings.Current.DefaultPortalAlias;
            var startIndex = httpAlias.IndexOf('/');
            var str = startIndex <= 0 ? "/" : httpAlias.Substring(startIndex);
            var strValue = str.EndsWith("/") ? str : str + "/";

            var vars = new Dictionary<string, string>();
            vars.Add("hc_siteRoot", strValue);

            input.Value = vars.ObjectToJson();
        }
    }
}