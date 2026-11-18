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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hotcakes.Web
{
    /// <summary>
    /// Provides file-system path and file-listing helpers for Hotcakes view resources.
    /// </summary>
    /// <seealso cref="ListFiles(string, string, string)"/>
    [Serializable]
    public static class PathHelper
    {
        /// <summary>
        /// Gets the virtual path containing the default Hotcakes views.
        /// </summary>
        public const string ViewsVirtualPath = "~/Portals/_default/HotcakesViews/";

        /// <summary>
        /// Lists files in a folder matching one include pattern and not matching the exclude pattern.
        /// </summary>
        /// <param name="folderPath">The folder to search.</param>
        /// <param name="includePattern">The file-name pattern to include.</param>
        /// <param name="excludePattern">The file-name pattern to exclude.</param>
        /// <returns>The matching file paths, or an empty list when the folder does not exist.</returns>
        /// <seealso cref="ListFiles(string, string[], string[])"/>
        public static List<string> ListFiles(string folderPath, string includePattern, string excludePattern)
        {
            return ListFiles(folderPath, new[] {includePattern}, new[] {excludePattern});
        }

        /// <summary>
        /// Lists files in a folder matching any include pattern and none of the exclude patterns.
        /// </summary>
        /// <param name="folderPath">The folder to search.</param>
        /// <param name="includePatterns">The file-name patterns to include.</param>
        /// <param name="excludePatterns">The file-name patterns to exclude.</param>
        /// <returns>The matching file paths, or an empty list when the folder does not exist.</returns>
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
