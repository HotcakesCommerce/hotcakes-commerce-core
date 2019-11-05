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
using Hotcakes.CommerceDTO.v1.Taxes;

namespace Hotcakes.Commerce.Taxes
{
    /// <summary>
    ///     This is the main object used for tax schedules in the applied
    /// </summary>
    /// <remarks>The REST API equivalent of this class is TaxScheduleDTO</remarks>
    [Serializable]
    public class TaxSchedule : ITaxSchedule
    {
        public TaxSchedule()
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
        /// <remarks>This is mapped to the TaxScheduleId property in this same class.</remarks>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The name of the tax schedule. This will be seen in the administration areas.
        /// </summary>
        /// <remarks>This is mapped to the TaxScheduleName property in this same class.</remarks>
        public string Name { get; set; }

        /// <summary>
        ///     This is used only for VAT implementations to help calculate the correct taxes per VAT rules.
        /// </summary>
        /// <remarks>This is mapped to the TaxScheduleDefaultRate property in this same class.</remarks>
        public decimal DefaultRate { get; set; }

        /// <summary>
        ///     This is used only for VAT implementations to help calculate the correct taxes for shipping per VAT rules.
        /// </summary>
        /// <remarks>This is mapped to the TaxScheduleDefaultShippingRate property in this same class.</remarks>
        public decimal DefaultShippingRate { get; set; }

        #region ITaxSchedule Members

        /// <summary>
        ///     0
        ///     The unique identifier or primary key of the tax schedule object
        /// </summary>
        /// <returns>This simply returns the Id property and is necessary for the interface.</returns>
        public long TaxScheduleId()
        {
            return Id;
        }

        /// <summary>
        ///     The name of the tax schedule. This will be seen in the administration areas.
        /// </summary>
        /// <returns>This simply returns the Name property and is necessary for the interface.</returns>
        public string TaxScheduleName()
        {
            return Name;
        }

        /// <summary>
        ///     This is used only for VAT implementations to help calculate the correct taxes per VAT rules.
        /// </summary>
        /// <returns>This simply returns the DefaultRate property and is necessary for the interface.</returns>
        public decimal TaxScheduleDefaultRate()
        {
            return DefaultRate;
        }

        /// <summary>
        ///     This is used only for VAT implementations to help calculate the correct taxes for shipping per VAT rules.
        /// </summary>
        /// <returns>This simply returns the DefaultShippingRate property and is necessary for the interface.</returns>
        public decimal TaxScheduleDefaultShippingRate()
        {
            return DefaultShippingRate;
        }

        #endregion

        #region DTO

        /// <summary>
        ///     Allows you to convert the current tax schedule object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of TaxScheduleDTO</returns>
        public TaxScheduleDTO ToDto()
        {
            var dto = new TaxScheduleDTO
            {
                Id = Id,
                Name = Name,
                StoreId = StoreId,
                DefaultRate = DefaultRate,
                DefaultShippingRate = DefaultShippingRate
            };

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current tax schedule object using a TaxScheduleDTO instance
        /// </summary>
        /// <param name="dto">An instance of the tax schedule from the REST API</param>
        public void FromDto(TaxScheduleDTO dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            Name = dto.Name;
            StoreId = dto.StoreId;
            DefaultRate = dto.DefaultRate;
            DefaultShippingRate = dto.DefaultShippingRate;
        }

        #endregion
    }
}