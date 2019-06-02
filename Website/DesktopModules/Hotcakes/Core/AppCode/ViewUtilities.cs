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
using System.IO;
using System.Web;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Modules.Core.AppCode
{
    [Serializable]
    public class ViewUtilities
    {
        public static bool DownloadFile(ProductFile file, HotcakesApplication app)
        {
            var extension = Path.GetExtension(file.FileName);
            var name = Path.GetFileName(file.FileName);
            double fileSize = 0;

            var storeId = app.CurrentRequestContext.CurrentStore.Id;
            var diskFileName = file.Bvin + "_" + file.FileName + ".config";
            if (!DiskStorage.FileVaultFileExists(storeId, diskFileName)) return false;

            var bytes = DiskStorage.FileVaultGetBytes(storeId, diskFileName);

            var type = MimeTypes.FindTypeForExtension(extension);

            fileSize = bytes.Length;

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();

            if (!string.IsNullOrEmpty(type))
            {
                HttpContext.Current.Response.ContentType = type;
            }

            HttpContext.Current.Response.AppendHeader("content-disposition", "attachment; filename=" + name);

            if (fileSize > 0)
            {
                HttpContext.Current.Response.AddHeader("Content-Length", fileSize.ToString());
            }

            HttpContext.Current.Response.BinaryWrite(bytes);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

            return true;
        }
    }
}