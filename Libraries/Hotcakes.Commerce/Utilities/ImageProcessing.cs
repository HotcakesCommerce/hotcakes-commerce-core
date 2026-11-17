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
using System.IO;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Creates standard-size derivatives of image files.
    /// </summary>
    /// <remarks>
    /// Derivatives are written beside the source image in conventional subdirectories when applicable. Failures are
    /// logged through the Hotcakes event log and reported as <see langword="false"/>.
    /// </remarks>
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

        /// <summary>
        /// Creates a derivative no larger than 50 by 50 pixels in the <c>tiny</c> subdirectory.
        /// </summary>
        /// <param name="originalFile">The source image file.</param>
        /// <returns><see langword="true"/> when the derivative is created; otherwise, <see langword="false"/>.</returns>
        public static bool ShrinkToTiny(string originalFile)
        {
            return ShrinkImage(originalFile, "tiny", TINYWIDTH, TINYHEIGHT);
        }

        /// <summary>
        /// Creates a derivative no larger than 220 by 220 pixels in the <c>small</c> subdirectory.
        /// </summary>
        /// <param name="originalFile">The source image file.</param>
        /// <returns><see langword="true"/> when the derivative is created; otherwise, <see langword="false"/>.</returns>
        public static bool ShrinkToSmall(string originalFile)
        {
            return ShrinkImage(originalFile, "small", SMALLWIDTH, SMALLHEIGHT);
        }

        /// <summary>
        /// Creates a derivative no larger than 440 by 440 pixels in the <c>medium</c> subdirectory.
        /// </summary>
        /// <param name="originalFile">The source image file.</param>
        /// <returns><see langword="true"/> when the derivative is created; otherwise, <see langword="false"/>.</returns>
        public static bool ShrinkToMedium(string originalFile)
        {
            return ShrinkImage(originalFile, "medium", MEDIUMWIDTH, MEDIUMHEIGHT);
        }

        /// <summary>
        /// Creates a derivative no larger than 310 by 110 pixels in the <c>logo</c> subdirectory.
        /// </summary>
        /// <param name="originalFile">The source image file.</param>
        /// <returns><see langword="true"/> when the derivative is created; otherwise, <see langword="false"/>.</returns>
        public static bool ShrinkToLogo(string originalFile)
        {
            return ShrinkImage(originalFile, "logo", LOGOWIDTH, LOGOHEIGHT);
        }

        /// <summary>
        /// Creates a banner derivative using the source image directory and the implementation's unrestricted banner dimensions.
        /// </summary>
        /// <param name="originalFile">The source image file.</param>
        /// <returns><see langword="true"/> when the derivative is created; otherwise, <see langword="false"/>.</returns>
        public static bool ShrinkToBanner(string originalFile)
        {
            return ShrinkImage(originalFile, "banner", BANNERWIDTH, BANNERHEIGHT);
        }

        /// <summary>
        /// Creates a 160 by 120 theme-preview derivative beside the source image.
        /// </summary>
        /// <param name="originalFile">The source image file.</param>
        /// <returns><see langword="true"/> when the derivative is created; otherwise, <see langword="false"/>.</returns>
        public static bool ShrinkToThemePreview(string originalFile)
        {
            return ShrinkImage(originalFile, "", THEMEPREVIEWWIDTH, THEMEPREVIEWHEIGHT);
        }

        /// <summary>
        /// Creates a 640 by 480 theme-preview derivative beside the source image.
        /// </summary>
        /// <param name="originalFile">The source image file.</param>
        /// <returns><see langword="true"/> when the derivative is created; otherwise, <see langword="false"/>.</returns>
        public static bool ShrinkToThemePreviewBig(string originalFile)
        {
            return ShrinkImage(originalFile, "", THEMEPREVIEWBIGWIDTH, THEMEPREVIEWBIGHEIGHT);
        }
    }
}
