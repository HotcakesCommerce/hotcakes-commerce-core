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
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Hotcakes.Commerce.Extensions
{
    /// <summary>
    /// Defines the renderer used by HCC MVC form extension methods.
    /// </summary>
    /// <remarks>
    /// Implementations may render a virtual form rather than a conventional HTML form;
    /// the HCC GET and POST selector attributes use <see cref="VirtualFormUsed"/> to
    /// select the corresponding <c>hccrequesttype</c> route value.
    /// </remarks>
    public interface IHccFormRenderer
    {
        /// <summary>
        /// Gets a value indicating whether the most recent form rendering used a virtual form.
        /// </summary>
        bool VirtualFormUsed { get; }

        /// <summary>
        /// Renders an HCC form and returns its MVC form scope.
        /// </summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="formAction">The action URL or form target to render.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <param name="htmlAttributes">HTML attributes applied to the rendered form.</param>
        /// <returns>An <see cref="MvcForm"/> representing the rendered form.</returns>
        MvcForm FormHelper(HtmlHelper htmlHelper, string formAction, FormMethod method,
            IDictionary<string, object> htmlAttributes);
    }
}
