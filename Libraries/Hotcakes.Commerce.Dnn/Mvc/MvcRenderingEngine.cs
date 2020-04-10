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
using System.Collections;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using DotNetNuke.UI.Modules;
using Hotcakes.Commerce.Extensions;

namespace Hotcakes.Commerce.Dnn.Mvc
{
    [Serializable]
    public class MvcRenderingEngine
    {
        public MvcRenderingEngine(ModuleInstanceContext moduleContext)
        {
            ModuleContext = moduleContext;
        }

        public ModuleInstanceContext ModuleContext { get; set; }

        public string Render(string controllerName, string actionName, object routeValues)
        {
            return Render(controllerName, actionName, null, routeValues);
        }

        public string Render(string controllerName, string actionName, string viewName = null, object routeValues = null)
        {
            if (routeValues == null)
                routeValues = new object();
            var routeData = CreateRouteData(controllerName, actionName, new RouteValueDictionary(routeValues));
            var httpContextBase = new HccHttpContextWrapper(HttpContext.Current);
            httpContextBase.Request.RequestContext.RouteData = routeData;
            var reqContext = new RequestContext(httpContextBase, routeData);

            var viewContext = new ViewContext();
            viewContext.RouteData = routeData;
            viewContext.RequestContext = reqContext;
            viewContext.HttpContext = httpContextBase;
            viewContext.ViewData = new ViewDataDictionary();

            if (!string.IsNullOrEmpty(viewName))
            {
                viewContext.ViewData["moduleViewName"] = viewName;
            }
            else if (ModuleContext != null)
            {
                var viewSettingName = string.Empty;
                var settings = new Hashtable(ModuleContext.Settings, StringComparer.OrdinalIgnoreCase);

                //added condition to check for searchinput action also. Because that action also has settings saved as View. So this also needs to be
                //managed like all index pages. 
                //Without adding search input condition issue occured when end user apply custom template for the search input view from module settings on page.
                if (actionName.Equals("Index", StringComparison.InvariantCultureIgnoreCase) ||
                    actionName.ToLower().Contains("searchinput"))
                    viewSettingName = "View";
                else
                    viewSettingName = string.Concat(actionName, "View");
                viewContext.ViewData["moduleViewName"] = (string) settings[viewSettingName];
            }

            viewContext.TempData = new TempDataDictionary();

            HtmlHelper htmlHelper = new HtmlHelper<object>(viewContext, new FakeViewDataContainer());
            var htmlString = htmlHelper.Action(actionName, controllerName, routeValues);
            return htmlString.ToHtmlString();
        }

        private RouteData CreateRouteData(string controllerName, string actionName, RouteValueDictionary routeValuesDic)
        {
            var routeData = new RouteData();

            var queryString = HttpContext.Current.Request.QueryString;
            foreach (string param in queryString)
                if (!string.IsNullOrEmpty(param) && !string.IsNullOrEmpty(queryString[param]))
                    routeData.Values[param] = queryString[param];

            foreach (var key in routeValuesDic.Keys)
            {
                routeData.Values[key] = routeValuesDic[key];
            }

            var moduleId = ModuleContext != null ? ModuleContext.ModuleId : -1;

            if (HttpContext.Current.Request.Params["moduleid"] != moduleId.ToString())
                routeData.Values["hccrequesttype"] = "hccget";
            else
                routeData.Values["hccrequesttype"] = "hccpost";

            //routeData.DataTokens["namespaces"] = new string[] { "Hotcakes.Modules.Core.Controllers" };

            if (ModuleContext != null)
                routeData.DataTokens["moduleContext"] = ModuleContext;

            return routeData;
        }

        private class FakeViewDataContainer : IViewDataContainer
        {
            private ViewDataDictionary viewData = new ViewDataDictionary();

            public ViewDataDictionary ViewData
            {
                get { return viewData; }
                set { viewData = value; }
            }
        }
    }
}