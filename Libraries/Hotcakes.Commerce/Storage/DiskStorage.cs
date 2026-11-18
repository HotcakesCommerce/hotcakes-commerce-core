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
using System.Web;
using System.Web.Hosting;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Storage
{
    /// <summary>
    /// Provides static helpers for store-scoped URLs, paths, images, uploads, file vaults, and file-manager operations.
    /// </summary>
    /// <remarks>
    /// The helpers preserve the established separation between store virtual URLs and hosting-mapped physical paths. URL construction uses <see cref="GetStoreDataVirtualPath(long, string)"/>, while disk access uses <see cref="GetStoreDataPhysicalPath(long, string)"/>. They do not replace the underlying file, stream, image-processing, or path-cleaning behavior.
    /// </remarks>
    [Serializable]
    public class DiskStorage
    {
        private const string MISSING_IMAGE_URL = "DesktopModules/Hotcakes/Core/Admin/Images/MissingImage.png";
        private const string DEFAULT_STORE_LOGO_URL = "DesktopModules/Hotcakes/Core/Admin/Images/DefaultStoreLogo.png";

        /// <summary>
        /// Gets the physical path to the built-in demo image directory, including a trailing separator.
        /// </summary>
        /// <value>The hosting-mapped physical demo-image path.</value>
        public static string ApplicationBuiltinDemoImagesPath
        {
            get
            {
                var path = HostingEnvironment.MapPath(GetHccVirtualPath("core/content/demoimages"));
                if (!path.EndsWith("\\"))
                {
                    path += "\\";
                }
                return path;
            }
        }

        /// <summary>
        /// Validates an extension against the storage format allow-list.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="extension">The extension used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool ValidateImageType(string extension)
        {
            var result = false;

            switch (extension.ToUpperInvariant())
            {
                case ".PNG":
                    result = true;
                    break;
                case ".JPG":
                    result = true;
                    break;
                case ".GIF":
                    result = true;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Validates an extension against the storage format allow-list.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="extension">The extension used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool ValidateStoreAssetType(string extension)
        {
            var result = false;

            switch (extension.ToUpperInvariant())
            {
                case ".PNG":
                    result = true;
                    break;
                case ".JPG":
                    result = true;
                    break;
                case ".GIF":
                    result = true;
                    break;
                case ".PDF":
                    result = true;
                    break;
                case ".DOC":
                    result = true;
                    break;
                case ".DOCX":
                    result = true;
                    break;
                case ".TXT":
                    result = true;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Copies content into store storage and applies the existing image processing lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="imageName">The imageName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool CopyDemoProductImage(long storeId, string productId, string imageName)
        {
            try
            {
                var demoImagesPath = ApplicationBuiltinDemoImagesPath;
                var saveLocation = GetStoreDataPhysicalPath(storeId);
                saveLocation += "products\\" + productId + "\\";

                FileHelper.CopySingle(Path.Combine(demoImagesPath, "tiny"), Path.Combine(saveLocation, "tiny"), imageName, true);
                FileHelper.CopySingle(Path.Combine(demoImagesPath, "small"), Path.Combine(saveLocation, "small"), imageName, true);
                FileHelper.CopySingle(Path.Combine(demoImagesPath, "medium"), Path.Combine(saveLocation, "medium"), imageName, true);

                FileHelper.CopySingle(Path.Combine(demoImagesPath, "medium"), saveLocation, imageName, true);
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="productImage">The productImage used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ProductImageUrlOriginal(HotcakesApplication app, string productId, string productImage,
            bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(productImage))
                return MissingImageUrl(app, isSecure);

            // return outside image references without rewriting
            if (productImage.StartsWith("http"))
                return productImage;

            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("products/{0}/{1}", productId, productImage);
            return url;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="productImage">The productImage used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ProductImageUrlMedium(HotcakesApplication app, string productId, string productImage,
            bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(productImage))
                return MissingImageUrl(app, isSecure);

            // return outside image references without rewriting
            if (productImage.StartsWith("http"))
                return productImage;

            var url = GetStoreDataUrl(app, isSecure);

            if (productImage.Contains('/'))
            {
                // We have a user specified path or file name so don't 
                // automatically append the product BVIN to the name
                url += productImage.TrimStart('/');
            }
            else
            {
                url += string.Format("products/{0}/medium/{1}", productId, productImage);
            }
            return url;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="productImage">The productImage used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ProductImageUrlSmall(HotcakesApplication app, string productId, string productImage,
            bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(productImage))
                return MissingImageUrl(app, isSecure);

            // return outside image references without rewriting
            if (productImage.StartsWith("http"))
                return productImage;

            var url = GetStoreDataUrl(app, isSecure);

            if (productImage.Contains('/'))
            {
                // We have a user specified path or file name so don't 
                // automatically append the product BVIN to the name
                url += productImage.TrimStart('/');
            }
            else
            {
                url += string.Format("products/{0}/small/{1}", productId, productImage);
            }
            return url;
        }

        // Additional Image Urls
        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="imageId">The imageId used by this operation.</param>
        /// <param name="file">The file used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadAdditionalProductImage(long storeId, string productId, string imageId,
            HttpPostedFile file)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(file.FileName)))
            {
                return false;
            }

            var path = string.Format("products/{0}/additional/{1}/{2}", productId, imageId,
                Text.CleanFileName(Path.GetFileName(file.FileName)));
            var saveLocation = GetStoreDataPhysicalPath(storeId, path);

            // Delete the old one if it exists
            if (File.Exists(saveLocation))
            {
                File.SetAttributes(saveLocation, FileAttributes.Normal);
                File.Delete(saveLocation);
            }
            result = WriteFileToPath(saveLocation, file);

            if (result)
            {
                result = ImageProcessing.ShrinkToTiny(saveLocation);
                if (result)
                {
                    result = ImageProcessing.ShrinkToSmall(saveLocation);
                    if (result)
                    {
                        result = ImageProcessing.ShrinkToMedium(saveLocation);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="imageId">The imageId used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <param name="data">The data used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadProductAdditionalImage(long storeId, string productId, string imageId, string fileName,
            byte[] data)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(fileName)))
            {
                return false;
            }

            var path = string.Format("products/{0}/additional/{1}/{2}", productId, imageId,
                Text.CleanFileName(Path.GetFileName(fileName)));
            var saveLocation = GetStoreDataPhysicalPath(storeId, path);

            // Delete the old one if it exists
            if (File.Exists(saveLocation))
            {
                File.SetAttributes(saveLocation, FileAttributes.Normal);
                File.Delete(saveLocation);
            }
            result = WriteBytesToPath(saveLocation, data);

            if (result)
            {
                result = ImageProcessing.ShrinkToTiny(saveLocation);
                if (result)
                {
                    result = ImageProcessing.ShrinkToSmall(saveLocation);
                    if (result)
                    {
                        result = ImageProcessing.ShrinkToMedium(saveLocation);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Deletes the targeted store content and returns the implementation-defined success result.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool DeleteAdditionalProductImage(long storeId, string productId)
        {
            var result = true;

            var path = string.Format("products/{0}/additional", productId);
            var sourceFolder = GetStoreDataPhysicalPath(storeId, path);

            if (Directory.Exists(sourceFolder))
            {
                try
                {
                    FileHelper.DeleteDirectoryAndFilesRecursive(sourceFolder);
                    Directory.Delete(sourceFolder);
                }
                catch (Exception ex)
                {
                    result = false;
                    EventLog.LogEvent(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the targeted store content and returns the implementation-defined success result.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="imageId">The imageId used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool DeleteAdditionalProductImage(long storeId, string productId, string imageId)
        {
            var result = true;

            var path = string.Format("products/{0}/additional/{1}", productId, imageId);
            var sourceFolder = GetStoreDataPhysicalPath(storeId, path);

            if (Directory.Exists(sourceFolder))
            {
                try
                {
                    FileHelper.DeleteDirectoryAndFilesRecursive(sourceFolder);
                    Directory.Delete(sourceFolder);
                }
                catch (Exception ex)
                {
                    result = false;
                    EventLog.LogEvent(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="imageId">The imageId used by this operation.</param>
        /// <param name="productImage">The productImage used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ProductAdditionalImageUrlOriginal(HotcakesApplication app, string productId, string imageId,
            string productImage, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(productImage))
                return MissingImageUrl(app, isSecure);

            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("products/{0}/additional/{1}/{2}", productId, imageId, productImage);
            return url;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="imageId">The imageId used by this operation.</param>
        /// <param name="productImage">The productImage used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ProductAdditionalImageUrlMedium(HotcakesApplication app, string productId, string imageId,
            string productImage, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(productImage))
                return MissingImageUrl(app, isSecure);

            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("products/{0}/additional/{1}/medium/{2}", productId, imageId, productImage);
            return url;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="imageId">The imageId used by this operation.</param>
        /// <param name="productImage">The productImage used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ProductAdditionalImageUrlSmall(HotcakesApplication app, string productId, string imageId,
            string productImage, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(productImage))
                return MissingImageUrl(app, isSecure);

            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("products/{0}/additional/{1}/small/{2}", productId, imageId, productImage);
            return url;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="imageId">The imageId used by this operation.</param>
        /// <param name="productImage">The productImage used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ProductAdditionalImageUrlTiny(HotcakesApplication app, string productId, string imageId,
            string productImage, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(productImage))
                return MissingImageUrl(app, isSecure);

            if (productImage.StartsWith("http"))
                return productImage;

            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("products/{0}/additional/{1}/tiny/{2}", productId, imageId, productImage);
            return url;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="productImage">The productImage used by this operation.</param>
        /// <param name="variantId">The variantId used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ProductVariantImageUrlOriginal(HotcakesApplication app, string productId,
            string productImage, string variantId, bool isSecure)
        {
            var storeId = app.CurrentStore.Id;

            if (VariantImageExists(storeId, productId, variantId))
            {
                return ProductVariantImageUrl(app, productId, productImage, variantId, isSecure, string.Empty);
            }
            return ProductImageUrlOriginal(app, productId, productImage, isSecure);
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="productImage">The productImage used by this operation.</param>
        /// <param name="variantId">The variantId used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ProductVariantImageUrlMedium(HotcakesApplication app, string productId, string productImage,
            string variantId, bool isSecure)
        {
            var storeId = app.CurrentStore.Id;

            if (VariantImageExists(storeId, productId, variantId))
            {
                return ProductVariantImageUrl(app, productId, productImage, variantId, isSecure, "/medium");
            }
            return ProductImageUrlMedium(app, productId, productImage, isSecure);
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="productImage">The productImage used by this operation.</param>
        /// <param name="variantId">The variantId used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ProductVariantImageUrlSmall(HotcakesApplication app, string productId, string productImage,
            string variantId, bool isSecure)
        {
            var storeId = app.CurrentStore.Id;

            if (VariantImageExists(storeId, productId, variantId))
            {
                return ProductVariantImageUrl(app, productId, productImage, variantId, isSecure, "/small");
            }
            return ProductImageUrlSmall(app, productId, productImage, isSecure);
        }

        private static string MissingImageUrl(HotcakesApplication app, bool isSecure)
        {
            return GetStoreRootUrl(app, isSecure) + MISSING_IMAGE_URL;
        }

        private static string ProductVariantImageUrl(HotcakesApplication app, string productId, string productImage,
            string variantId, bool isSecure, string specialFolder)
        {
            var variantImage = string.Empty;
            var saveLocation = GetStoreDataPhysicalPath(app.CurrentStore.Id);
            saveLocation += "products/" + productId + "/variants/" + variantId;
            if (Directory.Exists(saveLocation))
            {
                var images = Directory.GetFiles(saveLocation);
                if (images != null)
                {
                    if (images.Length > 0)
                    {
                        variantImage = Path.GetFileName(images[0]);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(variantImage))
                return MissingImageUrl(app, isSecure);

            if (productImage.StartsWith("http"))
                return variantImage;

            var url = GetStoreDataUrl(app, isSecure);
            url += "products/" + productId + "/variants/" + variantId + specialFolder + "/" + variantImage;
            return url;
        }

        private static bool VariantImageExists(long storeId, string productId, string variantId)
        {
            var saveLocation = GetStoreDataPhysicalPath(storeId);
            saveLocation += "products/" + productId + "/variants/" + variantId;
            if (Directory.Exists(saveLocation))
            {
                var images = Directory.GetFiles(saveLocation);
                if (images != null)
                {
                    if (images.Length > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <param name="imageName">The imageName used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string CategoryBannerUrl(HotcakesApplication app, string categoryId, string imageName,
            bool isSecure)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                var url = GetStoreDataUrl(app, isSecure);
                url += string.Format("categorybanners/{0}/banner/{1}", categoryId, imageName);
                return url;
            }

            return MissingImageUrl(app, isSecure);
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <param name="imageName">The imageName used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string CategoryBannerOriginalUrl(HotcakesApplication app, string categoryId, string imageName,
            bool isSecure)
        {
            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("categorybanners/{0}/{1}", categoryId, imageName);
            return url;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <param name="imageName">The imageName used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string CategoryIconUrl(HotcakesApplication app, string categoryId, string imageName, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return MissingImageUrl(app, isSecure);

            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("categoryicons/{0}/small/{1}", categoryId, imageName);
            return url;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <param name="imageName">The imageName used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string CategoryIconOriginalUrl(HotcakesApplication app, string categoryId, string imageName,
            bool isSecure)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                var url = GetStoreDataUrl(app, isSecure);
                url += string.Format("categoryicons/{0}/{1}", categoryId, imageName);
                return url;
            }

            return MissingImageUrl(app, isSecure);
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="logorevision">The logorevision used by this operation.</param>
        /// <param name="logoimage">The logoimage used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string StoreLogoOriginalUrl(HotcakesApplication app, long logorevision, string logoimage)
        {
            return StoreLogoOriginalUrl(app, logorevision, logoimage, false);
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="logorevision">The logorevision used by this operation.</param>
        /// <param name="logoimage">The logoimage used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string StoreLogoOriginalUrl(HotcakesApplication app, long logorevision, string logoimage,
            bool isSecure)
        {
            var result = GetStoreDataUrl(app, isSecure);
            result += "storelogo/" + logorevision + logoimage;

            return result;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="logorevision">The logorevision used by this operation.</param>
        /// <param name="imageName">The imageName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string StoreLogoUrl(HotcakesApplication app, long logorevision, string imageName)
        {
            return StoreLogoUrl(app, logorevision, imageName, false);
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="logorevision">The logorevision used by this operation.</param>
        /// <param name="imageName">The imageName used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string StoreLogoUrl(HotcakesApplication app, long logorevision, string imageName, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return GetStoreRootUrl(app, isSecure) + DEFAULT_STORE_LOGO_URL;

            var result = GetStoreDataUrl(app, isSecure);
            result += string.Format("storelogo/{0}/logo/{1}", logorevision, imageName);
            return result;
        }


        /// <summary>
        /// Deletes the targeted store content and returns the implementation-defined success result.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool DeleteProductImages(long storeId, string productId)
        {
            var result = true;

            var sourceFolder = GetStoreDataPhysicalPath(storeId);
            sourceFolder += "products/" + productId + "/";

            if (Directory.Exists(sourceFolder))
            {
                try
                {
                    FileHelper.DeleteDirectoryAndFilesRecursive(sourceFolder);
                }
                catch (Exception ex)
                {
                    result = false;
                    EventLog.LogEvent(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the targeted store content and returns the implementation-defined success result.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="variantId">The variantId used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool DeleteProductVariantImage(long storeId, string productId, string variantId)
        {
            var sourceFolder = GetStoreDataPhysicalPath(storeId);
            sourceFolder += "products/" + productId + "/variants/" + variantId;

            if (Directory.Exists(sourceFolder))
            {
                try
                {
                    FileHelper.DeleteDirectoryAndFilesRecursive(sourceFolder);
                    return true;
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent(ex);
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the targeted store content to the destination store path.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="originalId">The originalId used by this operation.</param>
        /// <param name="newId">The newId used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool CloneAllProductFiles(long storeId, string originalId, string newId)
        {
            var result = true;

            var sourceFolderSubPaths = string.Format("products/{0}/", originalId);
            var sourceFolder = GetStoreDataPhysicalPath(storeId, sourceFolderSubPaths);

            var destFolderSubPath = string.Format("products/{0}/", newId);
            var destFolder = GetStoreDataPhysicalPath(storeId, destFolderSubPath);

            if (Directory.Exists(sourceFolder))
            {
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }
                try
                {
                    FileHelper.CopyAllFiles(sourceFolder, destFolder);
                }
                catch (Exception ex)
                {
                    result = false;
                    EventLog.LogEvent(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Copies the targeted store content to the destination store path.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="imageId">The imageId used by this operation.</param>
        /// <param name="newProductId">The newProductId used by this operation.</param>
        /// <param name="newImageId">The newImageId used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool CloneAdditionalImage(long storeId, string productId, string imageId, string newProductId,
            string newImageId)
        {
            var result = true;

            var sourceFolderSubPath = string.Format("products/{0}/additional/{1}", productId, imageId);
            var sourceFolder = GetStoreDataPhysicalPath(storeId, sourceFolderSubPath);

            var destFolderSubPath = string.Format("products/{0}/additional/{1}", newProductId, newImageId);
            var destFolder = GetStoreDataPhysicalPath(storeId, destFolderSubPath);

            if (Directory.Exists(sourceFolder))
            {
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }
                try
                {
                    FileHelper.CopyAllFiles(sourceFolder, destFolder);
                }
                catch (Exception ex)
                {
                    result = false;
                    EventLog.LogEvent(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the targeted store content and returns the implementation-defined success result.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool DeleteCategoryImages(long storeId, long categoryId)
        {
            var result = true;

            var subPath = string.Format("categories/{0}/", categoryId);
            var sourceFolder = GetStoreDataPhysicalPath(storeId, subPath);

            if (Directory.Exists(sourceFolder))
            {
                foreach (var f in Directory.GetFiles(sourceFolder))
                {
                    try
                    {
                        File.SetAttributes(f, FileAttributes.Normal);
                        File.Delete(f);
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        EventLog.LogEvent(ex);
                    }
                }
                Directory.Delete(sourceFolder);
            }

            return result;
        }

        /// <summary>
        /// Removes all targeted store content.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool DestroyAllFilesForStore(long storeId)
        {
            var result = true;

            var sourceFolder = GetStoreDataPhysicalPath(storeId);

            if (Directory.Exists(sourceFolder))
            {
                try
                {
                    FileHelper.DeleteDirectoryAndFilesRecursive(sourceFolder);
                }
                catch (Exception ex)
                {
                    result = false;
                    EventLog.LogEvent(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string StoreUrl(HotcakesApplication app, string fileName, bool isSecure)
        {
            return GetStoreDataUrl(app, isSecure) + fileName;
        }

        private static bool WriteTextToFile(string saveLocation, string data)
        {
            var result = true;

            try
            {
                if (Directory.Exists(Path.GetDirectoryName(saveLocation)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(saveLocation));
                }
                File.WriteAllText(saveLocation, data);
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Reads the requested store-scoped content.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string ReadUserFile(long storeId, string fileName)
        {
            var result = string.Empty;
            var root = GetStoreDataPhysicalPath(storeId);
            var location = root + fileName;
            if (File.Exists(location))
            {
                result = File.ReadAllText(location);
            }
            return result;
        }

        /// <summary>
        /// Writes the requested store-scoped content.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <param name="data">The data used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool WriteUserFile(long storeId, string fileName, string data)
        {
            var root = GetStoreDataPhysicalPath(storeId);
            var destination = root + fileName;
            return WriteTextToFile(destination, data);
        }

        #region Get Urls

        /// <summary>
        /// Resolves the requested Hotcakes, store, virtual, or physical path.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string GetHccVirtualPath(string subpath = "")
        {
            return "~/DesktopModules/Hotcakes/" + subpath;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="context">The context used by this operation.</param>
        /// <param name="subpath">The optional path appended to the administration URL.</param>
        /// <param name="isSecure">Whether the store-root URL should use its secure form.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string GetHccAdminUrl(HccRequestContext context, string subpath = "", bool isSecure = false)
        {
            return GetStoreRootUrl(context, isSecure) + "DesktopModules/Hotcakes/Core/Admin/" + subpath;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="context">The context used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string GetStoreRootUrl(HccRequestContext context, bool isSecure)
        {
            var urlResolver = Factory.CreateHccUrlResolver();
            return urlResolver.GetStoreRootUrl(context.CurrentStore, isSecure);
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The application whose current request context and store are used.</param>
        /// <param name="subpath">The optional path appended to the administration URL.</param>
        /// <param name="isSecure">Whether the store-root URL should use its secure form.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string GetHccAdminUrl(HotcakesApplication app, string subpath = "", bool isSecure = false)
        {
            return GetHccAdminUrl(app.CurrentRequestContext, subpath, isSecure);
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string GetStoreRootUrl(HotcakesApplication app, bool isSecure)
        {
            return GetStoreRootUrl(app.CurrentRequestContext, isSecure);
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="app">The app used by this operation.</param>
        /// <param name="isSecure">The isSecure used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string GetStoreDataUrl(HotcakesApplication app, bool isSecure)
        {
            var rootUrl = GetStoreRootUrl(app, isSecure);
            var virtualPath = GetStoreDataVirtualPath(app.CurrentStore.Id);
            return rootUrl + virtualPath.TrimStart('~', '/');
        }

        /// <summary>
        /// Resolves the requested Hotcakes, store, virtual, or physical path.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The store identifier whose data path is resolved.</param>
        /// <param name="subpath">The optional path appended to the store data path.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string GetStoreDataVirtualPath(long storeId, string subpath = "")
        {
            var urlResolver = Factory.CreateHccUrlResolver();
            var virtualPath = urlResolver.GetStoreDataVirtualPath(storeId);
            return virtualPath + subpath;
        }

        /// <summary>
        /// Resolves the requested Hotcakes, store, virtual, or physical path.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The store identifier whose physical data path is resolved.</param>
        /// <param name="subpath">The optional path appended before mapping the virtual path.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string GetStoreDataPhysicalPath(long storeId, string subpath = "")
        {
            var virtualPath = GetStoreDataVirtualPath(storeId);
            return HostingEnvironment.MapPath(virtualPath + subpath);
        }

        #endregion

        #region Upload Files

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="pFile">The pFile used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string UploadPaymanentsAttachment(long storeId, HttpPostedFile pFile)
        {
            if (pFile != null)
            {
                var saveLocation = GetStoreDataPhysicalPath(storeId) + "affiliates\\";
                var fileName = Guid.NewGuid() + "\\" + Text.CleanFileName(Path.GetFileName(pFile.FileName));

                saveLocation += fileName;

                if (WriteFileToPath(saveLocation, pFile))
                {
                    return fileName;
                }
            }
            return null;
        }

        /// <summary>
        /// Builds a store-scoped URL, applying the documented image fallback and variant rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="filename">The filename used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string PaymentsAttachmentUrl(long storeId, string filename)
        {
            return GetStoreDataVirtualPath(storeId) + "affiliates/" + filename;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="pFile">The pFile used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string UploadTempImage(long storeId, HttpPostedFile pFile)
        {
            if (ValidateImageType(Path.GetExtension(pFile.FileName)))
            {
                return UploadPostedFile(storeId, pFile, "temp\\");
            }

            return null;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="pFile">The pFile used by this operation.</param>
        /// <param name="extention">The extention used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static string UploadTempFile(long storeId, HttpPostedFile pFile, string extention)
        {
            if (Path.GetExtension(pFile.FileName).ToUpper() == extention)
            {
                return UploadPostedFile(storeId, pFile, "temp\\");
            }

            return null;
        }

        /// <summary>
        /// Copies content into store storage and applies the existing image processing lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="tempFilePath">The tempFilePath used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool CopyProductImage(long storeId, string productId, string tempFilePath, string fileName)
        {
            var result = false;

            if (ValidateImageType(Path.GetExtension(fileName)))
            {
                var path = string.Format("products/{0}/{1}", productId, fileName);
                var saveLocation = GetStoreDataPhysicalPath(storeId, path);

                tempFilePath = HostingEnvironment.MapPath(tempFilePath);
                result = CopyFileToPath(tempFilePath, saveLocation, true);

                if (result)
                {
                    result = ImageProcessing.ShrinkToSmall(saveLocation);
                    if (result)
                    {
                        result = ImageProcessing.ShrinkToMedium(saveLocation);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="file">The file used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadProductImage(long storeId, string productId, HttpPostedFile file)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(file.FileName)))
            {
                return false;
            }

            var saveLocation = GetStoreDataPhysicalPath(storeId);
            saveLocation += "products/" + productId + "/";
            saveLocation += Text.CleanFileName(Path.GetFileName(file.FileName));

            // Delete the old one if it exists
            if (File.Exists(saveLocation))
            {
                File.SetAttributes(saveLocation, FileAttributes.Normal);
                File.Delete(saveLocation);
            }
            result = WriteFileToPath(saveLocation, file);

            if (result)
            {
                result = ImageProcessing.ShrinkToSmall(saveLocation);
                if (result)
                {
                    result = ImageProcessing.ShrinkToMedium(saveLocation);
                }
            }
            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <param name="data">The data used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadProductImage(long storeId, string productId, string fileName, byte[] data)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(fileName)))
            {
                return false;
            }

            var saveLocation = GetStoreDataPhysicalPath(storeId);
            saveLocation += "products\\" + productId + "\\";
            saveLocation += Text.CleanFileName(Path.GetFileName(fileName));

            // Delete the old one if it exists
            if (File.Exists(saveLocation))
            {
                File.SetAttributes(saveLocation, FileAttributes.Normal);
                File.Delete(saveLocation);
            }
            result = WriteBytesToPath(saveLocation, data);

            if (result)
            {
                result = ImageProcessing.ShrinkToSmall(saveLocation);
                if (result)
                {
                    result = ImageProcessing.ShrinkToMedium(saveLocation);
                }
            }
            return result;
        }

        /// <summary>
        /// Copies content into store storage and applies the existing image processing lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="variantId">The variantId used by this operation.</param>
        /// <param name="tempFilePath">The tempFilePath used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool CopyProductVariantImage(long storeId, string productId, string variantId, string tempFilePath,
            string fileName)
        {
            var result = false;

            if (ValidateImageType(Path.GetExtension(fileName)))
            {
                var path = string.Format("products/{0}/variants/{1}/{2}", productId, variantId, fileName);
                var saveLocation = GetStoreDataPhysicalPath(storeId, path);

                tempFilePath = HostingEnvironment.MapPath(tempFilePath);
                var destinationDir = Path.GetDirectoryName(saveLocation);
                if (Directory.Exists(destinationDir))
                {
                    Directory.Delete(destinationDir, true);
                }
                result = CopyFileToPath(tempFilePath, saveLocation, true);

                if (result)
                {
                    result = ImageProcessing.ShrinkToSmall(saveLocation);
                    if (result)
                    {
                        result = ImageProcessing.ShrinkToMedium(saveLocation);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="productId">The productId used by this operation.</param>
        /// <param name="variantId">The variantId used by this operation.</param>
        /// <param name="file">The file used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadProductVariantImage(long storeId, string productId, string variantId,
            HttpPostedFile file)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(file.FileName)))
            {
                return false;
            }

            var saveLocation = GetStoreDataPhysicalPath(storeId);
            saveLocation += "products/" + productId + "/variants/" + variantId;

            // Clear out old images before posting new one
            if (Directory.Exists(saveLocation))
            {
                FileHelper.DeleteDirectoryAndFilesRecursive(saveLocation);
            }

            saveLocation += "/" + variantId;
            saveLocation += Path.GetExtension(file.FileName);
            result = WriteFileToPath(saveLocation, file);

            if (result)
            {
                result = ImageProcessing.ShrinkToSmall(saveLocation);
                if (result)
                {
                    result = ImageProcessing.ShrinkToMedium(saveLocation);
                }
            }
            return result;
        }

        // Category Banners and Icons
        /// <summary>
        /// Copies content into store storage and applies the existing image processing lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <param name="tempFilePath">The tempFilePath used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool CopyCategoryBanner(long storeId, string categoryId, string tempFilePath, string fileName)
        {
            var result = false;

            if (ValidateImageType(Path.GetExtension(fileName)))
            {
                var path = string.Format("categorybanners/{0}/{1}", categoryId,
                    Text.CleanFileName(Path.GetFileName(fileName)));
                var saveLocation = GetStoreDataPhysicalPath(storeId, path);

                tempFilePath = HostingEnvironment.MapPath(tempFilePath);
                result = CopyFileToPath(tempFilePath, saveLocation, true);

                if (result)
                {
                    result = ImageProcessing.ShrinkToBanner(saveLocation);
                }
            }

            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <param name="file">The file used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadCategoryBanner(long storeId, string categoryId, HttpPostedFile file)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(file.FileName)))
            {
                return false;
            }
            var saveLocation = GetStoreDataPhysicalPath(storeId);
            saveLocation += "categorybanners/" + categoryId + "/";
            saveLocation += Text.CleanFileName(Path.GetFileName(file.FileName));

            // Delete the old one if it exists
            if (File.Exists(saveLocation))
            {
                File.SetAttributes(saveLocation, FileAttributes.Normal);
                File.Delete(saveLocation);
            }
            result = WriteFileToPath(saveLocation, file);

            if (result)
            {
                result = ImageProcessing.ShrinkToBanner(saveLocation);
            }

            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <param name="data">The data used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadCategoryBanner(long storeId, string categoryId, string fileName, byte[] data)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(fileName)))
            {
                return false;
            }
            var saveLocation = GetStoreDataPhysicalPath(storeId);
            saveLocation += "categorybanners/" + categoryId + "/";
            saveLocation += Text.CleanFileName(Path.GetFileName(fileName));

            // Delete the old one if it exists
            if (File.Exists(saveLocation))
            {
                File.SetAttributes(saveLocation, FileAttributes.Normal);
                File.Delete(saveLocation);
            }
            result = WriteBytesToPath(saveLocation, data);

            if (result)
            {
                result = ImageProcessing.ShrinkToBanner(saveLocation);
            }

            return result;
        }

        /// <summary>
        /// Copies content into store storage and applies the existing image processing lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <param name="tempFilePath">The tempFilePath used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool CopyCategoryIcon(long storeId, string categoryId, string tempFilePath, string fileName)
        {
            var result = false;

            if (ValidateImageType(Path.GetExtension(fileName)))
            {
                var path = string.Format("categoryicons/{0}/{1}", categoryId,
                    Text.CleanFileName(Path.GetFileName(fileName)));
                var saveLocation = GetStoreDataPhysicalPath(storeId, path);

                tempFilePath = HostingEnvironment.MapPath(tempFilePath);
                result = CopyFileToPath(tempFilePath, saveLocation, true);

                if (result)
                {
                    result = ImageProcessing.ShrinkToSmall(saveLocation);
                }
            }

            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <param name="file">The file used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadCategoryIcon(long storeId, string categoryId, HttpPostedFile file)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(file.FileName)))
            {
                return false;
            }

            var saveLocation = GetStoreDataPhysicalPath(storeId);
            saveLocation += "categoryicons/" + categoryId + "/";
            saveLocation += Text.CleanFileName(Path.GetFileName(file.FileName));

            // Delete the old one if it exists
            if (File.Exists(saveLocation))
            {
                File.SetAttributes(saveLocation, FileAttributes.Normal);
                File.Delete(saveLocation);
            }
            result = WriteFileToPath(saveLocation, file);

            if (result)
            {
                result = ImageProcessing.ShrinkToSmall(saveLocation);
            }

            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="categoryId">The categoryId used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <param name="data">The data used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadCategoryIcon(long storeId, string categoryId, string fileName, byte[] data)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(fileName)))
            {
                return false;
            }

            var saveLocation = GetStoreDataPhysicalPath(storeId);
            saveLocation += "categoryicons/" + categoryId + "/";
            saveLocation += Text.CleanFileName(Path.GetFileName(fileName));

            // Delete the old one if it exists
            if (File.Exists(saveLocation))
            {
                File.SetAttributes(saveLocation, FileAttributes.Normal);
                File.Delete(saveLocation);
            }
            result = WriteBytesToPath(saveLocation, data);

            if (result)
            {
                result = ImageProcessing.ShrinkToSmall(saveLocation);
            }

            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="store">The store used by this operation.</param>
        /// <param name="file">The file used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadStoreImage(Store store, HttpPostedFile file)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(file.FileName)))
            {
                return false;
            }

            if (store != null)
            {
                var newRevision = store.Settings.LogoRevision + 1;

                var saveLocation = GetStoreDataPhysicalPath(store.Id);
                saveLocation += "storelogo/" + newRevision + "/";
                saveLocation += Text.CleanFileName(Path.GetFileName(file.FileName));

                result = WriteFileToPath(saveLocation, file);

                if (result)
                {
                    store.Settings.LogoRevision = newRevision;
                    result = ImageProcessing.ShrinkToLogo(saveLocation);
                }
            }
            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="store">The store used by this operation.</param>
        /// <param name="tempFilePath">The tempFilePath used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool UploadStoreImage(Store store, string tempFilePath, string fileName)
        {
            var result = true;

            if (!ValidateImageType(Path.GetExtension(fileName)))
            {
                return false;
            }

            if (store != null)
            {
                var newRevision = store.Settings.LogoRevision + 1;

                var saveLocation = GetStoreDataPhysicalPath(store.Id);
                saveLocation += "storelogo/" + newRevision + "/";
                saveLocation += Text.CleanFileName(Path.GetFileName(fileName));

                tempFilePath = HostingEnvironment.MapPath(tempFilePath);
                result = CopyFileToPath(tempFilePath, saveLocation, true);

                if (result)
                {
                    store.Settings.LogoRevision = newRevision;
                    result = ImageProcessing.ShrinkToLogo(saveLocation);
                }
            }
            return result;
        }

        private static string UploadPostedFile(long storeId, HttpPostedFile pFile, string folder)
        {
            var tempLocation = GetStoreDataPhysicalPath(storeId) + folder;
            var tempFileName = tempLocation + Guid.NewGuid() + Path.GetExtension(pFile.FileName);

            // Ensure directory
            if (!Directory.Exists(tempLocation))
            {
                Directory.CreateDirectory(tempLocation);
            }

            // Delete outdated files
            var files = Directory.GetFiles(tempLocation);
            foreach (var file in files)
            {
                var now = DateTime.Now;
                if (File.GetLastWriteTime(file).AddHours(24) < now)
                {
                    File.Delete(file);
                }
            }

            // Save posted file to destination
            if (WriteFileToPath(tempFileName, pFile))
            {
                return tempFileName;
            }

            return null;
        }

        #endregion

        #region Implementation

        private static bool CopyFileToPath(string sourceFilePath, string saveLocation, bool deleteTarget)
        {
            var result = false;

            try
            {
                if (deleteTarget && File.Exists(saveLocation))
                {
                    File.SetAttributes(saveLocation, FileAttributes.Normal);
                    File.Delete(saveLocation);
                }

                if (sourceFilePath != null)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(saveLocation)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(saveLocation));
                    }

                    if (File.Exists(sourceFilePath))
                    {
                        File.Copy(sourceFilePath, saveLocation);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }

            return result;
        }

        private static bool WriteFileToPath(string saveLocation, HttpPostedFile file)
        {
            var result = true;

            try
            {
                if (file != null)
                {
                    if (Directory.Exists(Path.GetDirectoryName(saveLocation)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(saveLocation));
                    }
                    file.SaveAs(saveLocation);
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = false;
            }

            return result;
        }

        private static bool WriteFileToPath(string saveLocation, HttpPostedFileBase file)
        {
            var result = true;

            try
            {
                if (file != null)
                {
                    if (Directory.Exists(Path.GetDirectoryName(saveLocation)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(saveLocation));
                    }
                    file.SaveAs(saveLocation);
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = false;
            }

            return result;
        }

        private static bool WriteBytesToPath(string saveLocation, byte[] data)
        {
            var result = true;

            try
            {
                if (data != null)
                {
                    if (Directory.Exists(Path.GetDirectoryName(saveLocation)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(saveLocation));
                    }
                    File.WriteAllBytes(saveLocation, data);
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = false;
            }

            return result;
        }

        private static bool WriteFileToPath(string saveLocation, FileStream stream)
        {
            var result = true;

            try
            {
                if (stream != null)
                {
                    if (Directory.Exists(Path.GetDirectoryName(saveLocation)) == false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(saveLocation));
                    }

                    // Create a FileStream object to write a stream to a file
                    using (var fileStream = File.Create(saveLocation, (int) stream.Length))
                    {
                        // Fill the bytes[] array with the stream data
                        var bytesInStream = new byte[stream.Length];
                        stream.Read(bytesInStream, 0, bytesInStream.Length);

                        // Use FileStream object to write to the specified file
                        fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = false;
            }

            return result;
        }

        #endregion

        #region Public methods / File Vault

        /// <summary>
        /// Performs the requested file-vault operation using a cleaned base file name.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        protected static string FileVaultPhysicalPath(long storeId)
        {
            return GetStoreDataPhysicalPath(storeId) + "filevault\\";
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="diskFileName">The diskFileName used by this operation.</param>
        /// <param name="file">The file used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileVaultUpload(long storeId, string diskFileName, HttpPostedFile file)
        {
            var result = true;

            // Ensure the directory exists
            if (!Directory.Exists(FileVaultPhysicalPath(storeId)))
            {
                Directory.CreateDirectory(FileVaultPhysicalPath(storeId));
            }

            var saveLocation = FileVaultPhysicalPath(storeId);
            saveLocation += Text.CleanFileName(Path.GetFileName(diskFileName));
            result = WriteFileToPath(saveLocation, file);

            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="diskFileName">The diskFileName used by this operation.</param>
        /// <param name="stream">The stream used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileVaultUpload(long storeId, string diskFileName, FileStream stream)
        {
            var result = true;

            // Ensure the directory exists
            if (!Directory.Exists(FileVaultPhysicalPath(storeId)))
            {
                Directory.CreateDirectory(FileVaultPhysicalPath(storeId));
            }

            var saveLocation = FileVaultPhysicalPath(storeId);
            saveLocation += Text.CleanFileName(Path.GetFileName(diskFileName));
            result = WriteFileToPath(saveLocation, stream);

            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="diskFileName">The diskFileName used by this operation.</param>
        /// <param name="data">The data used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileVaultUpload(long storeId, string diskFileName, byte[] data)
        {
            var result = true;

            // Ensure the directory exists
            if (!Directory.Exists(FileVaultPhysicalPath(storeId)))
            {
                Directory.CreateDirectory(FileVaultPhysicalPath(storeId));
            }

            var saveLocation = FileVaultPhysicalPath(storeId);
            saveLocation += Text.CleanFileName(Path.GetFileName(diskFileName));
            result = WriteBytesToPath(saveLocation, data);

            return result;
        }

        /// <summary>
        /// Validates and writes uploaded content, preserving the existing replacement and image-variant lifecycle.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="diskFileName">The diskFileName used by this operation.</param>
        /// <param name="data">The data used by this operation.</param>
        /// <param name="isFirstPart">The isFirstPart used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileVaultUploadPartial(long storeId, string diskFileName, byte[] data, bool isFirstPart)
        {
            var result = false;

            // Ensure the directory exists
            if (!Directory.Exists(FileVaultPhysicalPath(storeId)))
            {
                Directory.CreateDirectory(FileVaultPhysicalPath(storeId));
            }

            var saveLocation = FileVaultPhysicalPath(storeId);
            saveLocation += Text.CleanFileName(Path.GetFileName(diskFileName));

            if (isFirstPart)
            {
                // Delete the old one if it exists and this is the first part upload
                if (File.Exists(saveLocation))
                {
                    File.SetAttributes(saveLocation, FileAttributes.Normal);
                    File.Delete(saveLocation);
                }

                // Create New
                var fs = File.Create(saveLocation);
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();
            }
            else
            {
                // Append
                using (var writeStream = File.Open(saveLocation, FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    writeStream.Write(data, 0, data.Length);
                }
            }

            result = true;
            return result;
        }

        /// <summary>
        /// Performs the requested file-vault operation using a cleaned base file name.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileVaultRemove(long storeId, string fileName)
        {
            var f = FileVaultPhysicalPath(storeId) + Text.CleanFileName(fileName);
            if (File.Exists(f))
            {
                File.SetAttributes(f, FileAttributes.Normal);
                File.Delete(f);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Performs the requested file-vault operation using a cleaned base file name.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileVaultFileExists(long storeId, string fileName)
        {
            if (File.Exists(FileVaultPhysicalPath(storeId) + Text.CleanFileName(fileName)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Performs the requested file-vault operation using a cleaned base file name.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static byte[] FileVaultGetBytes(long storeId, string fileName)
        {
            byte[] bytes = null;

            if (FileVaultFileExists(storeId, fileName))
            {
                bytes = File.ReadAllBytes(FileVaultPhysicalPath(storeId) + Text.CleanFileName(fileName));
            }

            return bytes;
        }

        /// <summary>
        /// Performs the requested file-vault operation using a cleaned base file name.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="fileName">The fileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static FileStream FileVaultGetStream(long storeId, string fileName)
        {
            FileStream stream = null;

            if (FileVaultFileExists(storeId, fileName))
            {
                stream = File.Open(FileVaultPhysicalPath(storeId) + Text.CleanFileName(fileName), FileMode.Open,
                    FileAccess.Read, FileShare.Read);
            }

            return stream;
        }

        #endregion

        #region Public methods / File Manager

        /// <summary>
        /// Cleans a file-manager path using the implementation's separator, character, parent-marker, and duplicate-separator rules.
        /// </summary>
        /// <remarks>
        /// This is string cleaning rather than full canonical-path validation: it removes selected characters and occurrences of <c>..</c>, trims leading separators, and stops after the built-in duplicate-separator fail-safe. It does not establish filesystem authorization by itself.
        /// </remarks>
        /// <param name="rawPath">The raw file-manager path to clean.</param>
        /// <returns>The cleaned path, or an empty string when the duplicate-separator fail-safe is exceeded.</returns>
        public static string FileManagerCleanPath(string rawPath)
        {
            var correctPath = rawPath.Replace('/', '\\');
            correctPath = correctPath.TrimStart('\\');
            correctPath = correctPath.Replace("..", string.Empty); // Don't allow escape to above
            correctPath = correctPath.Replace("\"", string.Empty);
            correctPath = correctPath.Replace(":", string.Empty);
            correctPath = correctPath.Replace("*", string.Empty);
            correctPath = correctPath.Replace("?", string.Empty);
            correctPath = correctPath.Replace("<", string.Empty);
            correctPath = correctPath.Replace(">", string.Empty);
            correctPath = correctPath.Replace("|", string.Empty);
            correctPath = correctPath.Replace("$", string.Empty);
            correctPath = correctPath.Replace("%", string.Empty);

            // replace double slashes with singles
            var failCounter = 0;
            while (correctPath.Contains("\\\\"))
            {
                failCounter++;
                correctPath = correctPath.Replace("\\\\", "\\");

                // Fail safe in case we have someone trying gum up the works 
                // with a really poorly formed path
                if (failCounter > 100)
                {
                    correctPath = string.Empty;
                    break;
                }
            }
            return correctPath;
        }

        private static string FullPhysicalPath(long storeId, string path)
        {
            var correctPath = FileManagerCleanPath(path);
            var fullPath = GetStoreDataPhysicalPath(storeId) + correctPath;
            return fullPath;
        }

        /// <summary>
        /// Performs the requested file-manager operation after the implementation's path cleaning rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="path">The path used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileManagerIsSystemPath(string path)
        {
            var p = FileManagerCleanPath(path);
            p = p.Trim().ToLowerInvariant();

            if (p == "filevault") return true;
            if (p == "products") return true;
            if (p == "storelogo") return true;
            if (p == "pages") return true;

            return false;
        }

        /// <summary>
        /// Performs the requested file-manager operation after the implementation's path cleaning rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="path">The path used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static List<string> FileManagerListDirectories(long storeId, string path)
        {
            var result = new List<string>();

            var fullPath = FullPhysicalPath(storeId, path);
            if (!Directory.Exists(fullPath))
            {
                return result;
            }
            var allDirs = Directory.GetDirectories(fullPath);
            if (allDirs != null)
            {
                foreach (var d in allDirs)
                {
                    result.Add(Path.GetFileName(d));
                }
            }

            return result;
        }

        /// <summary>
        /// Performs the requested file-manager operation after the implementation's path cleaning rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="path">The path used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static List<string> FileManagerListFiles(long storeId, string path)
        {
            var result = new List<string>();

            var fullPath = FullPhysicalPath(storeId, path);
            if (!Directory.Exists(fullPath))
            {
                return result;
            }
            var allFiles = Directory.GetFiles(fullPath);
            if (allFiles != null)
            {
                foreach (var f in allFiles)
                {
                    result.Add(Path.GetFileName(f));
                }
            }

            return result;
        }

        /// <summary>
        /// Performs the requested file-manager operation after the implementation's path cleaning rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="path">The path used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileManagerCreateDirectory(long storeId, string path)
        {
            var result = false;

            var fullPath = FullPhysicalPath(storeId, path);
            if (!Directory.Exists(fullPath))
            {
                try
                {
                    var info = Directory.CreateDirectory(fullPath);
                    if (info != null)
                    {
                        result = info.Exists;
                    }
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the targeted store content and returns the implementation-defined success result.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="path">The path used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileManagerDeleteDirectory(long storeId, string path)
        {
            var result = true;

            var fullPath = FullPhysicalPath(storeId, path);

            if (Directory.Exists(fullPath))
            {
                try
                {
                    FileHelper.DeleteDirectoryAndFilesRecursive(fullPath);
                }
                catch (Exception ex)
                {
                    result = false;
                    EventLog.LogEvent(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Performs the requested file-manager operation after the implementation's path cleaning rules.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="pathAndFileName">The pathAndFileName used by this operation.</param>
        /// <param name="file">The file used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileManagerCreateFile(long storeId, string pathAndFileName, HttpPostedFileBase file)
        {
            var fullPath = FullPhysicalPath(storeId, pathAndFileName);
            return WriteFileToPath(fullPath, file);
        }

        /// <summary>
        /// Deletes the targeted store content and returns the implementation-defined success result.
        /// </summary>
        /// <remarks>
        /// Store URLs, virtual paths, and physical paths use the existing resolver and hosting conventions. Extension checks, cleaned names, raw HTML values, omitted fields, exception behavior, stream ownership, and return values are unchanged by this documentation.
        /// </remarks>
        /// <param name="storeId">The storeId used by this operation.</param>
        /// <param name="pathAndFileName">The pathAndFileName used by this operation.</param>
        /// <returns>The resolved value or operation result; missing files and failures retain the implementation-defined behavior.</returns>
        public static bool FileManagerDeleteFile(long storeId, string pathAndFileName)
        {
            var fullPath = FullPhysicalPath(storeId, pathAndFileName);
            if (File.Exists(fullPath))
            {
                try
                {
                    File.SetAttributes(fullPath, FileAttributes.Normal);
                    File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent(ex);
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
