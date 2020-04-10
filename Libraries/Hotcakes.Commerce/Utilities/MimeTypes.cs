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

namespace Hotcakes.Commerce.Utilities
{
    public class MimeTypes
    {
        public static string FindTypeForExtension(string extension)
        {
            var result = string.Empty;

            if (extension != null)
            {
                switch (extension.ToLower())
                {
                    case ".bin":
                    case ".exe":
                        result = "application/octet-stream";
                        break;
                    case ".htm":
                    case ".html":
                        result = "text/html";
                        break;
                    case ".txt":
                        result = "text/plain";
                        break;
                    case ".doc":
                    case ".docx":
                        result = "application/ms-word";
                        break;
                    case ".csv":
                    case ".xls":
                    case ".xlsx":
                        result = "application/ms-excel";
                        break;
                    case ".ppt":
                        result = "application/ms-powerpoint";
                        break;
                    case ".zip":
                        result = "application/x-zip";
                        break;
                    case ".css":
                        result = "text/css";
                        break;
                    case ".gif":
                        result = "image/gif";
                        break;
                    case ".jpeg":
                    case ".jpg":
                        result = "image/jpeg";
                        break;
                    case ".png":
                        result = "image/png";
                        break;
                    case ".tif":
                    case ".tiff":
                        result = "image/tiff";
                        break;
                    case ".pict":
                        result = "image/x-pict";
                        break;
                    case ".bmp":
                        result = "image/x-ms-bmp";
                        break;
                    case ".aif":
                    case ".aiff":
                    case ".aifc":
                        result = "audio/x-aiff";
                        break;
                    case ".wav":
                        result = "audio/x-wav";
                        break;
                    case ".mp3":
                        result = "audio/x-mpeg-3";
                        break;
                    case ".mpeg":
                    case ".mpg":
                    case ".mpe":
                        result = "video/mpeg";
                        break;
                    case ".avi":
                        result = "video/x-msvideo";
                        break;
                    case ".qt":
                    case ".mov":
                        result = "video/quicktime";
                        break;
                    case ".rtf":
                        result = "application/rtf";
                        break;
                    case ".pdf":
                        result = "application/pdf";
                        break;
                    case ".gtar":
                        result = "application/x-gtar";
                        break;
                    case ".tar":
                        result = "application/x-tar";
                        break;
                    case ".hqx":
                        result = "application/mac-binhex40";
                        break;
                    case ".sit":
                    case ".sea":
                        result = "application/x-stuffit";
                        break;
                }
            }

            return result;
        }
    }
}