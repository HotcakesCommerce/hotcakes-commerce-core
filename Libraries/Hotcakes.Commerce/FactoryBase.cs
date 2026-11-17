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
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Configuration;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Search;
using Hotcakes.Commerce.Social;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Settings;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce
{
    /// <summary>Provides the standard provider-factory implementation.</summary>
    /// <remarks>Override provider-specific creation points while retaining repository and service construction contracts.</remarks>
    public abstract class FactoryBase : IFactory
    {
        /// <summary>Creates the URL resolver.</summary>
        public abstract IHccUrlResolver CreateHccUrlResolver();
        /// <summary>Creates the form renderer.</summary>
        public abstract IHccFormRenderer CreateHccFormRenderer();
        /// <summary>Creates the event logger.</summary>
        public abstract ILogger CreateEventLogger();
        /// <summary>Creates a social service.</summary><param name="context">The request context.</param>
        public abstract ISocialService CreateSocialService(HccRequestContext context);
        /// <summary>Creates a localization helper.</summary><param name="resourceFile">The resource file identifier.</param>
        public abstract ILocalizationHelper CreateLocalizationHelper(string resourceFile);
        /// <summary>Creates the configuration manager.</summary>
        public abstract IConfigurationManager CreateConfigurationManager();

        /// <summary>Creates a workflow factory.</summary><param name="context">The request context.</param>
        public abstract WorkflowFactory CreateWorkflowFactory(HccRequestContext context);
        /// <summary>Creates the promotion factory.</summary>
        public abstract PromotionFactory CreatePromotionFactory();

        /// <summary>Creates a module-settings provider.</summary><param name="moduleId">The module identifier.</param>
        public abstract IModuleSettingsProvider CreateModuleSettingsProvider(int moduleId);
        /// <summary>Creates the host-settings provider.</summary>
        public abstract IHostSettingsProvider CreateHostSettingsProvider();
        /// <summary>Creates the store-settings provider.</summary>
        public abstract IStoreSettingsProvider CreateStoreSettingsProvider();

        /// <summary>Creates a search provider.</summary><param name="hccRequestContext">The request context.</param>
        public abstract ISearchProvider CreateSearchProvider(HccRequestContext hccRequestContext);

        /// <summary>Creates a read/write context using the configured EF connection.</summary>
        public virtual HccDbContext CreateHccDbContext()
        {
            var conn = WebAppSettings.HccEFConnectionString;
            return new HccDbContext(conn);
        }

        /// <summary>Creates a read-only context using the configured connection.</summary>
        public virtual HccDbContext CreateReadOnlyHccDbContext()
        {
            var conn = WebAppSettings.HccEFReadOnlyConnectionString;
            return new HccDbContext(conn);
        }

        /// <summary>Creates a database strategy.</summary><typeparam name="T">The entity type.</typeparam>
        public virtual IRepoStrategy<T> CreateStrategy<T>() where T : class, new()
        {
            return new DbStrategy<T>(CreateHccDbContext());
        }

        /// <summary>Creates a read strategy.</summary><typeparam name="T">The entity type.</typeparam>
        public virtual IRepoStrategy<T> CreateReadStrategy<T>() where T : class, new()
        {
            return new DbStrategy<T>(CreateReadOnlyHccDbContext());
        }

        /*
		* Point for creation and CACHING repositories and services.
		* */

        /// <summary>Creates a repository using <see cref="HccRequestContext.Current"/>; this is the repository creation/caching extension point.</summary><typeparam name="T">The repository type.</typeparam>
        public virtual T CreateRepo<T>() where T : class, IRepo
        {
            return CreateRepo<T>(HccRequestContext.Current);
        }

        /// <summary>Creates a repository using an explicit context.</summary><typeparam name="T">The repository type.</typeparam><param name="context">The request context.</param>
        public virtual T CreateRepo<T>(HccRequestContext context) where T : class, IRepo
        {
            return (T) Activator.CreateInstance(typeof (T), context);
        }

        /// <summary>Creates a service using <see cref="HccRequestContext.Current"/>.</summary><typeparam name="T">The service type.</typeparam>
        public virtual T CreateService<T>() where T : HccServiceBase
        {
            return CreateService<T>(HccRequestContext.Current);
        }

        /// <summary>Creates a service using an explicit context.</summary><typeparam name="T">The service type.</typeparam><param name="context">The request context.</param>
        public virtual T CreateService<T>(HccRequestContext context) where T : HccServiceBase
        {
            return (T) Activator.CreateInstance(typeof (T), context);
        }

        /// <summary>Creates the product search provider through <see cref="CreateSearchProvider"/>; the name is a compatibility extension-point quirk.</summary><param name="hccRequestContext">The request context.</param>
        public virtual ISearchProvider CreateSearchProductProvider(HccRequestContext hccRequestContext)
        {
            return CreateSearchProvider(hccRequestContext);
        }
    }
}
