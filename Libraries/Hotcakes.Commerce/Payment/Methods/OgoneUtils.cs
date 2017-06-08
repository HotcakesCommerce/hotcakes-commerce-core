#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
    public static class OgoneUtils
    {
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