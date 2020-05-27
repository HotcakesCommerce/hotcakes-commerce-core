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
    public interface IFactory
    {
        IHccUrlResolver CreateHccUrlResolver();
        IHccFormRenderer CreateHccFormRenderer();
        ILogger CreateEventLogger();
        ISocialService CreateSocialService(HccRequestContext context);
        ILocalizationHelper CreateLocalizationHelper(string resourceFile);
        IConfigurationManager CreateConfigurationManager();

        WorkflowFactory CreateWorkflowFactory(HccRequestContext context);
        PromotionFactory CreatePromotionFactory();

        IModuleSettingsProvider CreateModuleSettingsProvider(int moduleId);
        IHostSettingsProvider CreateHostSettingsProvider();
        IStoreSettingsProvider CreateStoreSettingsProvider();

        ISearchProvider CreateSearchProvider(HccRequestContext hccRequestContext);

        HccDbContext CreateHccDbContext();
        IRepoStrategy<T> CreateStrategy<T>() where T : class, new();

        T CreateRepo<T>() where T : class, IRepo;
        T CreateRepo<T>(HccRequestContext context) where T : class, IRepo;

        T CreateService<T>() where T : HccServiceBase;
        T CreateService<T>(HccRequestContext context) where T : HccServiceBase;
    }
}