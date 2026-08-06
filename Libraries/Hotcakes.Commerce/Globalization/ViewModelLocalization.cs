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
using System.Reflection;

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    ///     Helper to populate static string properties on view-model localization classes from resource files.
    ///     The method is intentionally non-caching to avoid cross-store contamination in multi-store setups.
    /// </summary>
    public abstract class ViewModelLocalization
    {
        protected static void Init<T>(string modelName)
        {
            if (string.IsNullOrWhiteSpace(modelName)) throw new ArgumentException("modelName is required", nameof(modelName));

            var path = $"/Views/App_LocalResources/{modelName}.resx";
            var resourcePath = string.Concat(HotcakesApplication.Current.ViewsVirtualPath, path);
            var loc = Factory.Instance.CreateLocalizationHelper(resourcePath);
            if (loc == null) return;

            var props = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public);
            if (props == null || props.Length == 0) return;

            foreach (var prop in props)
            {
                if (prop.PropertyType != typeof(string)) continue;
                if (!prop.CanWrite) continue;

                try
                {
                    var value = loc.GetString(prop.Name);
                    if (!string.IsNullOrEmpty(value))
                    {
                        prop.SetValue(null, value, null);
                    }
                }
                catch
                {
                    // Swallow exceptions to avoid breaking callers when a single property fails.
                    // Logging may be added here if desired.
                }
            }
        }
    }
}