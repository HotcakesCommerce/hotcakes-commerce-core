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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Storage
{
    [Serializable]
    public class DiskStorage
    {
        private const string MISSING_IMAGE_URL = "DesktopModules/Hotcakes/Core/Admin/Images/MissingImage.png";
        private const string DEFAULT_STORE_LOGO_URL = "DesktopModules/Hotcakes/Core/Admin/Images/DefaultStoreLogo.png";

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

        public static bool CopyDemoProductImage(long storeId, string productId, string imageName)
        {
            try
            {
                var demoImagesPath = ApplicationBuiltinDemoImagesPath;
                var saveLocation = GetStoreDataPhysicalPath(storeId);
                saveLocation += "products\\" + productId + "\\";

                FileHelper.CopySingle(Path.Combine(demoImagesPath, "small"),
                    Path.Combine(saveLocation, "small"), imageName, true);
                FileHelper.CopySingle(Path.Combine(demoImagesPath, "medium"),
                    Path.Combine(saveLocation, "medium"), imageName, true);

                FileHelper.CopySingle(Path.Combine(demoImagesPath, "medium"),
                    saveLocation, imageName, true);
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                return false;
            }
            return true;
        }

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

        public static string ProductAdditionalImageUrlOriginal(HotcakesApplication app, string productId, string imageId,
            string productImage, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(productImage))
                return MissingImageUrl(app, isSecure);

            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("products/{0}/additional/{1}/{2}", productId, imageId, productImage);
            return url;
        }

        public static string ProductAdditionalImageUrlMedium(HotcakesApplication app, string productId, string imageId,
            string productImage, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(productImage))
                return MissingImageUrl(app, isSecure);

            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("products/{0}/additional/{1}/medium/{2}", productId, imageId, productImage);
            return url;
        }

        public static string ProductAdditionalImageUrlSmall(HotcakesApplication app, string productId, string imageId,
            string productImage, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(productImage))
                return MissingImageUrl(app, isSecure);

            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("products/{0}/additional/{1}/small/{2}", productId, imageId, productImage);
            return url;
        }

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

        public static string CategoryBannerOriginalUrl(HotcakesApplication app, string categoryId, string imageName,
            bool isSecure)
        {
            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("categorybanners/{0}/{1}", categoryId, imageName);
            return url;
        }

        public static string CategoryIconUrl(HotcakesApplication app, string categoryId, string imageName, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return MissingImageUrl(app, isSecure);

            var url = GetStoreDataUrl(app, isSecure);
            url += string.Format("categoryicons/{0}/small/{1}", categoryId, imageName);
            return url;
        }

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

        public static string StoreLogoOriginalUrl(HotcakesApplication app, long logorevision, string logoimage)
        {
            return StoreLogoOriginalUrl(app, logorevision, logoimage, false);
        }

        public static string StoreLogoOriginalUrl(HotcakesApplication app, long logorevision, string logoimage,
            bool isSecure)
        {
            var result = GetStoreDataUrl(app, isSecure);
            result += "storelogo/" + logorevision + logoimage;

            return result;
        }

        public static string StoreLogoUrl(HotcakesApplication app, long logorevision, string imageName)
        {
            return StoreLogoUrl(app, logorevision, imageName, false);
        }

        public static string StoreLogoUrl(HotcakesApplication app, long logorevision, string imageName, bool isSecure)
        {
            if (string.IsNullOrWhiteSpace(imageName))
                return GetStoreRootUrl(app, isSecure) + DEFAULT_STORE_LOGO_URL;

            var result = GetStoreDataUrl(app, isSecure);
            result += string.Format("storelogo/{0}/logo/{1}", logorevision, imageName);
            return result;
        }


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

        public static bool WriteUserFile(long storeId, string fileName, string data)
        {
            var root = GetStoreDataPhysicalPath(storeId);
            var destination = root + fileName;
            return WriteTextToFile(destination, data);
        }

        #region Get Urls

        public static string GetHccVirtualPath(string subpath = "")
        {
            return "~/DesktopModules/Hotcakes/" + subpath;
        }

        public static string GetHccAdminUrl(HccRequestContext context, string subpath = "", bool isSecure = false)
        {
            return GetStoreRootUrl(context, isSecure) + "DesktopModules/Hotcakes/Core/Admin/" + subpath;
        }

        public static string GetStoreRootUrl(HccRequestContext context, bool isSecure)
        {
            var urlResolver = Factory.CreateHccUrlResolver();
            return urlResolver.GetStoreRootUrl(context.CurrentStore, isSecure);
        }

        public static string GetHccAdminUrl(HotcakesApplication app, string subpath = "", bool isSecure = false)
        {
            return GetHccAdminUrl(app.CurrentRequestContext, subpath, isSecure);
        }

        public static string GetStoreRootUrl(HotcakesApplication app, bool isSecure)
        {
            return GetStoreRootUrl(app.CurrentRequestContext, isSecure);
        }

        public static string GetStoreDataUrl(HotcakesApplication app, bool isSecure)
        {
            var rootUrl = GetStoreRootUrl(app, isSecure);
            var virtualPath = GetStoreDataVirtualPath(app.CurrentStore.Id);
            return rootUrl + virtualPath.TrimStart('~', '/');
        }

        public static string GetStoreDataVirtualPath(long storeId, string subpath = "")
        {
            var urlResolver = Factory.CreateHccUrlResolver();
            var virtualPath = urlResolver.GetStoreDataVirtualPath(storeId);
            return virtualPath + subpath;
        }

        public static string GetStoreDataPhysicalPath(long storeId, string subpath = "")
        {
            var virtualPath = GetStoreDataVirtualPath(storeId);
            return HostingEnvironment.MapPath(virtualPath + subpath);
        }

        #endregion

        #region Upload Files

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

        public static string PaymentsAttachmentUrl(long storeId, string filename)
        {
            return GetStoreDataVirtualPath(storeId) + "affiliates/" + filename;
        }

        public static string UploadTempImage(long storeId, HttpPostedFile pFile)
        {
            if (ValidateImageType(Path.GetExtension(pFile.FileName)))
            {
                return UploadPostedFile(storeId, pFile, "temp\\");
            }

            return null;
        }

        public static string UploadTempFile(long storeId, HttpPostedFile pFile, string extention)
        {
            if (Path.GetExtension(pFile.FileName).ToUpper() == extention)
            {
                return UploadPostedFile(storeId, pFile, "temp\\");
            }

            return null;
        }

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

        protected static string FileVaultPhysicalPath(long storeId)
        {
            return GetStoreDataPhysicalPath(storeId) + "filevault\\";
        }

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

        public static bool FileVaultFileExists(long storeId, string fileName)
        {
            if (File.Exists(FileVaultPhysicalPath(storeId) + Text.CleanFileName(fileName)))
            {
                return true;
            }
            return false;
        }

        public static byte[] FileVaultGetBytes(long storeId, string fileName)
        {
            byte[] bytes = null;

            if (FileVaultFileExists(storeId, fileName))
            {
                bytes = File.ReadAllBytes(FileVaultPhysicalPath(storeId) + Text.CleanFileName(fileName));
            }

            return bytes;
        }

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

        public static bool FileManagerCreateFile(long storeId, string pathAndFileName, HttpPostedFileBase file)
        {
            var fullPath = FullPhysicalPath(storeId, pathAndFileName);
            return WriteFileToPath(fullPath, file);
        }

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