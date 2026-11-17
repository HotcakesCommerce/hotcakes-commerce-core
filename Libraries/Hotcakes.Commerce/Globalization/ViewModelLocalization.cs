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
    /// Provides non-caching initialization for static string properties on localized view-model types.
    /// </summary>
    /// <remarks>
    /// The resource path is formed from <see cref="HotcakesApplication.ViewsVirtualPath"/>
    /// and <c>/Views/App_LocalResources/{modelName}.resx</c>. Initialization reads the resource file
    /// on each call rather than caching values, which avoids cross-store contamination in multi-store setups.
    /// Errors for individual properties are swallowed so one property cannot break the caller.
    /// </remarks>
    public abstract class ViewModelLocalization
    {
        /// <summary>
        /// Populates writable public static string properties on <typeparamref name="T"/> from its view resource file.
        /// </summary>
        /// <typeparam name="T">The view-model type whose public static string properties are populated.</typeparam>
        /// <param name="modelName">The model name used to select <c>/Views/App_LocalResources/{modelName}.resx</c>.</param>
        /// <remarks>
        /// Null, unavailable, read-only, non-string, and empty resource values do not replace existing values.
        /// A blank model name throws <see cref="ArgumentException"/>. The method intentionally performs no
        /// caching and catches failures while setting individual properties.
        /// </remarks>
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