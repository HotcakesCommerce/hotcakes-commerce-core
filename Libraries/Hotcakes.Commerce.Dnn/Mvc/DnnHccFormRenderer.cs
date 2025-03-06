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
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Hotcakes.Commerce.Extensions;

namespace Hotcakes.Commerce.Dnn.Mvc
{
    [Serializable]
    public class DnnHccFormRenderer : IHccFormRenderer
    {
        public MvcForm FormHelper(HtmlHelper htmlHelper, string formAction, FormMethod method,
            IDictionary<string, object> htmlAttributes)
        {
            var page = htmlHelper.ViewDataContainer as IHccWebViewPage;
            var moduleId = -1;
            if (page != null && page.ModuleContext != null)
                moduleId = page.ModuleContext.ModuleId;

            var builder = new TagBuilder("div");
            builder.MergeAttributes(htmlAttributes);
            builder.MergeAttribute("data-type", "form");
            builder.MergeAttribute("data-action", formAction);
            builder.MergeAttribute("data-method", HtmlHelper.GetFormMethodString(method));
            builder.MergeAttribute("data-moduleid", moduleId.ToString());

            // We have ClientValidationEnabled and UnobtrusiveJavaScriptEnabled settings set to true
            // so we can ignore this
            //bool flag = htmlHelper.ViewContext.ClientValidationEnabled && !htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled;
            //if (flag)
            //{
            //    builder.GenerateId(htmlHelper.ViewContext.FormIdGenerator());
            //}
            htmlHelper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.StartTag));
            var form = new HccMvcForm(htmlHelper.ViewContext);
            //if (flag)
            //{
            //    htmlHelper.ViewContext.FormContext.FormId = builder.Attributes["id"];
            //}
            return form;
        }


        public bool VirtualFormUsed
        {
            get { return true; }
        }
    }
}