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
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using DotNetNuke.Framework;
using DotNetNuke.UI.Modules;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Filters;

namespace Hotcakes.Modules.Core.Controllers.Shared
{
    [StoreClosedFilter]
    [Serializable]
    public class BaseAppController : Controller
    {
        public HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        public ModuleInstanceContext ModuleContext
        {
            get
            {
                var dataTokens = Request.RequestContext.RouteData.DataTokens;
                return dataTokens["moduleContext"] as ModuleInstanceContext;
            }
        }

        public string ModuleViewName
        {
            get
            {
                var parentViewContext = ControllerContext.ParentActionViewContext;
                if (parentViewContext != null && parentViewContext.ParentActionViewContext == null)
                {
                    if (!string.IsNullOrEmpty((string) parentViewContext.ViewData["moduleViewName"]))
                        return (string) parentViewContext.ViewData["moduleViewName"];
                }
                return null;
            }
        }

        public ILocalizationHelper Localization { get; set; }

        private CultureSwitch ActionCultureSwitch { get; set; }

        private CultureSwitch ResultCultureSwitch { get; set; }

        private CDefault BasePage
        {
            get { return HttpContext.Handler as CDefault; }
        }

        public string PageTitle
        {
            get
            {
                if (BasePage != null)
                    return BasePage.Title;
                return string.Empty;
            }
            set
            {
                if (BasePage != null)
                    BasePage.Title = value;
            }
        }

        public string PageDescription
        {
            get
            {
                if (BasePage != null)
                    return BasePage.Description;
                return string.Empty;
            }
            set
            {
                if (BasePage != null)
                    BasePage.Description = value;
            }
        }

        public string PageKeywords
        {
            get
            {
                if (BasePage != null)
                    return BasePage.KeyWords;
                return string.Empty;
            }
            set
            {
                if (BasePage != null)
                    BasePage.KeyWords = value;
            }
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            HccRequestContextUtils.UpdateUserContentCulture(HccRequestContext.Current);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            SetSessionGuidCookies();

            ActionCultureSwitch = new CultureSwitch(HccApp.CurrentStore);

            InitializeLocalization();

            ViewBag.RootUrlSecure = HccApp.StoreUrl(true, false);
            ViewBag.RootUrl = HccApp.StoreUrl(false, true);

            ViewBag.StoreClosed = HccApp.CurrentStore.Settings.StoreClosed;
            ViewBag.StoreName = HccApp.CurrentStore.Settings.FriendlyName;

            // Save current URL for facebook like, etc.
            ViewBag.RawUrl = Request.Url.ToString();
            ViewBag.CurrentUrl = GetOriginalUrl();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            ActionCultureSwitch.RollbackCulture();
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);

            ResultCultureSwitch = new CultureSwitch(HccApp.CurrentStore);
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            ResultCultureSwitch.RollbackCulture();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            if (filterContext.Exception is StoreException)
            {
                var storeException = filterContext.Exception as StoreException;

                ViewBag.ErrorHeader = storeException.Header;
                ViewBag.ErrorContent = storeException.Content;
                var cssClass = string.Empty;
                switch (storeException.Type)
                {
                    case ViewMessageType.Info:
                        cssClass = "dnnFormMessage";
                        break;
                    case ViewMessageType.Success:
                        cssClass = "dnnFormMessage dnnFormSuccess";
                        break;
                    case ViewMessageType.Error:
                        cssClass = "dnnFormMessage dnnFormError";
                        break;
                    case ViewMessageType.Warning:
                        cssClass = "dnnFormMessage dnnFormWarning";
                        break;
                    default:
                        cssClass = "dnnFormMessage";
                        break;
                }
                ViewBag.CssClass = cssClass;

                filterContext.ExceptionHandled = true;
                filterContext.Result = PartialView("_Error");
            }
        }

        private void SetSessionGuidCookies()
        {
            var sessionGuid = SessionManager.GetCurrentSessionGuid();
            if (sessionGuid == null)
            {
                SessionManager.SetCurrentSessionGuid(Guid.NewGuid());
            }

            var shoppingSessionGuid = SessionManager.GetCurrentShoppingSessionGuid();
            if (shoppingSessionGuid == null)
            {
                SessionManager.SetCurrentShoppingSessionGuid(Guid.NewGuid());
            }
        }

        protected void FlashInfo(string message)
        {
            FlashMessage(message, "");
        }

        protected void FlashSuccess(string message)
        {
            FlashMessage(message, "dnnFormSuccess");
        }

        protected void FlashFailure(string message)
        {
            FlashMessage(message, "dnnFormError");
        }

        protected void FlashWarning(string message)
        {
            FlashMessage(message, "dnnFormWarning");
        }

        private void FlashMessage(string message, string typeClass)
        {
            var format = "<div class=\"dnnFormMessage {0}\">{1}</div>";
            TempData["messages"] += string.Format(format, typeClass, message);
        }

        protected override RedirectResult Redirect(string url)
        {
            if (Factory.CreateHccFormRenderer().VirtualFormUsed)
            {
                return new HccRedirectResult(url);
            }
            return base.Redirect(url);
        }

        protected void InitializeLocalization()
        {
            string localResourceFile;
            // area have to be from RouteData.DataTokens
            // controller have to be from RouteData.Values
            var areaName = (string) RouteData.DataTokens["area"];
            var controllerName = (string) RouteData.Values["controller"];

            if (string.IsNullOrEmpty(areaName))
            {
                localResourceFile = string.Format("{0}/Views/{1}/App_LocalResources/Controller.resx",
                    HccApp.ViewsVirtualPath, controllerName);
            }
            else
            {
                localResourceFile = string.Format("{0}/Areas/{1}/Views/{2}/App_LocalResources/Controller.resx",
                    HccApp.ViewsVirtualPath, areaName, controllerName);
            }
            Localization = Factory.Instance.CreateLocalizationHelper(localResourceFile);
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ModuleViewName;
            return base.View(viewName, masterName, model);
        }

        protected string GetValidationSummaryMessage()
        {
            var sb = new StringBuilder();
            ModelState.Values.ToList()
                .ForEach(v => { v.Errors.ToList().ForEach(e => sb.AppendFormat("{0} <br/>", e.ErrorMessage)); });
            return sb.ToString();
        }

        protected JsonResult GetStatusMessage(string message, bool isSuccess, string redirectUrl = "")
        {
            return Json(new {Status = isSuccess ? "OK" : "Failed", Message = message, RedirectUrl = redirectUrl});
        }

        public void RenderToHead(string key, string content)
        {
            RenderToElement(key, content, "Head");
        }

        public void RenderToBody(string key, string content)
        {
            RenderToElement(key, content, "ClientResourceIncludes");
        }

        private void RenderToElement(string key, string content, string controlId, bool injectToTop = false)
        {
            if (!string.IsNullOrEmpty(content))
            {
                if (BasePage != null)
                {
                    if (BasePage.Items.Contains(key))
                        return;

                    var element = BasePage.FindControl(controlId);
                    if (element != null)
                    {
                        var literal = new LiteralControl {Text = content};
                        if (injectToTop)
                        {
                            element.Controls.AddAt(0, literal);
                        }
                        else
                        {
                            element.Controls.Add(literal);
                        }
                        BasePage.Items[key] = true;
                    }
                }
            }
        }

        private string GetOriginalUrl()
        {
            var originalUrl = HttpContext.Items["UrlRewrite:OriginalUrl"] as string;
            return originalUrl ?? Request.RawUrl;
        }
    }
}