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

using System.Collections.Generic;

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    /// Defines resource lookup, formatting, JavaScript encoding, and dictionary access for a resource file.
    /// </summary>
    /// <remarks>
    /// Implementations own the resource and culture fallback behavior. Callers should use the
    /// culture overload when selecting a culture and <see cref="GetFormattedString(string, object[])"/>
    /// when supplying format arguments; these are separate contracts.
    /// </remarks>
    public interface ILocalizationHelper
    {
        /// <summary>
        /// Gets a resource value using the implementation's default culture behavior.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <returns>The resource value, or the implementation-defined fallback when it is unavailable.</returns>
        string GetString(string key);

        /// <summary>
        /// Gets a resource value for a specified culture.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <param name="culture">The culture name to use for the lookup.</param>
        /// <returns>The resource value, or the implementation-defined culture fallback when it is unavailable.</returns>
        string GetString(string key, string culture);

        /// <summary>
        /// Gets a resource value and applies the supplied format arguments.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <param name="args">The values used to format the resource value.</param>
        /// <returns>The formatted resource value, including the implementation's missing-resource behavior.</returns>
        string GetFormattedString(string key, params object[] args);

        /// <summary>
        /// Gets a resource value encoded for JavaScript use.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <returns>The encoded resource value.</returns>
        string GetJsEncodedString(string key);

        /// <summary>
        /// Gets a resource value encoded for JavaScript use with caller-supplied characters to encode.
        /// </summary>
        /// <param name="key">The resource key to look up.</param>
        /// <param name="characters">The characters supplied to the implementation's encoding routine.</param>
        /// <returns>The encoded resource value.</returns>
        string GetJsEncodedString(string key, string[] characters);

        /// <summary>
        /// Gets the resource values exposed by this helper as a key/value dictionary.
        /// </summary>
        /// <returns>A dictionary containing the resource entries returned by the implementation.</returns>
        Dictionary<string, string> GetResourceDictionary();
    }
}
