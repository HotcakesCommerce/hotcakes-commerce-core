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

namespace Hotcakes.Web.Barcodes
{
    [Serializable]
    internal class UPCSupplement5 : BarcodeCommon, IBarcode
    {
        private readonly string[] EAN_CodeA =
        {
            "0001101", "0011001", "0010011", "0111101", "0100011", "0110001",
            "0101111", "0111011", "0110111", "0001011"
        };

        private readonly string[] EAN_CodeB =
        {
            "0100111", "0110011", "0011011", "0100001", "0011101", "0111001",
            "0000101", "0010001", "0001001", "0010111"
        };

        private readonly string[] UPC_SUPP_5 =
        {
            "bbaaa", "babaa", "baaba", "baaab", "abbaa", "aabba", "aaabb", "ababa",
            "abaab", "aabab"
        };

        public UPCSupplement5(string input)
        {
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_UPCSupplemental_5(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the UPC Supplemental 5-digit algorithm.
        /// </summary>
        private string Encode_UPCSupplemental_5()
        {
            if (Raw_Data.Length != 5) throw new Exception("EUPC-SUP5-1: Invalid data length. (Length = 5 required).");

            if (!Barcode.CheckNumericOnly(Raw_Data))
                throw new Exception("EUPCA-2: Numeric Data Only");

            //calculate the checksum digit
            var even = 0;
            var odd = 0;

            //odd
            for (var i = 0; i <= 4; i += 2)
            {
                odd += int.Parse(Raw_Data.Substring(i, 1))*3;
            }

            //even
            for (var i = 1; i < 4; i += 2)
            {
                even += int.Parse(Raw_Data.Substring(i, 1))*9;
            }

            var total = even + odd;
            var cs = total%10;

            var pattern = UPC_SUPP_5[cs];

            var result = string.Empty;

            var pos = 0;
            foreach (var c in pattern)
            {
                //Inter-character separator
                if (pos == 0) result += "1011";
                else result += "01";

                if (c == 'a')
                {
                    //encode using odd parity
                    result += EAN_CodeA[int.Parse(Raw_Data[pos].ToString())];
                }
                else if (c == 'b')
                {
                    //encode using even parity
                    result += EAN_CodeB[int.Parse(Raw_Data[pos].ToString())];
                }
                pos++;
            }
            return result;
        }
    }
}