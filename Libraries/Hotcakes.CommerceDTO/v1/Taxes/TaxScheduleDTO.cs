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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Taxes
{
    /// <summary>
    ///     This is the main object used for tax schedules in the REST API
    /// </summary>
    /// <remarks>The main application equivalent of this class is TaxSchedule</remarks>
    [DataContract]
    [Serializable]
    public class TaxScheduleDTO
    {
        /// <summary>
        /// Initializes a tax schedule with an empty name and zero-valued identifiers and rates.
        /// </summary>
        public TaxScheduleDTO()
        {
            Id = 0;
            StoreId = 0;
            Name = string.Empty;
            DefaultRate = 0;
            DefaultShippingRate = 0;
        }

        /// <summary>
        /// Gets or sets the unique identifier of the tax schedule.
        /// </summary>
        /// <value>The tax schedule identifier.</value>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the store that owns the tax schedule.
        /// </summary>
        /// <value>The Hotcakes store identifier.</value>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        /// Gets or sets the name of the tax schedule.
        /// </summary>
        /// <remarks>The name is displayed in administration areas.</remarks>
        /// <value>The tax schedule name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the default tax rate used by VAT implementations.
        /// </summary>
        /// <remarks>This value helps calculate taxes according to applicable VAT rules.</remarks>
        /// <value>The default decimal tax rate.</value>
        [DataMember]
        public decimal DefaultRate { get; set; }

        /// <summary>
        /// Gets or sets the default shipping tax rate used by VAT implementations.
        /// </summary>
        /// <remarks>This value helps calculate shipping taxes according to applicable VAT rules.</remarks>
        /// <value>The default decimal shipping tax rate.</value>
        [DataMember]
        public decimal DefaultShippingRate { get; set; }
    }
}
