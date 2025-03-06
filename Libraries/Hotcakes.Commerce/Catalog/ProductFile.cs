#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Storage;
using Hotcakes.CommerceDTO.v1.Catalog;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Product file information
    /// </summary>
    [Serializable]
    public class ProductFile
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public ProductFile()
        {
            StoreId = 0;
            ProductId = string.Empty;
            AvailableMinutes = 0;
            MaxDownloads = 0;
            FileName = string.Empty;
            ShortDescription = string.Empty;
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
        }

        public string Bvin { get; set; }
        public DateTime LastUpdated { get; set; }
        public long StoreId { get; set; }
        public string ProductId { get; set; }
        public int AvailableMinutes { get; set; }
        public int MaxDownloads { get; set; }
        public string FileName { get; set; }
        public string ShortDescription { get; set; }

        /// <summary>
        ///     Combine description and file name
        /// </summary>
        public string CombinedDisplay
        {
            get { return ShortDescription + " [" + FileName + "]"; }
        }

        /// <summary>
        ///     Create DTO object for API
        /// </summary>
        /// <returns></returns>
        public ProductFileDTO ToDto()
        {
            var dto = new ProductFileDTO();

            dto.AvailableMinutes = AvailableMinutes;
            dto.Bvin = Bvin;
            dto.FileName = FileName;
            dto.LastUpdated = LastUpdated;
            dto.MaxDownloads = MaxDownloads;
            dto.ProductId = ProductId;
            dto.ShortDescription = ShortDescription;
            dto.StoreId = StoreId;

            return dto;
        }

        /// <summary>
        ///     Set values of the product file based on the DTO instance
        /// </summary>
        /// <param name="dto"><see cref="ProductFileDTO" /> instance</param>
        public void FromDto(ProductFileDTO dto)
        {
            if (dto == null) return;

            AvailableMinutes = dto.AvailableMinutes;
            Bvin = dto.Bvin;
            FileName = dto.FileName;
            LastUpdated = dto.LastUpdated;
            MaxDownloads = dto.MaxDownloads;
            ProductId = dto.ProductId;
            ShortDescription = dto.ShortDescription;
            StoreId = dto.StoreId;
        }

        /// <summary>
        ///     Save file
        /// </summary>
        /// <param name="storeId">Store unique identifier</param>
        /// <param name="fileid">File unique identifer</param>
        /// <param name="fileName">File name</param>
        /// <param name="file"><see cref="HttpPostedFile" /> instance</param>
        /// <returns>Returns true if file saved successfully</returns>
        public static bool SaveFile(long storeId, string fileid, string fileName, HttpPostedFile file)
        {
            var diskFileName = fileid + "_" + fileName + ".config";
            DiskStorage.FileVaultUpload(storeId, diskFileName, file);
            return true;
        }

        /// <summary>
        ///     Save file
        /// </summary>
        /// <param name="storeId">Store unique identifier</param>
        /// <param name="fileid">File unique identifier</param>
        /// <param name="fileName">File Name</param>
        /// <param name="stream">File stream instance</param>
        /// <returns>Returs true if file saved successfully</returns>
        public static bool SaveFile(long storeId, string fileid, string fileName, FileStream stream)
        {
            var diskFileName = fileid + "_" + fileName + ".config";
            DiskStorage.FileVaultUpload(storeId, diskFileName, stream);
            return true;
        }

        /// <summary>
        ///     Save file
        /// </summary>
        /// <param name="storeId">Store unique identifier</param>
        /// <param name="fileId">File unique identifier</param>
        /// <param name="fileName">File name</param>
        /// <param name="fileData">File byte array</param>
        /// <returns>Returns true if the file saved successfully</returns>
        public static bool SaveFile(long storeId, string fileId, string fileName, byte[] fileData)
        {
            var diskFileName = fileId + "_" + fileName + ".config";
            DiskStorage.FileVaultUpload(storeId, diskFileName, fileData);
            return true;
        }

        /// <summary>
        ///     Save data from source to destination file
        /// </summary>
        /// <param name="readStream">Source file stream</param>
        /// <param name="writeStream">Destination file stream</param>
        private static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            var length = 256;
            var buffer = new byte[length];
            var bytesRead = readStream.Read(buffer, 0, length);
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, length);
            }
            readStream.Close();
            writeStream.Close();
        }

        /// <summary>
        ///     Get file steam of the given product file
        /// </summary>
        /// <param name="f"><see cref="ProductFile" /> instance</param>
        /// <param name="path">Path of the file</param>
        /// <returns>Returns Filesteam of the given product</returns>
        public static FileStream GetPhysicalFile(ProductFile f, string path)
        {
            var diskFileName = f.Bvin + "_" + f.FileName + ".config";
            return DiskStorage.FileVaultGetStream(f.StoreId, diskFileName);
        }

        // Time Helper
        public void SetMinutes(int Months, int Days, int Hours, int Minutes)
        {
            var TotalMinutes = 0;
            TotalMinutes += Months*43200;
            TotalMinutes += Days*1440;
            TotalMinutes += Hours*60;
            TotalMinutes += Minutes;

            AvailableMinutes = TotalMinutes;
        }
    }
}