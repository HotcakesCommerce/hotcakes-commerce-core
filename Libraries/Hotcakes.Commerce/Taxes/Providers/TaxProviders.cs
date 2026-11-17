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
using System.Linq;
using System.Reflection;
using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Commerce.Taxes.Providers
{
    /// <summary>
    /// Discovers, lists, and selects the tax provider instances available to the application.
    /// </summary>
    /// <remarks>
    /// Provider types are discovered once from the assemblies already loaded in the current
    /// <see cref="AppDomain"/>. Concrete <see cref="ITaxProvider"/> types are instantiated and ordered by
    /// <see cref="ITaxProvider.SortIndex"/>. The resulting provider instances and their
    /// <see cref="ITaxProvider.Basesettings"/> objects are shared by all callers; selecting a provider
    /// merges store settings into that shared object and does not create a per-store clone or clear keys.
    /// This preserves the existing behavior and means callers should treat returned instances and lists as
    /// compatibility-sensitive shared state.
    /// </remarks>
    public class TaxProviders
    {
        /// <summary>
        /// The fixed provider ID used by the Avalara tax provider.
        /// </summary>
        /// <remarks>
        /// The exact value is <c>A1C70FD1-DA71-42D1-A299-A0D3C4DAAF92</c>. It is persisted and referenced
        /// by existing installations and must not be changed, even though the containing type uses the
        /// historical spelling <c>Avatax</c>.
        /// </remarks>
        public const string AvataxServiceId = "A1C70FD1-DA71-42D1-A299-A0D3C4DAAF92";

        static TaxProviders()
        {
            if (Providers == null)
            {
                Providers = new List<ITaxProvider>();

                var type = typeof (ITaxProvider);
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var methodTypes = new List<Type>();
                foreach (var assembly in assemblies)
                {
                    var assemblyTypes = GetLoadableTypes(assembly).Where(t => type.IsAssignableFrom(t) && !t.IsAbstract);
                    methodTypes.AddRange(assemblyTypes);
                }

                foreach (var methodType in methodTypes)
                {
                    Providers.Add((ITaxProvider) Activator.CreateInstance(methodType));
                }
                Providers = Providers.OrderBy(g => g.SortIndex).ToList();
            }
        }

        private static List<ITaxProvider> Providers { get; set; }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        /// <summary>Gets all providers discovered during type initialization, ordered by sort index.</summary>
        /// <returns>The shared mutable provider list.</returns>
        /// <remarks>
        /// The returned list is the backing list itself, not a copy. Provider instances in it are shared
        /// with <see cref="CurrentTaxProvider"/> and <see cref="Find"/>.
        /// </remarks>
        public static List<ITaxProvider> AvailableProviders()
        {
            return Providers;
        }

        /// <summary>Gets the provider enabled in the specified store.</summary>
        /// <param name="store">The store whose <c>TaxProviderEnabled</c> setting identifies the provider.</param>
        /// <returns>The matching provider, or <see langword="null"/> when no provider is enabled or found.</returns>
        /// <remarks>
        /// The enabled ID comparison is case-sensitive. When a provider is found, its store settings are
        /// loaded with <c>TaxProviderSettingsGet</c> and merged into the provider's shared
        /// <see cref="ITaxProvider.Basesettings"/> object.
        /// </remarks>
        public static ITaxProvider CurrentTaxProvider(Store store)
        {
            ITaxProvider provider = null;
            var sett = store.Settings;

            var taxProviderId = sett.TaxProviderEnabled;
            if (!string.IsNullOrEmpty(taxProviderId))
            {
                provider = Providers.FirstOrDefault(p => taxProviderId == p.ProviderId);
            }

            if (provider != null)
            {
                var mSett = sett.TaxProviderSettingsGet(provider.ProviderId);
                provider.Basesettings.Merge(mSett);
            }

            return provider;
        }


        /// <summary>Finds a provider by ID and loads that provider's settings for the specified store.</summary>
        /// <param name="providerId">The provider ID to find; comparison is case-insensitive.</param>
        /// <param name="store">The store whose provider settings should be merged.</param>
        /// <returns>The matching provider, or <see langword="null"/> when no provider has the requested ID.</returns>
        /// <remarks>
        /// The matching provider instance is shared. Store settings are merged into its existing
        /// <see cref="ITaxProvider.Basesettings"/> object, so keys not present in the current store remain
        /// present from earlier state. This documents the existing behavior without changing it.
        /// </remarks>
        public static ITaxProvider Find(string providerId, Store store)
        {
            var sett = store.Settings;

            var provider =
                Providers.FirstOrDefault(
                    dpm => string.Equals(dpm.ProviderId, providerId, StringComparison.OrdinalIgnoreCase));
            if (provider != null)
            {
                var mSett = sett.TaxProviderSettingsGet(provider.ProviderId);
                provider.Basesettings.Merge(mSett);
            }
            return provider;
        }
    }
}
