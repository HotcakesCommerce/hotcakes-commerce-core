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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Storage;
using Hotcakes.Web;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    ///     This class helps generate HTML, paths, and other information for images used in the application
    /// </summary>
    [Serializable]
    public static class ImageHelper
    {
        public static string SafeImage(string imagePath)
        {
            var result = string.Empty;

            if (imagePath.ToLower().StartsWith("http:"))
            {
                result = imagePath;
            }
            else
            {
                if (HttpContext.Current != null)
                {
                    if (File.Exists(HttpContext.Current.Server.MapPath(imagePath)))
                    {
                        result = imagePath;
                    }
                }
            }

            return result;
        }

        public static string GetValidImage(string fileName, bool useAppRoot)
        {
            var result = "DesktopModules/Hotcakes/Core/Admin/Images/NoImageAvailable.gif";
            if (fileName.Length != 0)
            {
                // Allow direct URLs as file names
                if ((fileName.ToLower().StartsWith("http://") == false) &
                    (fileName.ToLower().StartsWith("https://") == false))
                {
                    var fileNameReplaced = fileName.Replace("/", "\\");
                    if (fileNameReplaced.StartsWith("\\"))
                    {
                        fileNameReplaced = fileNameReplaced.Remove(0, 1);
                    }
                    if (File.Exists(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, fileNameReplaced)))
                    {
                        result = fileName.Replace("\\", "/");
                    }
                    if (useAppRoot)
                    {
                        if (result.StartsWith("/"))
                        {
                            result = "~" + result;
                        }
                        else
                        {
                            result = "~/" + result;
                        }
                    }
                }
                else
                {
                    result = fileName.Replace("\\", "/");
                }
            }
            else
            {
                if (useAppRoot)
                {
                    result = "~/" + result;
                }
            }

            return result;
        }

        public static ImageInfo GetImageInformation(string fileName)
        {
            var result = new ImageInfo();
            result.Width = 0;
            result.Height = 0;
            result.FormattedDimensions = "unknown";
            result.FormattedSize = "unknown";
            result.SizeInBytes = 0;

            var fullName = string.Empty;
            if (fileName != null)
            {
                fullName = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, fileName.Replace("/", "\\"));
            }


            if (File.Exists(fullName))
            {
                var f = new FileInfo(Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, fileName));
                if (f != null)
                {
                    result.SizeInBytes = f.Length;
                    if (result.SizeInBytes < 1024)
                    {
                        result.FormattedSize = result.SizeInBytes + " bytes";
                    }
                    else
                    {
                        if (result.SizeInBytes < 1048576)
                        {
                            result.FormattedSize = Math.Round(result.SizeInBytes/1024, 1) + " KB";
                        }
                        else
                        {
                            result.FormattedSize = Math.Round(result.SizeInBytes/1048576, 1) + " MB";
                        }
                    }
                }
                f = null;

                if (File.Exists(fullName))
                {
                    Image WorkingImage;
                    WorkingImage = Image.FromFile(fullName);
                    if (WorkingImage != null)
                    {
                        result.Width = WorkingImage.Width;
                        result.Height = WorkingImage.Height;
                        result.FormattedDimensions = result.Width.ToString(CultureInfo.InvariantCulture) + " x " +
                                                     result.Height.ToString(CultureInfo.InvariantCulture);
                    }
                    WorkingImage.Dispose();
                    WorkingImage = null;
                }
            }

            return result;
        }

        public static ImageInfo GetProportionalImageDimensionsForImage(ImageInfo oldInfo, int maxWidth, int maxHeight)
        {
            var result = new ImageInfo();

            if (oldInfo != null)
            {
                var s = Images.GetNewDimensions(maxWidth, maxHeight, oldInfo.Width, oldInfo.Height);
                result.Height = s.Height;
                result.Width = s.Width;
            }

            return result;
        }

        public static bool CompressJpeg(string filePath, long quality)
        {
            var result = true;
            var fullFile = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, filePath);

            if (File.Exists(fullFile))
            {
                if (quality < 1L)
                {
                    quality = 1L;
                }
                else
                {
                    if (quality > 100L)
                    {
                        quality = 100L;
                    }
                }

                Image WorkingImage;
                WorkingImage = Image.FromFile(filePath);

                Image FinalImage;
                FinalImage = new Bitmap(WorkingImage.Width, WorkingImage.Height, PixelFormat.Format24bppRgb);

                var G = Graphics.FromImage(FinalImage);
                G.InterpolationMode = InterpolationMode.HighQualityBicubic;
                G.PixelOffsetMode = PixelOffsetMode.HighQuality;
                G.CompositingQuality = CompositingQuality.HighQuality;
                G.SmoothingMode = SmoothingMode.HighQuality;
                G.DrawImage(WorkingImage, 0, 0, WorkingImage.Width, WorkingImage.Height);

                // Dispose working Image so we can save with the same name
                WorkingImage.Dispose();
                WorkingImage = null;

                // Compression Code
                var myCodec = GetEncoderInfo("image/jpeg");
                var myEncoder = Encoder.Quality;
                var myEncoderParams = new EncoderParameters(1);
                var myParam = new EncoderParameter(myEncoder, quality);
                myEncoderParams.Param[0] = myParam;
                // End Compression Code

                File.Delete(fullFile);
                FinalImage.Save(fullFile, myCodec, myEncoderParams);
                FinalImage.Dispose();
                FinalImage = null;
            }
            else
            {
                result = false;
            }

            return result;
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j <= encoders.Length; j++)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
            return null;
        }

        public static bool ResizeImage(string currentImagePath, string newImagePath, int newHeight, int newWidth)
        {
            try
            {
                Images.ShrinkImageFileOnDisk(currentImagePath, newImagePath, newHeight, newWidth);
                return true;
            }
            catch (Exception Ex)
            {
                throw new ArgumentException("Image Resize Error: " + Ex.Message);
            }
        }

        /// <summary>
        ///     This method will iterate through the specified swatches and emit the requisite HTML for each
        /// </summary>
        /// <param name="p">Product - an instance of the product to find swatches for</param>
        /// <param name="app">HotcakesApplicateion - an instance of the current store application object</param>
        /// <returns>String - the HTML to be used for rendering the swatches</returns>
        /// <remarks>
        ///     If the swatch doesn't match an available swatch file, it will not be included in the HTML. Also, all swatch
        ///     images must be PNG or GIF.
        /// </remarks>
        public static string GenerateSwatchHtmlForProduct(Product p, HotcakesApplication app)
        {
            var result = string.Empty;

            if (app.CurrentStore.Settings.ProductEnableSwatches == false) return result;

            if (p.Options.Count > 0)
            {
                var found = false;

                var swatchBase = DiskStorage.GetStoreDataUrl(app, app.IsCurrentRequestSecure());
                swatchBase += "swatches";
                if (p.SwatchPath.Length > 0) swatchBase += "/" + p.SwatchPath;

                var swatchPhysicalBase = DiskStorage.GetStoreDataPhysicalPath(app.CurrentStore.Id);
                swatchPhysicalBase += "swatches\\";
                if (p.SwatchPath.Length > 0) swatchPhysicalBase += p.SwatchPath + "\\";

                var sb = new StringBuilder();
                sb.Append("<div class=\"productswatches\">");
                foreach (var opt in p.Options)
                {
                    if (opt.IsColorSwatch)
                    {
                        found = true;
                        foreach (var oi in opt.Items)
                        {
                            if (oi.IsLabel) continue;

                            var prefix = CleanSwatchName(oi.Name);

                            if (File.Exists(swatchPhysicalBase + prefix + ".png"))
                            {
                                sb.Append("<img width=\"18\" height=\"18\" src=\"" + swatchBase + "/" + prefix +
                                          ".png\" border=\"0\" alt=\"" + prefix + "\" />");
                            }
                            else
                            {
                                sb.Append("<img width=\"18\" height=\"18\" src=\"" + swatchBase + "/" + prefix +
                                          ".gif\" border=\"0\" alt=\"" + prefix + "\" />");
                            }
                        }
                    }
                }
                sb.Append("</div>");

                if (found)
                {
                    result = sb.ToString();
                }
            }

            return result;
        }

        private static string CleanSwatchName(string source)
        {
            var result = source.Replace(" ", "_");

            result = result.Replace("/", string.Empty);
            result = result.Replace("\"", string.Empty);
            result = result.Replace("//", string.Empty);
            result = result.Replace(":", string.Empty);
            result = result.Replace(";", string.Empty);
            result = result.Replace("'", string.Empty);
            result = result.Replace("!", string.Empty);
            result = result.Replace("~", string.Empty);
            result = result.Replace("@", string.Empty);
            result = result.Replace("#", string.Empty);
            result = result.Replace("$", string.Empty);
            result = result.Replace("%", string.Empty);
            result = result.Replace("^", string.Empty);
            result = result.Replace("&", string.Empty);
            result = result.Replace("*", string.Empty);
            result = result.Replace("(", string.Empty);
            result = result.Replace(")", string.Empty);
            result = result.Replace("[", string.Empty);
            result = result.Replace("]", string.Empty);
            result = result.Replace("{", string.Empty);
            result = result.Replace("}", string.Empty);
            result = result.Replace("|", string.Empty);
            result = result.Replace("-", string.Empty);
            result = result.Replace("+", string.Empty);
            result = result.Replace("<", string.Empty);
            result = result.Replace(">", string.Empty);
            result = result.Replace(".", string.Empty);
            result = result.Replace(",", string.Empty);
            result = result.Replace("?", string.Empty);
            result = result.Replace("=", string.Empty);
            result = result.Replace("`", string.Empty);

            return result;
        }
    }
}