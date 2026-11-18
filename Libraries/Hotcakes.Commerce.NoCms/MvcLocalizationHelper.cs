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
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Commerce.NoCms
{
    /// <summary>
    /// Provides the MVC-host implementation of the HCC localization helper contract.
    /// </summary>
    /// <remarks>
    /// The adapter currently exposes the contract without implementing resource lookup; each
    /// operation therefore retains its existing <see cref="NotImplementedException"/> behavior.
    /// </remarks>
    public class MvcLocalizationHelper : ILocalizationHelper
    {
        /// <summary>
        /// Gets a resource value using the implementation's default culture behavior.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <returns>The resource value when implemented.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not implement resource lookup.</exception>
        public string GetString(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a resource value for a specified culture.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <param name="culture">The culture name to use for the lookup.</param>
        /// <returns>The culture-specific resource value when implemented.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not implement resource lookup.</exception>
        public string GetString(string key, string culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a resource value and applies the supplied format arguments.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <param name="args">The values used to format the resource value.</param>
        /// <returns>The formatted resource value when implemented.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not implement resource lookup.</exception>
        public string GetFormattedString(string key, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the resource values exposed by this helper as a dictionary.
        /// </summary>
        /// <returns>The resource dictionary when implemented.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not implement resource lookup.</exception>
        public Dictionary<string, string> GetResourceDictionary()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a resource value encoded for JavaScript use.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <returns>The JavaScript-encoded resource value when implemented.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not implement resource lookup.</exception>
        public string GetJsEncodedString(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a resource value encoded for JavaScript use with caller-supplied characters to encode.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <param name="characters">The characters supplied to the encoding routine.</param>
        /// <returns>The JavaScript-encoded resource value when implemented.</returns>
        /// <exception cref="NotImplementedException">Always thrown because this MVC adapter does not implement resource lookup.</exception>
        public string GetJsEncodedString(string key, string[] characters)
        {
            throw new NotImplementedException();
        }
    }
}
