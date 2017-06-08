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

using System;
using System.Drawing;
using System.IO;
using ImageResizer;

namespace Hotcakes.Web
{
    [Serializable]
    public class Images
    {
        public static Size GetNewDimensions(int maxwidth, int maxheight, ref Bitmap originalImage)
        {
            int height;
            int width;
            height = originalImage.Height;
            width = originalImage.Width;
            return GetNewDimensions(maxwidth, maxheight, width, height);
        }

        public static Size GetNewDimensions(int maxwidth, int maxheight, int width, int height)
        {
            float multiplier;

            var result = new Size(width, height);

            // both dimensions are within size
            if (height <= maxheight && width <= maxwidth)
                return result;

            multiplier = maxwidth/(float) width;

            if (height*multiplier <= maxheight)
            {
                height = (int) (height*multiplier);
                return new Size(maxwidth, height);
            }

            multiplier = maxheight/(float) height;
            width = (int) (width*multiplier);
            return new Size(width, maxheight);
        }

        public static void ShrinkImageFileOnDisk(string originalFile, string newFileName, int maxWidth, int maxHeight)
        {
            var tempFile = TempFiles.GetTemporaryFileInfo();
            var settings = new ResizeSettings(maxWidth, maxHeight, FitMode.Max, null);
            ImageBuilder.Current.Build(originalFile, tempFile.FullName, settings);

            if (File.Exists(newFileName))
            {
                File.SetAttributes(newFileName, FileAttributes.Normal);
                File.Delete(newFileName);
            }
            File.Copy(tempFile.FullName, newFileName);
        }
    }
}