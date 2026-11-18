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
using Hotcakes.CommerceDTO.v1.Taxes;

namespace Hotcakes.Commerce.Taxes
{
    /// <summary>Represents a store-scoped tax schedule and its default rates.</summary>
    /// <remarks>The REST API equivalent is <see cref="TaxScheduleDTO"/>. Default rates are percentage values and are used only by applicable VAT implementations.</remarks>
    [Serializable]
    public class TaxSchedule : ITaxSchedule
    {
        /// <summary>Initializes a tax schedule with zero identifiers and rates and an empty name.</summary>
        public TaxSchedule()
        {
            Id = 0;
            StoreId = 0;
            Name = string.Empty;
            DefaultRate = 0;
            DefaultShippingRate = 0;
        }

        /// <summary>Gets or sets the unique identifier or primary key of the tax schedule.</summary>
        /// <remarks>This is the value returned by the compatibility-sensitive <see cref="TaxScheduleId"/> method.</remarks>
        public long Id { get; set; }

        /// <summary>Gets or sets the Hotcakes store identifier that owns the schedule.</summary>
        /// <remarks>The repository assigns the current store when creating a schedule.</remarks>
        public long StoreId { get; set; }

        /// <summary>Gets or sets the tax schedule name displayed in administration areas.</summary>
        /// <remarks>This is the value returned by <see cref="TaxScheduleName"/>.</remarks>
        public string Name { get; set; }

        /// <summary>Gets or sets the default item tax rate for applicable VAT implementations.</summary>
        /// <remarks>The value is a percentage, not a decimal factor; consumers that calculate a factor divide it by 100. This is the value returned by <see cref="TaxScheduleDefaultRate"/>.</remarks>
        public decimal DefaultRate { get; set; }

        /// <summary>Gets or sets the default shipping tax rate for applicable VAT implementations.</summary>
        /// <remarks>The value is a percentage, not a decimal factor; consumers that calculate a factor divide it by 100. This is the value returned by <see cref="TaxScheduleDefaultShippingRate"/>.</remarks>
        public decimal DefaultShippingRate { get; set; }

        #region ITaxSchedule Members

        /// <summary>Gets the schedule identifier required by <see cref="ITaxSchedule"/>.</summary>
        /// <returns>The current <see cref="Id"/> value.</returns>
        public long TaxScheduleId()
        {
            return Id;
        }

        /// <summary>Gets the schedule name required by <see cref="ITaxSchedule"/>.</summary>
        /// <returns>The current <see cref="Name"/> value.</returns>
        public string TaxScheduleName()
        {
            return Name;
        }

        /// <summary>Gets the default item percentage required by <see cref="ITaxSchedule"/>.</summary>
        /// <returns>The current <see cref="DefaultRate"/> value.</returns>
        public decimal TaxScheduleDefaultRate()
        {
            return DefaultRate;
        }

        /// <summary>Gets the default shipping percentage required by <see cref="ITaxSchedule"/>.</summary>
        /// <returns>The current <see cref="DefaultShippingRate"/> value.</returns>
        public decimal TaxScheduleDefaultShippingRate()
        {
            return DefaultShippingRate;
        }

        #endregion

        #region DTO

        /// <summary>Converts this tax schedule model to its REST API DTO representation.</summary>
        /// <returns>A new <see cref="TaxScheduleDTO"/> containing the current property values.</returns>
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

        /// <summary>Copies values from a REST API DTO into this tax schedule model.</summary>
        /// <param name="dto">The DTO to copy, or <see langword="null"/>.</param>
        /// <remarks>If <paramref name="dto"/> is <see langword="null"/>, this method does nothing and preserves every current value.</remarks>
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
