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
using System.Web.UI;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class Pager : HccUserControl, IPostBackEventHandler
    {
        #region Event handlers

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!string.IsNullOrEmpty(PageSizeSet))
            {
                Controls.Add(new LiteralControl(RenderPagerSetup()));
            }

            if (PageCount > 1)
            {
                var link = PostBackMode
                    ? "javascript:" + Page.ClientScript.GetPostBackEventReference(this, "{0}")
                    : Request.Path + "?page={0}";

                Controls.Add(new LiteralControl(
                    Paging.RenderPagerWithLimits(
                        link,
                        PageNumber,
                        PageCount,
                        MaxPagesToShow
                        )));
            }
        }

        #endregion

        #region Fields

        private bool _resetPageNumber;
        private const int MaxPagesToShow = 15;

        private Pager LinkedPager
        {
            get
            {
                if (!string.IsNullOrEmpty(LinkedPagerID))
                {
                    return (Pager) Parent.FindControl(LinkedPagerID);
                }

                return null;
            }
        }

        #endregion

        #region Properties

        public event EventHandler PageChanged;
        public bool PostBackMode = false;
        public string LinkedPagerID { get; set; }

        public int PageNumber
        {
            get
            {
                if (!_resetPageNumber)
                {
                    var page = PostBackMode ? ViewState["page"] as string : Request.Params["page"];
                    int num;

                    if (int.TryParse(page, out num))
                    {
                        return num;
                    }
                }
                return 1;
            }
        }

        public int PageSize
        {
            get { return (ViewState["PageSize"] ?? "10").ConvertTo(10); }
            set { ViewState["PageSize"] = value; }
        }

        public int PageCount
        {
            get { return (ViewState["pageCount"] ?? "0").ConvertTo(0); }
            set { ViewState["pageCount"] = value; }
        }

        public string PageSizeSet { get; set; }
        public string PageSizeAllCaption { get; set; }

        #endregion

        #region Public methods

        public void ResetPageNumber()
        {
            _resetPageNumber = true;
            if (PostBackMode) ViewState["page"] = "1";
        }

        public void SetRowCount(int totalRowCount)
        {
            UpdatePageCount(totalRowCount, PageSize);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            var args = eventArgument.Split(',');
            ViewState["page"] = args.First();

            if (args.Length > 1)
            {
                var pSize = args[1].ConvertTo(0);
                PageSize = pSize > 0 ? pSize : int.MaxValue;
            }

            if (PageChanged != null)
                PageChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Implementation

        private string RenderPagerSetup()
        {
            var pSize = PageSize;
            var strPageSize = pSize == int.MaxValue ? "0" : pSize.ToString();
            var pageSizeSet = (PageSizeSet ?? "").Split(',');
            var sb = new StringBuilder();
            sb.Append("<div class=\"hcPagerSetup\"><ul>");
            foreach (var si in pageSizeSet)
            {
                var link = Page.ClientScript.GetPostBackEventReference(this, "1," + si);
                sb.Append(strPageSize == si ? "<li class=\"hcCurrent\">" : "<li>");
                sb.AppendFormat("<a href=\"javascript:{0}\">{1}</a>", link, GetPSItemCaption(si));
                sb.Append("</li>");
            }
            sb.Append("</ul></div>");

            return sb.ToString();
        }

        private string GetPSItemCaption(string val)
        {
            if (val != "0") return val;
            return PageSizeAllCaption ?? Localization.GetString("All");
        }

        private void UpdatePageCount(int totalRowCount, int pageSize)
        {
            PageCount = Paging.TotalPages(totalRowCount, pageSize);

            if (LinkedPager != null)
            {
                LinkedPager.UpdatePageCount(totalRowCount, pageSize);
            }
        }

        #endregion
    }
}