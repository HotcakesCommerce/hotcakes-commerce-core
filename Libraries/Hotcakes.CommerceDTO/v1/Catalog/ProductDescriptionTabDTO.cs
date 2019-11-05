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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This object holds a single instance of an "Info Tab" that customers use to list product specs and other details.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ProductDescriptionTabDTO
    {
        public ProductDescriptionTabDTO()
        {
            Bvin = string.Empty;
            TabTitle = string.Empty;
            HtmlData = string.Empty;
            SortOrder = 0;
        }

        /// <summary>
        ///     The unique ID &amp; primary key of the info tab.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     The localized title as you want a customer to see it.
        /// </summary>
        [DataMember]
        public string TabTitle { get; set; }

        /// <summary>
        ///     This is the content of the info tab that is shown to customers.
        /// </summary>
        [DataMember]
        public string HtmlData { get; set; }

        /// <summary>
        ///     Sorting is supported by giving each info tab a sequential number.
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }
    }
}