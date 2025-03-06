#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    public class TaxProviders
    {
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

        public static List<ITaxProvider> AvailableProviders()
        {
            return Providers;
        }

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