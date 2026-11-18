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
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hotcakes.Commerce.Payment.Methods
{
    /// <summary>
    /// Provides the Ogone SHA-out parameter list and signature calculation
    /// utility.
    /// </summary>
    public static class OgoneUtils
    {
        /// <summary>
        /// Gets the mutable list of exact Ogone response parameter names used
        /// when constructing a SHA-out signature.
        /// </summary>
        /// <remarks>
        /// The list preserves the provider spelling, including underscores.
        /// The field is intentionally mutable for compatibility with existing
        /// integrations.
        /// </remarks>
        public static List<string> ShaOutParams = new List<string>
        {
            "AAVADDRESS",
            "AAVCHECK",
            "AAVMAIL",
            "AAVNAME",
            "AAVPHONE",
            "AAVZIP",
            "ACCEPTANCE",
            "ALIAS",
            "AMOUNT",
            "BIC",
            "BIN",
            "BRAND",
            "CARDNO",
            "CCCTY",
            "CN",
            "COMPLUS",
            "CREATION_STATUS",
            "CURRENCY",
            "CVCCHECK",
            "DCC_COMMPERCENTAGE",
            "DCC_CONVAMOUNT",
            "DCC_CONVCCY",
            "DCC_EXCHRATE",
            "DCC_EXCHRATESOURCE",
            "DCC_EXCHRATETS",
            "DCC_INDICATOR",
            "DCC_MARGINPERCENTAGE",
            "DCC_VALIDHOURS",
            "DIGESTCARDNO",
            "ECI",
            "ED",
            "ENCCARDNO",
            "FXAMOUNT",
            "FXCURRENCY",
            "IBAN",
            "IP",
            "IPCTY",
            "NBREMAILUSAGE",
            "NBRIPUSAGE",
            "NBRIPUSAGE_ALLTX",
            "NBRUSAGE",
            "NCERROR",
            "NCERRORCARDNO",
            "NCERRORCN",
            "NCERRORCVC",
            "NCERRORED",
            "ORDERID",
            "PAYID",
            "PM",
            "SCO_CATEGORY",
            "SCORING",
            "STATUS",
            "SUBBRAND",
            "SUBSCRIPTION_ID",
            "TRXDATE",
            "VC"
        };

        /// <summary>
        /// Calculates an Ogone SHA signature from the supplied parameters.
        /// </summary>
        /// <param name="parameters">The parameter collection to include in the signature.</param>
        /// <param name="hashAlgorithm">The SHA algorithm used to calculate the digest.</param>
        /// <param name="passPhrase">The pass phrase appended to every included parameter.</param>
        /// <returns>
        /// An uppercase hexadecimal digest. Parameters are upper-cased and
        /// sorted by key; entries whose values are null, empty, or whitespace
        /// are omitted.
        /// </returns>
        /// <remarks>
        /// The hash input is built by concatenating each included
        /// <c>KEY=value</c> pair followed immediately by <paramref name="passPhrase"/>,
        /// with no separators. The resulting string is encoded as UTF-8 before
        /// hashing. The selected algorithm is provided by
        /// <see cref="OgoneHashAlgorithm"/>.
        /// </remarks>
        public static string CalculateShaHash(NameValueCollection parameters, OgoneHashAlgorithm hashAlgorithm,
            string passPhrase)
        {
            var dict = parameters.AllKeys.ToDictionary(k => k.ToUpper(), k => parameters[k]);
            var sortedDict = new SortedDictionary<string, string>(dict);

            var stringForHashing = string.Empty;
            sortedDict.Where(p => !string.IsNullOrWhiteSpace(p.Value))
                .ToList()
                .ForEach(p => { stringForHashing += string.Concat(p.Key, "=", p.Value, passPhrase); });

            var encoding = Encoding.UTF8;
            var bytesToHash = encoding.GetBytes(stringForHashing);
            using (var sha = GetHashAlgorithm(hashAlgorithm))
            {
                var hash = sha.ComputeHash(bytesToHash);

                var hashString = BitConverter.ToString(hash);
                hashString = hashString.Replace("-", string.Empty);

                return hashString;
            }
        }

        private static HashAlgorithm GetHashAlgorithm(OgoneHashAlgorithm hashAlgorithm)
        {
            switch (hashAlgorithm)
            {
                case OgoneHashAlgorithm.Sha1:
                    return new SHA1Managed();
                case OgoneHashAlgorithm.Sha256:
                    return new SHA256Managed();
                case OgoneHashAlgorithm.Sha512:
                    return new SHA512Managed();
                default:
                    return null;
            }
        }
    }
}
