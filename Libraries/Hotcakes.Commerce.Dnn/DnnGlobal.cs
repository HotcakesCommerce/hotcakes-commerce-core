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
using System.Globalization;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;

namespace Hotcakes.Commerce.Dnn
{
    /// <summary>
    /// Provides access to the current DNN portal, portal settings, and localization context.
    /// </summary>
    public interface IDnnGlobal
    {
        /// <summary>
        /// Gets the identifier of the current portal, or <see cref="DotNetNuke.Common.Utilities.Null.NullInteger"/> when no portal is available.
        /// </summary>
        /// <returns>The current portal identifier.</returns>
        int GetPortalId();

        /// <summary>
        /// Gets the current <see cref="PortalInfo"/>.
        /// </summary>
        /// <returns>The current portal, or <see langword="null"/> when it cannot be resolved.</returns>
        PortalInfo GetCurrentPortal();

        /// <summary>
        /// Gets the current <see cref="PortalSettings"/>.
        /// </summary>
        /// <returns>The current portal settings, or <see langword="null"/> when no settings are available.</returns>
        PortalSettings GetCurrentPortalSettings();

        /// <summary>
        /// Gets the locales configured for the current portal.
        /// </summary>
        /// <returns>A dictionary keyed by culture code.</returns>
        Dictionary<string, Locale> GetLocales();

        /// <summary>
        /// Gets a locale for the current portal by culture code.
        /// </summary>
        /// <param name="code">The culture code to find.</param>
        /// <returns>The matching <see cref="Locale"/>, or <see langword="null"/> when no match exists.</returns>
        Locale GetLocale(string code);

        /// <summary>
        /// Gets the locale used to render the current page.
        /// </summary>
        /// <returns>The current page culture.</returns>
        CultureInfo GetPageLocale();
    }


    /// <summary>
    /// Provides the DNN portal and localization context used by Hotcakes Commerce.
    /// </summary>
    [Serializable]
    public class DnnGlobal : ServiceLocator<IDnnGlobal, DnnGlobal>
    {
        /// <summary>
        /// Sets the portal settings used by the current execution context.
        /// </summary>
        /// <param name="pSettings">The portal settings to use, or <see langword="null"/> to clear the explicit settings.</param>
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

            /// <summary>
            /// Gets the identifier of the current portal, or <see cref="DotNetNuke.Common.Utilities.Null.NullInteger"/> when no portal is available.
            /// </summary>
            /// <returns>The current portal identifier.</returns>
            public int GetPortalId()
            {
                var sett = GetCurrentPortalSettings();

                if (sett != null)
                    return sett.PortalId;
                return Null.NullInteger;
            }

            /// <summary>
            /// Gets the current <see cref="PortalInfo"/>.
            /// </summary>
            /// <returns>The current portal, or <see langword="null"/> when it cannot be resolved.</returns>
            public PortalInfo GetCurrentPortal()
            {
                var controller = new PortalController();
                return controller.GetPortal(GetPortalId());
            }

            /// <summary>
            /// Gets the current <see cref="PortalSettings"/>.
            /// </summary>
            /// <returns>The current portal settings, or <see langword="null"/> when none are available.</returns>
            public PortalSettings GetCurrentPortalSettings()
            {
                return Settings ?? PortalSettings.Current;
            }

            /// <summary>
            /// Gets the locales configured for the current portal.
            /// </summary>
            /// <returns>A dictionary keyed by culture code.</returns>
            public Dictionary<string, Locale> GetLocales()
            {
                return LocaleController.Instance.GetLocales(GetPortalId());
            }

            /// <summary>
            /// Gets a locale for the current portal by culture code.
            /// </summary>
            /// <param name="code">The culture code to find.</param>
            /// <returns>The matching <see cref="Locale"/>, or <see langword="null"/> when no match exists.</returns>
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

            /// <summary>
            /// Gets the locale used to render the current page.
            /// </summary>
            /// <returns>The current page culture.</returns>
            public CultureInfo GetPageLocale()
            {
                var portalSettings = Instance.GetCurrentPortalSettings();
                return Localization.GetPageLocale(portalSettings);
            }
        }
    }
}
