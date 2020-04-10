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

using System.Web;
using System.Web.SessionState;
using Hotcakes.Commerce;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    /// <summary>
    ///     Summary description for ProductsHandler
    /// </summary>
    public class ProductsHandler : BaseHandler, IHttpHandler, IReadOnlySessionState
    {
        protected override object HandleAction(HttpContext context, HotcakesApplication hccApp)
        {
            if (context.User.Identity.IsAuthenticated == false)
            {
                // not found
                context.Response.StatusCode = 404;
                context.Response.End();
                return null;
            }

            var method = context.Request.Params["method"];

            switch (method)
            {
                case "GetImportProgress":
                    return GetImportProgress(context, hccApp);
                default:
                    break;
            }
            return true;
        }

        private object GetImportProgress(HttpContext context, HotcakesApplication hccApp)
        {
            var manager = new SessionManager(context.Session);
            var returnObj = new object();
            lock (manager.AdminProductImportLog)
            {
                returnObj = new
                {
                    Progress = manager.AdminProductImportProgress,
                    Log = string.Join("\n", manager.AdminProductImportLog)
                };
            }
            return returnObj;
        }
    }
}