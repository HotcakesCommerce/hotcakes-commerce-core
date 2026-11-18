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
    /// <summary>Defines provider creation contracts for services, settings, persistence, search, and extensions.</summary>
    public interface IFactory
    {
        /// <summary>Creates the URL resolver.</summary>
        IHccUrlResolver CreateHccUrlResolver();
        /// <summary>Creates the form renderer.</summary>
        IHccFormRenderer CreateHccFormRenderer();
        /// <summary>Creates the event logger.</summary>
        ILogger CreateEventLogger();
        /// <summary>Creates a social service.</summary><param name="context">The request context.</param>
        ISocialService CreateSocialService(HccRequestContext context);
        /// <summary>Creates a localization helper.</summary><param name="resourceFile">The resource file.</param>
        ILocalizationHelper CreateLocalizationHelper(string resourceFile);
        /// <summary>Creates the configuration manager.</summary>
        IConfigurationManager CreateConfigurationManager();

        /// <summary>Creates a workflow factory.</summary><param name="context">The request context.</param>
        WorkflowFactory CreateWorkflowFactory(HccRequestContext context);
        /// <summary>Creates the promotion factory.</summary>
        PromotionFactory CreatePromotionFactory();

        /// <summary>Creates a module-settings provider.</summary><param name="moduleId">The module identifier.</param>
        IModuleSettingsProvider CreateModuleSettingsProvider(int moduleId);
        /// <summary>Creates the host-settings provider.</summary>
        IHostSettingsProvider CreateHostSettingsProvider();
        /// <summary>Creates the store-settings provider.</summary>
        IStoreSettingsProvider CreateStoreSettingsProvider();

        /// <summary>Creates a search provider.</summary><param name="hccRequestContext">The request context.</param>
        ISearchProvider CreateSearchProvider(HccRequestContext hccRequestContext);

        /// <summary>Creates a read/write database context.</summary>
        HccDbContext CreateHccDbContext();
        /// <summary>Creates a read-only database context.</summary>
        HccDbContext CreateReadOnlyHccDbContext();
        /// <summary>Creates an entity persistence strategy.</summary><typeparam name="T">The entity type.</typeparam>
        IRepoStrategy<T> CreateStrategy<T>() where T : class, new();
        /// <summary>Creates a read-only persistence strategy.</summary><typeparam name="T">The entity type.</typeparam>
        IRepoStrategy<T> CreateReadStrategy<T>() where T : class, new();

        /// <summary>Creates a repository for the current context.</summary><typeparam name="T">The repository type.</typeparam>
        T CreateRepo<T>() where T : class, IRepo;
        /// <summary>Creates a repository for an explicit context.</summary><typeparam name="T">The repository type.</typeparam><param name="context">The request context.</param>
        T CreateRepo<T>(HccRequestContext context) where T : class, IRepo;

        /// <summary>Creates a service for the current context.</summary><typeparam name="T">The service type.</typeparam>
        T CreateService<T>() where T : HccServiceBase;
        /// <summary>Creates a service for an explicit context.</summary><typeparam name="T">The service type.</typeparam><param name="context">The request context.</param>
        T CreateService<T>(HccRequestContext context) where T : HccServiceBase;
    }
}
