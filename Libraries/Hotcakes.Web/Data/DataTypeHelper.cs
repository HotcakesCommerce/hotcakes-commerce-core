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

namespace Hotcakes.Web.Data
{
    /// <summary>
    /// Provides conversions between GUID values and bvin strings.
    /// </summary>
    [Serializable]
    public static class DataTypeHelper
    {
        /// <summary>
        /// Converts a nullable GUID to its lowercase, hyphenated bvin representation.
        /// </summary>
        /// <param name="guid">The GUID to convert.</param>
        /// <returns>The bvin string, or <see cref="string.Empty"/> when <paramref name="guid"/> has no value.</returns>
        public static string GuidToBvin(Guid? guid)
        {
            if (!guid.HasValue)
                return string.Empty;
            return guid.Value.ToString("D").ToLowerInvariant();
        }

        /// <summary>
        /// Converts a bvin string to a GUID.
        /// </summary>
        /// <param name="bvin">The bvin string.</param>
        /// <returns>The parsed GUID, or <see cref="Guid.Empty"/> when <paramref name="bvin"/> is null or empty.</returns>
        /// <exception cref="FormatException">The bvin is not a valid GUID representation.</exception>
        public static Guid BvinToGuid(string bvin)
        {
            if (!string.IsNullOrEmpty(bvin))
                return new Guid(bvin);
            return Guid.Empty;
        }

        /// <summary>
        /// Converts a bvin string to a nullable GUID.
        /// </summary>
        /// <param name="bvin">The bvin string.</param>
        /// <returns>The parsed GUID, or <see langword="null"/> when <paramref name="bvin"/> is null or empty.</returns>
        /// <exception cref="FormatException">The bvin is not a valid GUID representation.</exception>
        public static Guid? BvinToNullableGuid(string bvin)
        {
            if (string.IsNullOrEmpty(bvin))
                return null;
            return new Guid(bvin);
        }
    }
}
