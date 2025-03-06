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
using System.IO;
using System.Linq;

namespace Hotcakes.Web
{
    [Serializable]
    public static class PathHelper
    {
        public const string ViewsVirtualPath = "~/Portals/_default/HotcakesViews/";

        public static List<string> ListFiles(string folderPath, string includePattern, string excludePattern)
        {
            return ListFiles(folderPath, new[] {includePattern}, new[] {excludePattern});
        }

        public static List<string> ListFiles(string folderPath, string[] includePatterns, string[] excludePatterns)
        {
            var result = new List<string>();
            if (Directory.Exists(folderPath))
            {
                var fileNamesInclude = new List<string>();
                foreach (var includePattern in includePatterns)
                {
                    fileNamesInclude.AddRange(Directory.EnumerateFiles(folderPath, includePattern));
                }
                var fileNamesExclude = new List<string>();
                foreach (var excludePattern in excludePatterns)
                {
                    fileNamesExclude.AddRange(Directory.EnumerateFiles(folderPath, excludePattern));
                }
                return fileNamesInclude.Except(fileNamesExclude).ToList();
            }
            return result;
        }
    }
}