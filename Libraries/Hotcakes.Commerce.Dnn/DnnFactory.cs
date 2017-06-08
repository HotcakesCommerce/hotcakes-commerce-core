#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using System.Reflection;
using BrandonHaynes.ModelAdapter.EntityFramework;
using DotNetNuke.Common.Utilities;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Configuration;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Dnn.Marketing;
using Hotcakes.Commerce.Dnn.Mvc;
using Hotcakes.Commerce.Dnn.Workflow;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Search;
using Hotcakes.Commerce.Social;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Settings;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnFactory : FactoryBase
    {
        public DnnFactory()
        {
            Mapping = new Dictionary<Type, Type>();

            Mapping.Add(typeof (AffiliateRepository), typeof (DnnAffiliateRepository));
            Mapping.Add(typeof (CategoryRepository), typeof (DnnCategoryRepository));
            Mapping.Add(typeof (ProductRepository), typeof (DnnProductRepository));
            Mapping.Add(typeof (ProductReviewRepository), typeof (DnnProductReviewRepository));

            Mapping.Add(typeof (AccountService), typeof (DnnAccountService));
            Mapping.Add(typeof (CatalogService), typeof (DnnCatalogService));
            Mapping.Add(typeof (ContactService), typeof (DnnContactService));
            Mapping.Add(typeof (MembershipServices), typeof (DnnMembershipServices));
        }

        private Dictionary<Type, Type> Mapping { get; set; }

        public override WorkflowFactory CreateWorkflowFactory(HccRequestContext context)
        {
            var settings = context.CurrentStore.Settings.Urls;

            if (!string.IsNullOrEmpty(settings.WorkflowPluginAssemblyAndType))
            {
                try
                {
                    var typeWf = Type.GetType(settings.WorkflowPluginAssemblyAndType);
                    return (WorkflowFactory) Activator.CreateInstance(typeWf);
                }
                catch
                {
                    settings.WorkflowPluginAssemblyAndType = string.Empty;
                    var accountService = CreateService<AccountService>();
                    accountService.Stores.Update(context.CurrentStore);
                }
            }

            return new DnnWorkflowFactory();
        }

        public override IHccUrlResolver CreateHccUrlResolver()
        {
            return new DnnHccUrlResolver();
        }

        public override IHccFormRenderer CreateHccFormRenderer()
        {
            return new DnnHccFormRenderer();
        }

        public override ILogger CreateEventLogger()
        {
            return new DnnEventLog();
        }

        public override ISocialService CreateSocialService(HccRequestContext context)
        {
            // TODO: Remove this in 3.1 so we don't have to use reflection
            var typeName = "SocialServiceBase";
            var fullTypeName = string.Format("Hotcakes.Commerce.Dnn.Social.{0}, Hotcakes.Commerce.Dnn", typeName);
            var type = Type.GetType(fullTypeName);
            return (ISocialService) Activator.CreateInstance(type, context);
        }

        public override ILocalizationHelper CreateLocalizationHelper(string resourceFile)
        {
            return new DnnLocalizationHelper(resourceFile);
        }

        public override PromotionFactory CreatePromotionFactory()
        {
            return new DnnPromotionFactory();
        }

        public override IConfigurationManager CreateConfigurationManager()
        {
            return new DnnConfigurationManager();
        }

        public override IModuleSettingsProvider CreateModuleSettingsProvider(int moduleId)
        {
            return new DnnModuleSettingsProvider(moduleId);
        }

        public override IHostSettingsProvider CreateHostSettingsProvider()
        {
            return new DnnHostSettingsProvider();
        }

        public override IStoreSettingsProvider CreateStoreSettingsProvider()
        {
            return new DnnStoreSettingsProvider();
        }

        public override ISearchProvider CreateSearchProvider(HccRequestContext hccRequestContext)
        {
            return new DnnSearchProvider(hccRequestContext);
        }

        public override HccDbContext CreateHccDbContext()
        {
            var connString = WebAppSettings.HccEFConnectionString;
            var objectQualifier = Config.GetObjectQualifer();
            var dataBaseOwner = Config.GetDataBaseOwner().TrimEnd('.');
            var connAdapter = new TablePrefixModelAdapter(objectQualifier, new TableSchemaModelAdapter(dataBaseOwner));
            var adapter = new ConnectionAdapter(connAdapter, Assembly.GetCallingAssembly());

            return new HccDbContext(adapter.AdaptConnection(connString), true);
        }

        public override T CreateRepo<T>(HccRequestContext context)
        {
            if (Mapping.ContainsKey(typeof (T)))
            {
                var type = Mapping[typeof (T)];
                return (T) CreateRepoOrService(type, context);
            }
            return base.CreateRepo<T>(context);
        }

        public override T CreateService<T>(HccRequestContext context)
        {
            if (Mapping.ContainsKey(typeof (T)))
            {
                var type = Mapping[typeof (T)];
                return (T) CreateRepoOrService(type, context);
            }
            return base.CreateService<T>(context);
        }

        private object CreateRepoOrService(Type type, HccRequestContext context)
        {
            return Activator.CreateInstance(type, context);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public override ContactService CreateContactService(HccRequestContext context, bool isForMemoryOnly)
        {
            return CreateService<ContactService>(context);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public override MembershipServices CreateMembershipServices(HccRequestContext context, bool isForMemoryOnly)
        {
            return new DnnMembershipServices(context, isForMemoryOnly);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public override AccountService CreateAccountService(HccRequestContext context, bool isForMemoryOnly)
        {
            return new DnnAccountService(context, isForMemoryOnly);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public override CatalogService CreateCatalogService(HccRequestContext context, bool isForMemoryOnly)
        {
            return new DnnCatalogService(context, isForMemoryOnly);
        }

        #endregion
    }
}