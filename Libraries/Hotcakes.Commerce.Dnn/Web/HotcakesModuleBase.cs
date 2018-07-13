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
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;
using Hotcakes.Commerce.Dnn.Mvc;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web;
using StackExchange.Profiling;

namespace Hotcakes.Commerce.Dnn.Web
{
    [Serializable]
    public class HotcakesModuleBase : PortalModuleBase
    {
        // TODO: Change all script references to look for the DEBUG setting, and be CDN/minified scripts except when DEBUG == true, then use the local scripts

        private const string IsScriptResourcesAddedKey = "IsScriptResourcesAddedKey";
        private const string ScriptsBasePath = "~/DesktopModules/Hotcakes/Core/Scripts/";
        private LiteralControl _ltMiniProfiler;

        private bool IsScriptResourcesAdded
        {
            get
            {
                return Page.Items[IsScriptResourcesAddedKey] != null && (bool) Page.Items[IsScriptResourcesAddedKey];
            }
            set { Page.Items[IsScriptResourcesAddedKey] = value; }
        }

        public MvcRenderingEngine MvcRenderingEngine { get; set; }

        protected HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            JavaScript.RegisterClientReference(Page, ClientAPI.ClientNamespaceReferences.dnn);

            HccRequestContextUtils.UpdateUserContentCulture(HccRequestContext.Current);

            _ltMiniProfiler = new LiteralControl();
            Page.Form.Controls.Add(_ltMiniProfiler);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (HccApp.CurrentStore == null)
                    throw new StoreNotFoundException();

                InitializeScriptManager();
                RegisterScriptVariables();

                MvcRenderingEngine = new MvcRenderingEngine(ModuleContext);

                // Register css
                RegisterStyleSheet(HccApp.ViewsVirtualPath + "/views.css");

                // Register scripts
                RegisterScript("hcc.core.js", 1);
                RegisterScript("hcc.views.js", 2);

                JavaScript.RequestRegistration(CommonJs.DnnPlugins);

                var panel = new Panel();

                using (MiniProfiler.Current.Step(ModuleConfiguration.DesktopModule.FriendlyName))
                {
                    var view = RenderView();

                    var literal = new LiteralControl(view);

                    panel.CssClass = "hcMvcView";
                    literal.EnableViewState = false;

                    panel.Controls.Add(literal);
                }
                Controls.Add(panel);
            }
            catch (StoreNotFoundException)
            {
                var control = LoadControl("~/DesktopModules/Hotcakes/Core/Controls/StoreNotFound.ascx");

                Controls.Clear();
                Controls.Add(control);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            _ltMiniProfiler.Text = MiniProfiler.RenderIncludes().ToHtmlString();
        }

        protected void RegisterStyleSheet(string path)
        {
            ClientResourceManager.RegisterStyleSheet(Page, ResolveUrl(path), FileOrder.Css.ModuleCss);
        }

        protected void RegisterViewScript(string path, int order = 0)
        {
            ClientResourceManager.RegisterScript(Page, ResolveUrl(HccApp.ViewsVirtualPath + "/Scripts/" + path),
                FileOrder.Js.DefaultPriority + 10 + order);
        }

        protected void RegisterScript(string path, int order = 0)
        {
            ClientResourceManager.RegisterScript(Page, ResolveUrl(ScriptsBasePath + path),
                FileOrder.Js.DefaultPriority + order);
        }

        protected void RegisterUnobtrusiveValidateScripts()
        {
            RegisterScript("jquery.validate.hcc.js", 3);
            RegisterScript("jquery.validate.hcc.unobtrusive.js", 4);
        }

        protected void RegisterKOScripts()
        {
            ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/Scripts/knockout.js", FileOrder.Js.jQuery);
            ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/Scripts/jquery.history.js",
                FileOrder.Js.jQuery);
            ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/Scripts/json2.js", FileOrder.Js.jQuery);
            ClientResourceManager.RegisterScript(Page, ResolveUrl(ScriptsBasePath + "knockout.mapping.js"),
                FileOrder.Js.jQuery + 1);
        }

        protected virtual void InitializeScriptManager()
        {
            if (!IsScriptResourcesAdded)
            {
                var localResourceFile = string.Format("{0}/Scripts/App_LocalResources/ScriptResources.resx",
                    HccApp.ViewsVirtualPath);
                var localization = Factory.Instance.CreateLocalizationHelper(localResourceFile);

                var dictionary = localization.GetResourceDictionary();

                var json = dictionary.ObjectToJson();

                // add the ecommerce tracking script. This was added as per the request from client on support forum.                
                var analyticsScript = string.Empty;
                if (HccApp.CurrentStore.Settings.Analytics.UseGoogleEcommerce)
                {
                    analyticsScript = " ga('require', 'ecommerce'); ";
                }

                var scriptResource = string.Format(" {1} var hcc = hcc || {{}}; hcc.l10n = {0};", json, analyticsScript);
                ScriptManager.RegisterClientScriptBlock(this, typeof (HotcakesModuleBase), "ScriptResources",
                    scriptResource, true);

                IsScriptResourcesAdded = true;
            }
        }

        protected void RegisterScriptVariables()
        {
            var httpAlias = PortalSettings.DefaultPortalAlias;
            var startIndex = httpAlias.IndexOf('/');
            var str = startIndex <= 0 ? "/" : httpAlias.Substring(startIndex);
            var strValue = str.EndsWith("/") ? str : str + "/";

            ClientAPI.RegisterClientVariable(Page, "hc_siteRoot", strValue, /*overwrite*/ true);
        }

        protected virtual string RenderView()
        {
            return null;
        }
    }
}