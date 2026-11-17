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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Analytics
{
    /// <summary>
    /// Provides persistence operations for store-scoped <see cref="AnalyticsEvent"/> records.
    /// </summary>
    /// <remarks>
    /// Queries and deletes are executed against the analytics event data store. Methods that accept
    /// a store identifier or customer identifier do not add authorization checks; callers must supply
    /// identifiers appropriate to the current operation and enforce any required access policy.
    /// </remarks>
    /// <seealso cref="AnalyticsService"/>
    public class AnalyticsEventsRepository : HccSimpleRepoBase<hcc_AnalyticsEvent, AnalyticsEvent>
    {
        /// <summary>
        /// Initializes a repository using the specified commerce request context.
        /// </summary>
        /// <param name="context">The request context that supplies the current commerce scope.</param>
        public AnalyticsEventsRepository(HccRequestContext context)
            : base(context)
        {
        }

        protected override void CopyDataToModel(hcc_AnalyticsEvent data, AnalyticsEvent model)
        {
            model.AnalyticsEventId = data.AnalyticsEventId;
            model.UserId = data.UserId;
            model.SessionGuid = data.SessionGuid;
            model.ShoppingSessionGuid = data.ShoppingSessionGuid;
            model.StoreId = data.StoreId;
            model.Action = (ActionTypes) Enum.Parse(typeof (ActionTypes), data.Action);
            model.ObjectId = data.ObjectId;
            model.AdditionalData = data.AdditionalData;
            model.DateTime = data.DateTime;
        }

        protected override void CopyModelToData(hcc_AnalyticsEvent data, AnalyticsEvent model)
        {
            data.AnalyticsEventId = model.AnalyticsEventId;
            data.UserId = model.UserId;
            data.SessionGuid = model.SessionGuid;
            data.ShoppingSessionGuid = model.ShoppingSessionGuid;
            data.StoreId = model.StoreId;
            data.Action = model.Action.ToString();
            data.ObjectId = model.ObjectId;
            data.AdditionalData = model.AdditionalData;
            data.DateTime = model.DateTime;
        }

        /// <summary>
        /// Deletes one analytics event by its database identifier.
        /// </summary>
        /// <param name="analyticsEventId">The identifier of the event to delete.</param>
        /// <returns><see langword="true"/> when the repository delete operation succeeds; otherwise, <see langword="false"/>.</returns>
        public bool Delete(long analyticsEventId)
        {
            return Delete(a => a.AnalyticsEventId == analyticsEventId);
        }

        /// <summary>
        /// Deletes analytics events associated with a customer identifier.
        /// </summary>
        /// <param name="userId">The customer identifier whose events should be deleted.</param>
        /// <returns><see langword="true"/> when the repository delete operation succeeds; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This method does not independently verify that the customer belongs to the current store.</remarks>
        public bool DeleteByUserId(string userId)
        {
            return Delete(a => a.UserId == userId);
        }

        /// <summary>
        /// Deletes all analytics events for the specified store.
        /// </summary>
        /// <param name="storID">The store identifier whose events should be deleted.</param>
        /// <returns><see langword="true"/> when the repository delete operation succeeds; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This is a destructive operation and does not infer the store identifier from the request context.</remarks>
        public bool DeleteAllForCurrentStore(long storID)
        {
            return Delete(a => a.StoreId == storID);
        }

        /// <summary>
        /// Finds analytics events associated with a browsing session.
        /// </summary>
        /// <param name="sessionGuid">The browsing-session identifier to find.</param>
        /// <returns>A list of matching analytics events.</returns>
        public List<AnalyticsEvent> FindBySessionGuid(Guid sessionGuid)
        {
            return FindListPoco(q => { return q.Where(a => a.SessionGuid == sessionGuid); });
        }

        /// <summary>
        /// Finds analytics events associated with a customer identifier.
        /// </summary>
        /// <param name="userId">The customer identifier to find.</param>
        /// <returns>A list of matching analytics events.</returns>
        /// <remarks>This method does not independently verify that the customer belongs to the current store.</remarks>
        public List<AnalyticsEvent> FindByUserId(string userId)
        {
            return FindListPoco(q => { return q.Where(a => a.UserId == userId); });
        }
    }
}
