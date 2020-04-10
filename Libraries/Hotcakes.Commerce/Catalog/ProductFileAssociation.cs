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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Product file information
    /// </summary>
    public class ProductFileAssociation
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public ProductFileAssociation()
        {
            Id = 0;
            StoreId = 0;
            FileId = string.Empty;
            ProductId = string.Empty;
            AvailableMinutes = 0;
            MaxDownloads = 0;
            LastUpdatedUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Unique identifier for product and file mapping
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Unique store identifier
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     Unique file identifier
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        ///     Unique product identifier
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        ///     Available minutes for which this file would be available for download
        /// </summary>
        public int AvailableMinutes { get; set; }

        /// <summary>
        ///     Maximum allowed download of the file
        /// </summary>
        public int MaxDownloads { get; set; }

        /// <summary>
        ///     Last updated time of the file
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }
    }
}