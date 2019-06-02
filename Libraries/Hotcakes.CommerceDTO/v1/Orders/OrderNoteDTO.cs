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

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Order Notes in the REST API
    /// </summary>
    /// <remarks>The main application equivalent is OrderNote.</remarks>
    [DataContract]
    [Serializable]
    public class OrderNoteDTO
    {
        public OrderNoteDTO()
        {
            Id = 0;
            StoreId = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            OrderID = string.Empty;
            AuditDate = DateTime.UtcNow;
            Note = string.Empty;
            IsPublic = false;
        }

        /// <summary>
        ///     The unique ID of the current order note.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the order note was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The unique ID of the order that the current order note belongs to.
        /// </summary>
        [DataMember]
        public string OrderID { get; set; }

        /// <summary>
        ///     Used for auditing purposes to know when the order note was created.
        /// </summary>
        [DataMember]
        public DateTime AuditDate { get; set; }

        /// <summary>
        ///     The text that the note should contain, such as a message to the customer about shipping.
        /// </summary>
        [DataMember]
        public string Note { get; set; }

        /// <summary>
        ///     If true, the note will be shown to the customer, otherwise only merchants will see the note.
        /// </summary>
        [DataMember]
        public bool IsPublic { get; set; }
    }
}