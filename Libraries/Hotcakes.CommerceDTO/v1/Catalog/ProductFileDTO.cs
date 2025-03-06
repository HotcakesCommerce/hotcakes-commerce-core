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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This object holds the information of single instance of the Produce file.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ProductFileDTO
    {
        /// <summary>
        ///     consturctor to set the default values of the all properties on
        ///     initializition of this type of object.
        /// </summary>
        public ProductFileDTO()
        {
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            StoreId = 0;
            ProductId = string.Empty;
            AvailableMinutes = 0;
            MaxDownloads = 0;
            FileName = string.Empty;
            ShortDescription = string.Empty;
        }

        /// <summary>
        ///     The unique ID or primary key of the product file.
        /// </summary>
        /// <remarks>This property should always be used instead of Id.</remarks>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     Last update when this file has been updated by system user.
        /// </summary>
        [DataMember]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The unique ID or primary key of the product.
        /// </summary>
        /// <remarks>This property should always be used instead of Id.</remarks>
        [DataMember]
        public string ProductId { get; set; }

        /// <summary>
        ///     Variable which show s how long this file is avaible for download.
        /// </summary>
        [DataMember]
        public int AvailableMinutes { get; set; }

        /// <summary>
        ///     Represents how many maximum downloads can be done for this file from website.
        /// </summary>
        [DataMember]
        public int MaxDownloads { get; set; }

        /// <summary>
        ///     Name of the file which displayed to the end user on site.
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        ///     Description of the file to show what file is used for and purpose of it.
        /// </summary>
        [DataMember]
        public string ShortDescription { get; set; }
    }
}