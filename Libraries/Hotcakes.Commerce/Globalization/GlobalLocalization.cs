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

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    /// Provides access to the application's global view resource file.
    /// </summary>
    /// <remarks>
    /// Each lookup creates a helper for the resource path formed from
    /// <see cref="HotcakesApplication.ViewsVirtualPath"/> and
    /// <c>/Views/App_LocalResources/GlobalResources.resx</c>. The path and per-call
    /// helper creation are compatibility-sensitive parts of the global lookup contract.
    /// </remarks>
    public static class GlobalLocalization
    {
        private static ILocalizationHelper Localization
        {
            get
            {
                var globalResourceFile = string.Concat(HotcakesApplication.Current.ViewsVirtualPath,
                    "/Views/App_LocalResources/GlobalResources.resx");
                return Factory.Instance.CreateLocalizationHelper(globalResourceFile);
            }
        }

        /// <summary>
        /// Gets a localized value from the global resource file using the helper's default culture behavior.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <returns>The localized resource value returned by <see cref="ILocalizationHelper.GetString(string)"/>.</returns>
        public static string GetString(string key)
        {
            return Localization.GetString(key);
        }

        /// <summary>
        /// Gets a localized value from the global resource file for a specified culture.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <param name="culture">The culture name passed to the localization helper.</param>
        /// <returns>The localized resource value returned by <see cref="ILocalizationHelper.GetString(string, string)"/>.</returns>
        public static string GetString(string key, string culture)
        {
            return Localization.GetString(key, culture);
        }

        /// <summary>
        /// Gets a formatted value from the global resource file.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <param name="args">The format arguments passed to the localization helper.</param>
        /// <returns>The formatted resource value returned by <see cref="ILocalizationHelper.GetFormattedString(string, object[])"/>.</returns>
        public static string GetFormattedString(string key, params object[] args)
        {
            return Localization.GetFormattedString(key, args);
        }
    }
}
