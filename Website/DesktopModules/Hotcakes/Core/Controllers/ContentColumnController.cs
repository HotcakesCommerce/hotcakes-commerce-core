﻿#region License

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
using System.Web.Mvc;
using Hotcakes.Modules.Core.Controllers.Shared;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class ContentColumnController : BaseAppController
    {
        [ChildActionOnly]
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content(string.Empty);

            var col = HccApp.ContentServices.Columns.Find(id);
            if (col == null)
            {
                col = HccApp.ContentServices.Columns.FindByDisplayName(id);
            }
            if (col == null)
                return Content(string.Empty); // Content("Column Not Found");

            return View(col);
        }
    }
}