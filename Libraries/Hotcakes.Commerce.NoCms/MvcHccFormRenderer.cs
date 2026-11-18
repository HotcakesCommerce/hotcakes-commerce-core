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

using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Hotcakes.Commerce.Extensions;

namespace Hotcakes.Commerce.NoCms
{
    /// <summary>
    /// Renders HCC forms through the ASP.NET MVC HTML helper implementation.
    /// </summary>
    /// <remarks>
    /// This adapter invokes MVC's established form helper and does not use a virtual form.
    /// </remarks>
    public class MvcHccFormRenderer : IHccFormRenderer
    {
        /// <summary>
        /// Renders an HCC form using the MVC form helper.
        /// </summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="formAction">The action URL or form target to render.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <param name="htmlAttributes">The HTML attributes applied to the rendered form.</param>
        /// <returns>An <see cref="MvcForm"/> representing the rendered form scope.</returns>
        /// <remarks>The helper is resolved through MVC's non-public <c>FormHelper</c> method to preserve the established rendering behavior.</remarks>
        public MvcForm FormHelper(HtmlHelper htmlHelper, string formAction, FormMethod method,
            IDictionary<string, object> htmlAttributes)
        {
            var methodInfo = typeof (FormExtensions).GetMethod("FormHelper",
                BindingFlags.Static | BindingFlags.NonPublic);
            return (MvcForm) methodInfo.Invoke(null, new object[] {htmlHelper, formAction, method, htmlAttributes});
        }


        /// <summary>
        /// Gets a value indicating whether the most recent form rendering used a virtual form.
        /// </summary>
        /// <value>Always <see langword="false"/> for the MVC renderer.</value>
        public bool VirtualFormUsed
        {
            get { return false; }
        }
    }
}
