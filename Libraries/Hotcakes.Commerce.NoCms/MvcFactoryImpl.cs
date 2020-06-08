#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
    internal class MvcFactoryImpl : FactoryBase
    {
        public override WorkflowFactory CreateWorkflowFactory(HccRequestContext context)
        {
            return new WorkflowFactory();
        }

        public override IHccUrlResolver CreateHccUrlResolver()
        {
            return new MvcHccUrlResolver();
        }

        public override IHccFormRenderer CreateHccFormRenderer()
        {
            return new MvcHccFormRenderer();
        }

        public override ILogger CreateEventLogger()
        {
            return new MvcEventLog();
        }

        public override ISocialService CreateSocialService(HccRequestContext context)
        {
            throw new NotImplementedException();
        }

        public override ILocalizationHelper CreateLocalizationHelper(string resourceFile)
        {
            return new MvcLocalizationHelper();
        }

        public override PromotionFactory CreatePromotionFactory()
        {
            return new PromotionFactory();
        }


        public override IConfigurationManager CreateConfigurationManager()
        {
            throw new NotImplementedException();
        }

        public override IModuleSettingsProvider CreateModuleSettingsProvider(int moduleId)
        {
            throw new NotImplementedException();
        }

        public override IHostSettingsProvider CreateHostSettingsProvider()
        {
            throw new NotImplementedException();
        }

        public override IStoreSettingsProvider CreateStoreSettingsProvider()
        {
            throw new NotImplementedException();
        }

        public override ISearchProvider CreateSearchProvider(HccRequestContext hccRequestContext)
        {
            throw new NotImplementedException();
        }
    }
}