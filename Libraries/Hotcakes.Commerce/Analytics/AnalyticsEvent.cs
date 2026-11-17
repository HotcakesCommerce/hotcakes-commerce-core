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

namespace Hotcakes.Commerce.Analytics
{
    /// <summary>
    /// Represents one customer, shopping-session, or store analytics event.
    /// </summary>
    /// <remarks>
    /// An event is scoped to a store through <see cref="StoreId"/> and may be associated with a
    /// customer, browsing session, and shopping session. Persist and query instances through
    /// <see cref="AnalyticsEventsRepository"/> or record new events through
    /// <see cref="AnalyticsService"/>.
    /// </remarks>
    /// <seealso cref="ActionTypes"/>
    public class AnalyticsEvent
    {
        /// <summary>Gets or sets the database identifier of the analytics event.</summary>
        /// <value>The persisted analytics-event identifier.</value>
        public long AnalyticsEventId { get; set; }

        /// <summary>Gets or sets the identifier of the customer associated with the event.</summary>
        /// <value>The customer identifier, or <see langword="null"/> when the event is not associated with a customer.</value>
        public string UserId { get; set; }

        /// <summary>Gets or sets the browsing-session identifier associated with the event.</summary>
        /// <value>The session identifier, or <see langword="null"/> when no browsing session is available.</value>
        public Guid? SessionGuid { get; set; }

        /// <summary>Gets or sets the shopping-session identifier associated with the event.</summary>
        /// <value>The shopping-session identifier, or <see langword="null"/> when no shopping session is available.</value>
        public Guid? ShoppingSessionGuid { get; set; }

        /// <summary>Gets or sets the identifier of the store that owns the event.</summary>
        /// <value>The store identifier used to isolate analytics data between stores.</value>
        public long StoreId { get; set; }

        /// <summary>Gets or sets the type of action represented by the event.</summary>
        /// <value>The serialized analytics action.</value>
        /// <seealso cref="ActionTypes"/>
        public ActionTypes Action { get; set; }

        /// <summary>Gets or sets the identifier of the commerce object related to the event.</summary>
        /// <value>The related object identifier, or <see langword="null"/> when the event has no specific object.</value>
        public Guid? ObjectId { get; set; }

        /// <summary>Gets or sets additional event data.</summary>
        /// <value>Optional event-specific data stored with the analytics record.</value>
        /// <remarks>The content and format are determined by the caller and are not interpreted by this model.</remarks>
        public string AdditionalData { get; set; }

        /// <summary>Gets or sets the UTC date and time at which the event occurred.</summary>
        /// <value>The event timestamp.</value>
        public DateTime DateTime { get; set; }
    }
}
