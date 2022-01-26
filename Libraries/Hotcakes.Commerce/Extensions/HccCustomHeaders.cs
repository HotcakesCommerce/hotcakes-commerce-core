#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2021 Upendo Ventures, LLC
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
using System.Web.Mvc;

namespace Hotcakes.Commerce.Extensions
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class HccCustomHeaders : System.Web.Mvc.ActionFilterAttribute
    {
        private const string HEADER_CACHE_VALUE = "no-store, must-revalidate";
        private const string HEADER_PRAGMA_KEY = "Pragma";
        private const string HEADER_PRAGMA_VALUE = "no-cache";
        private const string HEADER_EXPIRES_KEY = "Expires";
        private const string HEADER_EXPIRES_VALUE = "0";

        [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            context.RequestContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.RequestContext.HttpContext.Response.Cache.AppendCacheExtension(HEADER_CACHE_VALUE);
            context.RequestContext.HttpContext.Response.AppendHeader(HEADER_PRAGMA_KEY, HEADER_PRAGMA_VALUE);
            context.RequestContext.HttpContext.Response.AppendHeader(HEADER_EXPIRES_KEY, HEADER_EXPIRES_VALUE);

            base.OnActionExecuted(context);
        }
    }
}
