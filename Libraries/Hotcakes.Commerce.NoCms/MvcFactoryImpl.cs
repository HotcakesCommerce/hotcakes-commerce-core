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
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.NoCms.Urls;
using Hotcakes.Commerce.Search;
using Hotcakes.Commerce.Social;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Settings;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.NoCms
{
    /// <summary>
    /// Supplies the MVC-host implementations of the HCC factory extension points.
    /// </summary>
    /// <remarks>
    /// This internal adapter is selected by the MVC host. Its public overrides are documented
    /// because they implement the provider-factory contract inherited from <see cref="FactoryBase"/>.
    /// Unsupported MVC integrations retain their established <see cref="NotImplementedException"/> behavior.
    /// </remarks>
    internal class MvcFactoryImpl : FactoryBase
    {
        /// <summary>
        /// Creates the MVC workflow factory.
        /// </summary>
        /// <param name="context">The current HCC request context.</param>
        /// <returns>A new MVC-compatible <see cref="WorkflowFactory"/>.</returns>
        public override WorkflowFactory CreateWorkflowFactory(HccRequestContext context)
        {
            return new WorkflowFactory();
        }

        /// <summary>
        /// Creates the MVC URL resolver.
        /// </summary>
        /// <returns>A new <see cref="MvcHccUrlResolver"/>.</returns>
        public override IHccUrlResolver CreateHccUrlResolver()
        {
            return new MvcHccUrlResolver();
        }

        /// <summary>
        /// Creates the MVC form renderer.
        /// </summary>
        /// <returns>A new <see cref="MvcHccFormRenderer"/>.</returns>
        public override IHccFormRenderer CreateHccFormRenderer()
        {
            return new MvcHccFormRenderer();
        }

        /// <summary>
        /// Creates the MVC event logger.
        /// </summary>
        /// <returns>A new <see cref="MvcEventLog"/>.</returns>
        public override ILogger CreateEventLogger()
        {
            return new MvcEventLog();
        }

        /// <summary>
        /// Creates the social-service implementation for the MVC host.
        /// </summary>
        /// <param name="context">The current HCC request context.</param>
        /// <returns>A social-service implementation when supported.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not provide social services.</exception>
        public override ISocialService CreateSocialService(HccRequestContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the MVC localization helper.
        /// </summary>
        /// <param name="resourceFile">The resource file identifier.</param>
        /// <returns>A new <see cref="MvcLocalizationHelper"/>.</returns>
        public override ILocalizationHelper CreateLocalizationHelper(string resourceFile)
        {
            return new MvcLocalizationHelper();
        }

        /// <summary>
        /// Creates the promotion factory used by the MVC host.
        /// </summary>
        /// <returns>A new <see cref="PromotionFactory"/>.</returns>
        public override PromotionFactory CreatePromotionFactory()
        {
            return new PromotionFactory();
        }


        /// <summary>
        /// Creates the MVC configuration manager.
        /// </summary>
        /// <returns>A configuration manager when supported.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not provide configuration management.</exception>
        public override IConfigurationManager CreateConfigurationManager()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a module-settings provider.
        /// </summary>
        /// <param name="moduleId">The module identifier for the settings provider.</param>
        /// <returns>A module-settings provider when supported.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not provide module settings.</exception>
        public override IModuleSettingsProvider CreateModuleSettingsProvider(int moduleId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a host-settings provider.
        /// </summary>
        /// <returns>A host-settings provider when supported.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not provide host settings.</exception>
        public override IHostSettingsProvider CreateHostSettingsProvider()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a store-settings provider.
        /// </summary>
        /// <returns>A store-settings provider when supported.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not provide store settings.</exception>
        public override IStoreSettingsProvider CreateStoreSettingsProvider()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the search provider for the MVC host.
        /// </summary>
        /// <param name="hccRequestContext">The current HCC request context.</param>
        /// <returns>A search provider when supported.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not provide search services.</exception>
        public override ISearchProvider CreateSearchProvider(HccRequestContext hccRequestContext)
        {
            throw new NotImplementedException();
        }
    }
}
