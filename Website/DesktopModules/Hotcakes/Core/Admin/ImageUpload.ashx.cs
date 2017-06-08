#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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

using System.IO;
using System.Web;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin
{
    /// <summary>
    ///     Summary description for ImageUpload
    /// </summary>
    public class ImageUpload : BaseHandler
    {
        protected override object HandleAction(HttpRequest request, HotcakesApplication hccApp)
        {
            if (request.Files.Count > 0)
            {
                var file = request.Files[0];
                var path = DiskStorage.UploadTempImage(hccApp.CurrentStore.Id, file);

                if (!string.IsNullOrEmpty(path))
                {
                    path = "~/" + path.Replace(request.PhysicalApplicationPath, "").Replace("\\", "/");

                    return new TempImage
                    {
                        FileName = Path.GetFileName(file.FileName),
                        TempFileName = VirtualPathUtility.ToAbsolute(path)
                    };
                }
                return new TempImage {Message = "Only .PNG, .JPG, .GIF file types are allowed"};
            }

            return new TempImage {Message = "Unknonw Error"};
        }

        public class TempImage
        {
            public string FileName { get; set; }
            public string TempFileName { get; set; }
            public string Message { get; set; }
        }
    }
}