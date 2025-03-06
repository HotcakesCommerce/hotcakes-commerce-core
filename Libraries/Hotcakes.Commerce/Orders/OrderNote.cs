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
using Hotcakes.CommerceDTO.v1.Orders;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Order Notes
    /// </summary>
    /// <remarks>The REST API equivalent is OrderNoteDTO.</remarks>
    [Serializable]
    public class OrderNote
    {
        public OrderNote()
            : this(string.Empty)
        {
        }

        public OrderNote(string note)
        {
            Id = 0;
            StoreId = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            OrderID = string.Empty;
            AuditDate = DateTime.UtcNow;
            Note = note;
            IsPublic = false;
        }

        /// <summary>
        ///     The unique ID of the current order note.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the order note was last updated.
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The unique ID of the order that the current order note belongs to.
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        ///     Used for auditing purposes to know when the order note was created.
        /// </summary>
        public DateTime AuditDate { get; set; }

        /// <summary>
        ///     The text that the note should contain, such as a message to the customer about shipping.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        ///     If true, the note will be shown to the customer, otherwise only merchants will see the note.
        /// </summary>
        public bool IsPublic { get; set; }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current order note object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of OrderNoteDTO</returns>
        public OrderNoteDTO ToDto()
        {
            var dto = new OrderNoteDTO
            {
                AuditDate = AuditDate,
                Id = Id,
                IsPublic = IsPublic,
                LastUpdatedUtc = LastUpdatedUtc,
                Note = Note ?? string.Empty,
                OrderID = OrderID ?? string.Empty,
                StoreId = StoreId
            };

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current order note object using a OrderNoteDTO instance
        /// </summary>
        /// <param name="dto">An instance of the order note from the REST API</param>
        public void FromDto(OrderNoteDTO dto)
        {
            if (dto == null) return;

            AuditDate = dto.AuditDate;
            Id = dto.Id;
            IsPublic = dto.IsPublic;
            LastUpdatedUtc = dto.LastUpdatedUtc;
            Note = dto.Note ?? string.Empty;
            OrderID = dto.OrderID ?? string.Empty;
            StoreId = dto.StoreId;
        }

        #endregion
    }
}