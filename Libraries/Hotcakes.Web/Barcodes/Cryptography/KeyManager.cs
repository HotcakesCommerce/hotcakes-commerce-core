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

namespace Hotcakes.Web.Cryptography
{
    /// <summary>
    /// Provides access to encryption keys used by the cryptography helpers.
    /// </summary>
    [Serializable]
    public static class KeyManager
    {
        /// <summary>
        /// Generates a new 256-bit encryption key.
        /// </summary>
        /// <returns>A randomly generated key in hyphen-separated hexadecimal form.</returns>
        /// <see cref="KeyGenerator.Generate256bitKey()"/>
        /// <see cref="GetKey(long)"/>
        public static string GenerateKey()
        {
            return KeyGenerator.Generate256bitKey();
        }

        /// <summary>
        /// Gets the encryption key associated with an identifier.
        /// </summary>
        /// <param name="id">The identifier of the key to retrieve.</param>
        /// <returns>The configured encryption key in hyphen-separated hexadecimal form.</returns>
        /// <remarks>Key selection is currently not differentiated by <paramref name="id"/>.</remarks>
        /// <see cref="GenerateKey()"/>
        public static string GetKey(long id)
        {
            // TODO: Support Multiple keys and switching keys
            return "41-FD-07-13-47-B0-F3-6B-5A-8E-D1-10-6D-14-26-D7-23-9A-12-E9-24-F9-AC-B9-5F-11-54-F5-F1-9B-57-B0";
        }
    }
}
