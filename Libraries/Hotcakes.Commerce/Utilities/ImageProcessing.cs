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
using Hotcakes.Web;

namespace Hotcakes.Commerce.Utilities
{
    public class ImageProcessing
    {
        private const int TINYWIDTH = 50;
        private const int TINYHEIGHT = 50;
        private const int SMALLWIDTH = 220;
        private const int SMALLHEIGHT = 220;
        private const int MEDIUMWIDTH = 440;
        private const int MEDIUMHEIGHT = 440;
        private const int LOGOWIDTH = 310;
        private const int LOGOHEIGHT = 110;
        private const int BANNERWIDTH = int.MaxValue; //700;
        private const int BANNERHEIGHT = int.MaxValue; //400;
        private const int THEMEPREVIEWHEIGHT = 120;
        private const int THEMEPREVIEWWIDTH = 160;
        private const int THEMEPREVIEWBIGHEIGHT = 480;
        private const int THEMEPREVIEWBIGWIDTH = 640;

        private static bool ShrinkImage(string originalFile, string outputDirectory, int maxWidth, int maxHeight)
        {
            var result = true;

            try
            {
                var pathOfOriginal = Path.GetDirectoryName(originalFile);
                var pathOfOutput = Path.Combine(pathOfOriginal, outputDirectory);
                if (!Directory.Exists(pathOfOutput))
                {
                    Directory.CreateDirectory(pathOfOutput);
                }
                var outputFile = Path.Combine(pathOfOutput, Path.GetFileName(originalFile));
                Images.ShrinkImageFileOnDisk(originalFile, outputFile, maxWidth, maxHeight);
            }
            catch (Exception ex)
            {
                result = false;
                EventLog.LogEvent(ex);
            }

            return result;
        }

        public static bool ShrinkToTiny(string originalFile)
        {
            return ShrinkImage(originalFile, "tiny", TINYWIDTH, TINYHEIGHT);
        }

        public static bool ShrinkToSmall(string originalFile)
        {
            return ShrinkImage(originalFile, "small", SMALLWIDTH, SMALLHEIGHT);
        }

        public static bool ShrinkToMedium(string originalFile)
        {
            return ShrinkImage(originalFile, "medium", MEDIUMWIDTH, MEDIUMHEIGHT);
        }

        public static bool ShrinkToLogo(string originalFile)
        {
            return ShrinkImage(originalFile, "logo", LOGOWIDTH, LOGOHEIGHT);
        }

        public static bool ShrinkToBanner(string originalFile)
        {
            return ShrinkImage(originalFile, "banner", BANNERWIDTH, BANNERHEIGHT);
        }

        public static bool ShrinkToThemePreview(string originalFile)
        {
            return ShrinkImage(originalFile, "", THEMEPREVIEWWIDTH, THEMEPREVIEWHEIGHT);
        }

        public static bool ShrinkToThemePreviewBig(string originalFile)
        {
            return ShrinkImage(originalFile, "", THEMEPREVIEWBIGWIDTH, THEMEPREVIEWBIGHEIGHT);
        }
    }
}