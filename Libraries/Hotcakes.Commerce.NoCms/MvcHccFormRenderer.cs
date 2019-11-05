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

using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Hotcakes.Commerce.Extensions;

namespace Hotcakes.Commerce.NoCms
{
    public class MvcHccFormRenderer : IHccFormRenderer
    {
        public MvcForm FormHelper(HtmlHelper htmlHelper, string formAction, FormMethod method,
            IDictionary<string, object> htmlAttributes)
        {
            var methodInfo = typeof (FormExtensions).GetMethod("FormHelper",
                BindingFlags.Static | BindingFlags.NonPublic);
            return (MvcForm) methodInfo.Invoke(null, new object[] {htmlHelper, formAction, method, htmlAttributes});
        }


        public bool VirtualFormUsed
        {
            get { return false; }
        }
    }
}