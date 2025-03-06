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
        public TaxScheduleDTO()
        {
            Id = 0;
            StoreId = 0;
            Name = string.Empty;
            DefaultRate = 0;
            DefaultShippingRate = 0;
        }

        /// <summary>
        ///     The unique identifier or primary key of the tax schedule object
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     The date/time stamp that the referral was made, used for auditing purposes only.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The name of the tax schedule. This will be seen in the administration areas.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     This is used only for VAT implementations to help calculate the correct taxes per VAT rules.
        /// </summary>
        [DataMember]
        public decimal DefaultRate { get; set; }

        /// <summary>
        ///     This is used only for VAT implementations to help calculate the correct taxes for shipping per VAT rules.
        /// </summary>
        [DataMember]
        public decimal DefaultShippingRate { get; set; }
    }
}