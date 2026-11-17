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
using Hotcakes.Commerce.Data;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Analytics
{
    /// <summary>
    /// Records and removes store-scoped customer and commerce analytics events.
    /// </summary>
    /// <remarks>
    /// Session and store identifiers are captured when the service is constructed. Create a new
    /// service when the request scope changes. Events are persisted through
    /// <see cref="AnalyticsEventsRepository"/> and receive a UTC timestamp when registered.
    /// </remarks>
    /// <seealso cref="AnalyticsEvent"/>
    /// <seealso cref="ActionTypes"/>
    public class AnalyticsService : HccServiceBase
    {
        private Guid? _cachedSessionGuid;
        private Guid? _cachedShoppingSessionGuid;
        private long _cachedStoreId;

        /// <summary>
        /// Initializes the analytics service for the specified commerce request context.
        /// </summary>
        /// <param name="context">The request context used to determine the store and current sessions.</param>
        public AnalyticsService(HccRequestContext context)
            : base(context)
        {
            AnalyticsEvents = Factory.CreateRepo<AnalyticsEventsRepository>(Context);

            // Cache frequently accessed values once during initialization
            _cachedSessionGuid = SessionManager.GetCurrentSessionGuid();
            _cachedShoppingSessionGuid = SessionManager.GetCurrentShoppingSessionGuid();
            _cachedStoreId = Context.CurrentStore.Id;
        }

        /// <summary>
        /// Gets the repository used to persist analytics events.
        /// </summary>
        /// <value>The repository associated with this service's request context.</value>
        public AnalyticsEventsRepository AnalyticsEvents { get; protected set; }

        /// <summary>
        /// Registers one analytics event for the current store and captured session context.
        /// </summary>
        /// <param name="userId">The customer identifier associated with the event, if available.</param>
        /// <param name="actionType">The action represented by the event.</param>
        /// <param name="objectId">The commerce object identifier in BVIN format, if applicable.</param>
        /// <remarks>
        /// The object identifier is converted to a nullable GUID, and the event timestamp is assigned
        /// using <see cref="DateTime.UtcNow"/>. The event is persisted immediately through
        /// <see cref="AnalyticsEvents"/>.
        /// </remarks>
        public void RegisterEvent(string userId, ActionTypes actionType, string objectId)
        {
            var analyticsEvent = new AnalyticsEvent();
            analyticsEvent.UserId = userId;
            analyticsEvent.SessionGuid = _cachedSessionGuid;
            analyticsEvent.ShoppingSessionGuid = _cachedShoppingSessionGuid;
            analyticsEvent.StoreId = _cachedStoreId;
            analyticsEvent.Action = actionType;
            analyticsEvent.ObjectId = DataTypeHelper.BvinToNullableGuid(objectId);
            analyticsEvent.DateTime = DateTime.UtcNow;

            AnalyticsEvents.Create(analyticsEvent);
        }

        /// <summary>
        /// Registers multiple analytics events for the current store and captured session context.
        /// </summary>
        /// <param name="events">The event tuples containing customer identifier, action, and optional object identifier.</param>
        /// <remarks>
        /// A null or empty list is ignored. All events in a non-empty batch receive the same UTC
        /// timestamp and are persisted through one repository batch operation.
        /// </remarks>
        public void RegisterEvents(List<Tuple<string, ActionTypes, string>> events)
        {
            if (events == null || events.Count == 0)
                return;

            var analyticsEvents = new List<AnalyticsEvent>();
            var timestamp = DateTime.UtcNow;

            foreach (var eventData in events)
            {
                var analyticsEvent = new AnalyticsEvent
                {
                    UserId = eventData.Item1,
                    SessionGuid = _cachedSessionGuid,
                    ShoppingSessionGuid = _cachedShoppingSessionGuid,
                    StoreId = _cachedStoreId,
                    Action = eventData.Item2,
                    ObjectId = DataTypeHelper.BvinToNullableGuid(eventData.Item3),
                    DateTime = timestamp
                };
                analyticsEvents.Add(analyticsEvent);
            }

            AnalyticsEvents.BatchCreate(analyticsEvents, mergeSubItems: false);
        }

        /// <summary>
        /// Deletes analytics events associated with a customer identifier.
        /// </summary>
        /// <param name="userID">The customer identifier whose events should be deleted.</param>
        /// <remarks>This operation is destructive and delegates store/authorization scope handling to the repository and caller.</remarks>
        public void DeleteEventsByCustomer(string userID)
        {
            AnalyticsEvents.DeleteByUserId(userID);
        }

        /// <summary>
        /// Deletes all analytics events for a store by invoking the database delete procedure.
        /// </summary>
        /// <param name="storeId">The store identifier whose analytics events should be deleted.</param>
        /// <returns><see langword="true"/> after the delete procedure is invoked.</returns>
        /// <remarks>This is a destructive operation; callers must authorize the supplied store scope before invoking it.</remarks>
        public bool DeleteEventsByStoreID(long storeId)
        {
            using (var db = Factory.CreateHccDbContext())
            {
                db.DeleteStoreAnalyticsEvents(storeId);
            }
            return true;
        }

        protected IRepoStrategy<AnalyticsEvent> CreateAnalyticsStrategy()
        {
            return Factory.Instance.CreateStrategy<AnalyticsEvent>();
        }
    }
}
