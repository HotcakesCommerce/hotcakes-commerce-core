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

using System;
using System.Collections.Generic;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Areas.AdminContent.Models
{
    [Serializable]
    public class FileManagerViewModel
    {
        public FileManagerViewModel(string path)
        {
            Path = path.TrimStart('\\');
            Directories = new List<string>();
            Files = new List<string>();
            BreadCrumbs = new BreadCrumbViewModel();
            BasePreviewUrl = string.Empty;
        }

        public BreadCrumbViewModel BreadCrumbs { get; set; }
        public List<string> Directories { get; set; }
        public List<string> Files { get; set; }
        public string BasePreviewUrl { get; set; }
        public string Path { get; set; }

        public string ParentPath
        {
            get
            {
                var potential = Path.TrimEnd('\\');
                var parent = string.Empty;
                if (potential.Length > 0)
                {
                    parent = System.IO.Path.GetDirectoryName(potential);
                }
                return parent;
            }
        }

        public bool AllowParentPath
        {
            get { return Path.Trim().Length > 0; }
        }

        public string ChildPath(string directoryName)
        {
            return Path + "\\" + directoryName;
        }

        public string PreviewUrl(string fileName)
        {
            return BasePreviewUrl + RelativeFileUrl(fileName);
        }

        public string RelativeFileUrl(string fileName)
        {
            var result = Path;
            result = result.Replace("\\", "/");
            result = result.TrimStart('/');
            result = result.TrimEnd('/');
            result = result + "/" + fileName;

            return result;
        }
    }
}