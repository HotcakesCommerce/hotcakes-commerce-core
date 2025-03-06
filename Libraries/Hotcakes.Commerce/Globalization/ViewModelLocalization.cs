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

using System.Reflection;

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    ///     This strategy with calling Init method in static contructor won't work properly in multistore setup
    ///     We need simply prevent single initialization and don't use static field or static contructor to handle that
    ///     scenario.
    ///     Since for now this approach is used only to edit billing info for recurring orders so we skip fixing this for now
    /// </summary>
    public abstract class ViewModelLocalization
    {
        protected static void Init<T>(string modelName)
        {
            var path = string.Format("/Views/App_LocalResources/{0}.resx", modelName);
            var loc =
                Factory.Instance.CreateLocalizationHelper(string.Concat(HotcakesApplication.Current.ViewsVirtualPath,
                    path));
            var props = typeof (T).GetProperties(BindingFlags.Static | BindingFlags.Public);

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof (string))
                {
                    prop.SetValue(null, loc.GetString(prop.Name), null);
                }
            }
        }
    }
}