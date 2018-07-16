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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ClientDependency.Core.Config;
using DotNetNuke.Application;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
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

            var versionofDNN = typeof (DotNetNukeContext).Assembly.GetName().Version;

            if (versionofDNN >= new Version("8.0"))
            {
                RegisterCssInclude(FileOrder.Css.DefaultCss, "~/DesktopModules/Hotcakes/Core/Admin/default.css");
            }
            else
            {
                RegisterCssInclude(FileOrder.Css.DefaultCss, "~/Portals/_default/default.css");
            }
            RegisterCssInclude(FileOrder.Css.DefaultPriority, "~/DesktopModules/Hotcakes/Core/Admin/admin.css");
            if (!HideAdminControlBar)
            {
                RegisterCssInclude(FileOrder.Css.DefaultPriority,
                    "~/DesktopModules/Hotcakes/ControlPanel/ControlBar.css");
            }

            RegisterJsInclude(FileOrder.Js.jQuery, "~/Resources/Shared/Scripts/jquery/jquery.min.js");
            RegisterJsInclude(FileOrder.Js.jQueryMigrate, "~/Resources/Shared/Scripts/jquery/jquery-migrate.min.js");
            RegisterJsInclude(FileOrder.Js.jQueryUI, "~/Resources/Shared/Scripts/jquery/jquery-ui.min.js");

            if (!HideAdminControlBar)
            {
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 0, "~/js/dnn.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 1,
                    "~/Resources/Shared/Scripts/jquery/jquery.hoverIntent.min.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 2, "~/Resources/Shared/Scripts/dnn.jquery.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 3,
                    "~/Resources/Shared/components/DropDownList/dnn.DropDownList.js");
                RegisterJsInclude(FileOrder.Js.DefaultPriority + 4,
                    "~/DesktopModules/Hotcakes/ControlPanel/scripts/dnn.controlBar.js");
            }
            RegisterJsInclude(FileOrder.Js.DefaultPriority + 10,
                "~/DesktopModules/Hotcakes/Core/Scripts/jquery.ui.widget.js");
            RegisterJsInclude(FileOrder.Js.DefaultPriority + 11,
                "~/DesktopModules/Hotcakes/Core/Scripts/jquery.fileupload.js");
            RegisterJsInclude(FileOrder.Js.DefaultPriority + 12, "~/Resources/Shared/Scripts/knockout.js");
            RegisterJsInclude(FileOrder.Js.DefaultPriority + 13,
                "~/DesktopModules/Hotcakes/Core/Scripts/knockout.mapping.js");
            RegisterJsInclude(FileOrder.Js.DefaultPriority + 14, "~/DesktopModules/Hotcakes/Core/Scripts/hcc.core.js");
            RegisterJsInclude(FileOrder.Js.DefaultPriority + 15, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.min.js");
			RegisterJsInclude(FileOrder.Js.DefaultPriority + 16, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.Milestone.js");
			RegisterJsInclude(FileOrder.Js.DefaultPriority + 17, "~/DesktopModules/Hotcakes/Core/Scripts/chartjs/Chart.Milestone.Legend.js");

            RegisterJsInclude(FileOrder.Js.DefaultPriority + 18, "~/DesktopModules/Hotcakes/Core/Admin/Admin.js");

            RegisterJsInclude(FileOrder.Js.DefaultPriority + 19,
                "~/DesktopModules/Hotcakes/Core/Scripts/hcc.product.performance.js");
            RegisterJsInclude(FileOrder.Js.DefaultPriority + 20,
                "~/DesktopModules/Hotcakes/Core/Scripts/hcc.category.performance.js");
            RegisterJsInclude(FileOrder.Js.DefaultPriority + 21,
                "~/DesktopModules/Hotcakes/Core/Scripts/hcc.admin.dashboard.js");
            RegisterJsInclude(FileOrder.Js.DefaultPriority + 22,
                "~/DesktopModules/Hotcakes/Core/Scripts/jquery.switchButton.js");
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