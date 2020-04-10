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
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Configuration;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Search;
using Hotcakes.Commerce.Social;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Settings;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce
{
    public abstract class FactoryBase : IFactory
    {
        public abstract IHccUrlResolver CreateHccUrlResolver();
        public abstract IHccFormRenderer CreateHccFormRenderer();
        public abstract ILogger CreateEventLogger();
        public abstract ISocialService CreateSocialService(HccRequestContext context);
        public abstract ILocalizationHelper CreateLocalizationHelper(string resourceFile);
        public abstract IConfigurationManager CreateConfigurationManager();

        public abstract WorkflowFactory CreateWorkflowFactory(HccRequestContext context);
        public abstract PromotionFactory CreatePromotionFactory();

        public abstract IModuleSettingsProvider CreateModuleSettingsProvider(int moduleId);
        public abstract IHostSettingsProvider CreateHostSettingsProvider();
        public abstract IStoreSettingsProvider CreateStoreSettingsProvider();

        public abstract ISearchProvider CreateSearchProvider(HccRequestContext hccRequestContext);

        public virtual HccDbContext CreateHccDbContext()
        {
            var conn = WebAppSettings.HccEFConnectionString;
            return new HccDbContext(conn);
        }

        public virtual IRepoStrategy<T> CreateStrategy<T>() where T : class, new()
        {
            return new DbStrategy<T>(CreateHccDbContext());
        }

        /*
		* Point for creation and CACHING repositories and services.
		* */

        public virtual T CreateRepo<T>() where T : class, IRepo
        {
            return CreateRepo<T>(HccRequestContext.Current);
        }

        public virtual T CreateRepo<T>(HccRequestContext context) where T : class, IRepo
        {
            return (T) Activator.CreateInstance(typeof (T), context);
        }

        public virtual T CreateService<T>() where T : HccServiceBase
        {
            return CreateService<T>(HccRequestContext.Current);
        }

        public virtual T CreateService<T>(HccRequestContext context) where T : HccServiceBase
        {
            return (T) Activator.CreateInstance(typeof (T), context);
        }

        public virtual ISearchProvider CreateSearchProductProvider(HccRequestContext hccRequestContext)
        {
            return CreateSearchProvider(hccRequestContext);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public abstract ContactService CreateContactService(HccRequestContext context, bool isForMemoryOnly);

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public abstract MembershipServices CreateMembershipServices(HccRequestContext context, bool isForMemoryOnly);

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public abstract AccountService CreateAccountService(HccRequestContext context, bool isForMemoryOnly);

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public abstract CatalogService CreateCatalogService(HccRequestContext context, bool isForMemoryOnly);

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public WorkflowFactory CreateWorkflowFactory(HotcakesApplication hccApp)
        {
            return CreateWorkflowFactory(hccApp.CurrentRequestContext);
        }

        #endregion
    }
}