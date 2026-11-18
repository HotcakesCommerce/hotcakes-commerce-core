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
    /// <summary>
    /// Creates DNN-specific Hotcakes services, repositories, providers, and data contexts through <see cref="FactoryBase"/>.
    /// </summary>
    [Serializable]
    public class DnnFactory : FactoryBase
    {
        /// <summary>
        /// Initializes a factory with the repository and service mappings used by DNN.
        /// </summary>
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

        /// <summary>
        /// Creates the configured workflow factory, falling back to the DNN workflow factory when no valid plugin is configured.
        /// </summary>
        /// <param name="context">The current Hotcakes request context.</param>
        /// <returns>The workflow factory for the current store.</returns>
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

        /// <summary>
        /// Creates the DNN URL resolver.
        /// </summary>
        /// <returns>A DNN-specific URL resolver.</returns>
        public override IHccUrlResolver CreateHccUrlResolver()
        {
            return new DnnHccUrlResolver();
        }

        /// <summary>
        /// Creates the DNN form renderer.
        /// </summary>
        /// <returns>A DNN-specific form renderer.</returns>
        public override IHccFormRenderer CreateHccFormRenderer()
        {
            return new DnnHccFormRenderer();
        }

        /// <summary>
        /// Creates the logger that writes Hotcakes events to the DNN event log.
        /// </summary>
        /// <returns>A DNN event logger.</returns>
        public override ILogger CreateEventLogger()
        {
            return new DnnEventLog();
        }

        /// <summary>
        /// Creates the DNN social service for the current request.
        /// </summary>
        /// <param name="context">The current Hotcakes request context.</param>
        /// <returns>A DNN social service.</returns>
        public override ISocialService CreateSocialService(HccRequestContext context)
        {
            // TODO: Remove this in 3.1 so we don't have to use reflection
            var typeName = "SocialServiceBase";
            var fullTypeName = string.Format("Hotcakes.Commerce.Dnn.Social.{0}, Hotcakes.Commerce.Dnn", typeName);
            var type = Type.GetType(fullTypeName);
            return (ISocialService) Activator.CreateInstance(type, context);
        }

        /// <summary>
        /// Creates a localization helper for a resource file.
        /// </summary>
        /// <param name="resourceFile">The resource file used for localized values.</param>
        /// <returns>A DNN localization helper.</returns>
        public override ILocalizationHelper CreateLocalizationHelper(string resourceFile)
        {
            return new DnnLocalizationHelper(resourceFile);
        }

        /// <summary>
        /// Creates the DNN promotion factory.
        /// </summary>
        /// <returns>A DNN-specific promotion factory.</returns>
        public override PromotionFactory CreatePromotionFactory()
        {
            return new DnnPromotionFactory();
        }

        /// <summary>
        /// Creates the DNN configuration manager.
        /// </summary>
        /// <returns>A configuration manager backed by DNN host settings.</returns>
        public override IConfigurationManager CreateConfigurationManager()
        {
            return new DnnConfigurationManager();
        }

        /// <summary>
        /// Creates a settings provider for a DNN module.
        /// </summary>
        /// <param name="moduleId">The DNN module identifier whose settings are managed.</param>
        /// <returns>A module settings provider for the specified module.</returns>
        public override IModuleSettingsProvider CreateModuleSettingsProvider(int moduleId)
        {
            return new DnnModuleSettingsProvider(moduleId);
        }

        /// <summary>
        /// Creates the provider used to read DNN host settings.
        /// </summary>
        /// <returns>A DNN host settings provider.</returns>
        public override IHostSettingsProvider CreateHostSettingsProvider()
        {
            return new DnnHostSettingsProvider();
        }

        /// <summary>
        /// Creates the provider used to read store settings and locales from DNN.
        /// </summary>
        /// <returns>A DNN store settings provider.</returns>
        public override IStoreSettingsProvider CreateStoreSettingsProvider()
        {
            return new DnnStoreSettingsProvider();
        }

        /// <summary>
        /// Creates a DNN-aware product search provider.
        /// </summary>
        /// <param name="hccRequestContext">The current Hotcakes request context.</param>
        /// <returns>A search provider that applies DNN catalog security filtering.</returns>
        public override ISearchProvider CreateSearchProvider(HccRequestContext hccRequestContext)
        {
            return new DnnSearchProvider(hccRequestContext);
        }

        /// <summary>
        /// Creates a writable Hotcakes database context using DNN connection and naming settings.
        /// </summary>
        /// <returns>A configured Hotcakes database context.</returns>
        public override HccDbContext CreateHccDbContext()
        {
            var connString = WebAppSettings.HccEFConnectionString;
            var objectQualifier = Config.GetObjectQualifer();
            var dataBaseOwner = Config.GetDataBaseOwner().TrimEnd('.');
            var connAdapter = new TablePrefixModelAdapter(objectQualifier, new TableSchemaModelAdapter(dataBaseOwner));
            var adapter = new ConnectionAdapter(connAdapter, Assembly.GetCallingAssembly());

            return new HccDbContext(adapter.AdaptConnection(connString), true);
        }

        /// <summary>
        /// Creates a read-only Hotcakes database context using DNN connection and naming settings.
        /// </summary>
        /// <returns>A configured read-only Hotcakes database context.</returns>
        public override HccDbContext CreateReadOnlyHccDbContext()
        {
            var connString = WebAppSettings.HccEFReadOnlyConnectionString;
            var objectQualifier = Config.GetObjectQualifer();
            var dataBaseOwner = Config.GetDataBaseOwner().TrimEnd('.');
            var connAdapter = new TablePrefixModelAdapter(objectQualifier, new TableSchemaModelAdapter(dataBaseOwner));
            var adapter = new ConnectionAdapter(connAdapter, Assembly.GetCallingAssembly());

            return new HccDbContext(adapter.AdaptConnection(connString), true);
        }

        /// <summary>
        /// Creates a repository, using the DNN implementation when one is registered for the requested type.
        /// </summary>
        /// <typeparam name="T">The repository type to create.</typeparam>
        /// <param name="context">The current Hotcakes request context.</param>
        /// <returns>A repository instance for the requested type.</returns>
        public override T CreateRepo<T>(HccRequestContext context)
        {
            if (Mapping.ContainsKey(typeof (T)))
            {
                var type = Mapping[typeof (T)];
                return (T) CreateRepoOrService(type, context);
            }
            return base.CreateRepo<T>(context);
        }

        /// <summary>
        /// Creates a service, using the DNN implementation when one is registered for the requested type.
        /// </summary>
        /// <typeparam name="T">The service type to create.</typeparam>
        /// <param name="context">The current Hotcakes request context.</param>
        /// <returns>A service instance for the requested type.</returns>
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
    }
}
