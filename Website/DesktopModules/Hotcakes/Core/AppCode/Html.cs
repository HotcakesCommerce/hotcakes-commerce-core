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
using System.Text;

namespace Hotcakes.Modules.Core.AppCode
{
    [Serializable]
    public static class Html
    {
        private const string JQUERYFORMAT = "<script src=\"{0}jquery-3.5.1.min.js\" type=\"text/javascript\"></script>";
        private const string JQUERYMIGRATEFORMAT = "<script src=\"{0}jquery-migrate-3.3.0.min.js\" type=\"text/javascript\"></script>";
        private const string JQUERYUIFORMAT = "<script src=\"{0}jquery-ui-1.12.1/js/jquery-ui.min.js\" type=\"text/javascript\"></script>";
        private const string FOOTERFORMAT = "<div id=\"footer\"><div id=\"copyright\">&copy; Copyright 2013-2018 Hotcakes Commerce, LLC<br />&copy; Copyright 2019-{0} Upendo Ventures, LLC </div></div>";
        private const string SLASH = "/";

        public static string JQueryIncludes(string baseScriptFolder, bool IsSecure)
        {
            var sb = new StringBuilder();
            if (!baseScriptFolder.EndsWith(SLASH))
            {
                baseScriptFolder = string.Concat(baseScriptFolder, SLASH);
            }

            sb.AppendLine(string.Format(JQUERYFORMAT, baseScriptFolder));
            sb.AppendLine(string.Format(JQUERYMIGRATEFORMAT, baseScriptFolder));
            sb.AppendLine(string.Format(JQUERYUIFORMAT, baseScriptFolder));
            return sb.ToString();
        }

        public static string AdminFooter()
        {
            return string.Format(FOOTERFORMAT, DateTime.UtcNow.Year);
        }
    }
}