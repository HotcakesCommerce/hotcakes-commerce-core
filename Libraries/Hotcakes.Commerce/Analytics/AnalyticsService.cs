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
using System.Collections.Generic;
using Hotcakes.Commerce.Data;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Analytics
{
    public class AnalyticsService : HccServiceBase
    {
        private Guid? _cachedSessionGuid;
        private Guid? _cachedShoppingSessionGuid;
        private long _cachedStoreId;

        public AnalyticsService(HccRequestContext context)
            : base(context)
        {
            AnalyticsEvents = Factory.CreateRepo<AnalyticsEventsRepository>(Context);

            // Cache frequently accessed values once during initialization
            _cachedSessionGuid = SessionManager.GetCurrentSessionGuid();
            _cachedShoppingSessionGuid = SessionManager.GetCurrentShoppingSessionGuid();
            _cachedStoreId = Context.CurrentStore.Id;
        }

        public AnalyticsEventsRepository AnalyticsEvents { get; protected set; }

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

        public void DeleteEventsByCustomer(string userID)
        {
            AnalyticsEvents.DeleteByUserId(userID);
        }

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