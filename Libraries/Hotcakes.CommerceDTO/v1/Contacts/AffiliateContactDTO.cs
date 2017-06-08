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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Contacts
{
    /// <summary>
    ///     This is the main object that is used for all affiliate contact management in the REST API
    /// </summary>
    /// <remarks>The main appplication equivalent to this is the AffiliateContact object</remarks>
    [DataContract]
    [Serializable]
    [Obsolete("Obsolete in 1.8.0. Affiliate contacts are not used")]
    public class AffiliateContactDTO
    {
        public AffiliateContactDTO()
        {
            Id = 0;
            AffiliateId = string.Empty;
            UserId = string.Empty;
            StoreId = 0;
        }

        /// <summary>
        ///     A unique ID for the affiliate contact used to manage the object in the data source.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The unique ID or bvin of the affiliate that this contact is attached to.
        /// </summary>
        [DataMember]
        public string AffiliateId { get; set; }

        /// <summary>
        ///     The unique ID to identify and work with the affiliate contact in the CMS.
        /// </summary>
        [DataMember]
        public string UserId { get; set; }
    }
}