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
using System.Collections.Generic;
using System.Globalization;
using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;

namespace Hotcakes.Commerce.Dnn
{
    public interface IDnnGlobal
    {
        int GetPortalId();
        PortalInfo GetCurrentPortal();
        PortalSettings GetCurrentPortalSettings();
        Dictionary<string, Locale> GetLocales();
        Locale GetLocale(string code);
        CultureInfo GetPageLocale();
    }


    [Serializable]
    public class DnnGlobal : ServiceLocator<IDnnGlobal, DnnGlobal>
    {
        public static void SetPortalSettings(PortalSettings pSettings)
        {
            DnnGlobalImpl.Settings = pSettings;
        }

        protected override Func<IDnnGlobal> GetFactory()
        {
            return () => new DnnGlobalImpl();
        }

        internal class DnnGlobalImpl : IDnnGlobal
        {
            [ThreadStatic] private static PortalSettings _settings;

            internal static PortalSettings Settings
            {
                get { return _settings; }
                set { _settings = value; }
            }

            public int GetPortalId()
            {
                var sett = GetCurrentPortalSettings();

                if (sett != null)
                    return sett.PortalId;
                return Null.NullInteger;
            }

            public PortalInfo GetCurrentPortal()
            {
                var controller = new PortalController();
                return controller.GetPortal(GetPortalId());
            }

            public PortalSettings GetCurrentPortalSettings()
            {
                return Settings ?? PortalSettings.Current;
            }

            public Dictionary<string, Locale> GetLocales()
            {
                return LocaleController.Instance.GetLocales(GetPortalId());
            }

            public Locale GetLocale(string code)
            {
                var locale = LocaleController.Instance.GetLocale(GetPortalId(), code);
                if (locale == null)
                {
                    // workaround to explicitly fill and retrieve the locales under some conditions
                    // under these (unknown) conditions, the CMS API doesn't aways fill the locale collection
                    var locales = CBO.FillDictionary("CultureCode", DataProvider.Instance().GetLanguages(), new Dictionary<string, Locale>(StringComparer.OrdinalIgnoreCase));
                    locales.TryGetValue(code, out locale);
                }
                return locale;
            }

            public CultureInfo GetPageLocale()
            {
                var portalSettings = Instance.GetCurrentPortalSettings();
                return Localization.GetPageLocale((IPortalSettings)portalSettings);
            }
        }
    }
}