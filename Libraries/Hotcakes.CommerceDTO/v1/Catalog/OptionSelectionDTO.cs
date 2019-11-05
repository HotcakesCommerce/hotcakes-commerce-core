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
    ///     This is the primary class used for option selection in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is OptionSelection.</remarks>
    [DataContract]
    [Serializable]
    public class OptionSelectionDTO
    {
        public OptionSelectionDTO()
        {
            OptionBvin = string.Empty;
            SelectionData = string.Empty;
        }

        /// <summary>
        ///     The unique ID or primary key of the option.
        /// </summary>
        [DataMember]
        public string OptionBvin { get; set; }

        /// <summary>
        ///     The options or choices selected for the product.
        /// </summary>
        [DataMember]
        public string SelectionData { get; set; }
    }
}