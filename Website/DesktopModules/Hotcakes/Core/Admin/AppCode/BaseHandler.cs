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
using System.Web;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public abstract class BaseHandler : IHttpHandler
    {
        private string _localResourceFile;

        public virtual string LocalResourceFile
        {
            get
            {
                if (string.IsNullOrEmpty(_localResourceFile))
                {
                    var relativePath = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath;
                    var index = relativePath.LastIndexOf('/');
                    _localResourceFile = relativePath.Insert(index + 1, "App_LocalResources/") + ".resx";
                }
                return _localResourceFile;
            }
            set { _localResourceFile = value; }
        }

        public ILocalizationHelper Localization { get; set; }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            Localization = Factory.Instance.CreateLocalizationHelper(LocalResourceFile);

            HccRequestContextUtils.UpdateAdminContentCulture(HccRequestContext.Current);

            CultureSwitch.SetCulture(HccRequestContext.Current.CurrentStore);

            var res = HandleAction(context, HotcakesApplication.Current);
            context.Response.ContentType = "application/json";
            context.Response.Write(res.ObjectToJson());
        }

        protected virtual object HandleAction(HttpContext context, HotcakesApplication hccApp)
        {
            return HandleAction(context.Request, hccApp);
        }

        protected virtual object HandleAction(HttpRequest request, HotcakesApplication hccApp)
        {
            return null;
        }
    }
}