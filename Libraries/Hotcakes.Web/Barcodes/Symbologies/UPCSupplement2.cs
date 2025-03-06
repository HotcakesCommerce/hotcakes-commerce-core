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

using System;

namespace Hotcakes.Web.Barcodes
{
    [Serializable]
    internal class UPCSupplement2 : BarcodeCommon, IBarcode
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

        private readonly string[] UPC_SUPP_2 = {"aa", "ab", "ba", "bb"};

        public UPCSupplement2(string input)
        {
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_UPCSupplemental_2(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the UPC Supplemental 2-digit algorithm.
        /// </summary>
        private string Encode_UPCSupplemental_2()
        {
            if (Raw_Data.Length != 2) throw new Exception("EUPC-SUP2-1: Invalid data length. (Length = 2 required)");

            if (!Barcode.CheckNumericOnly(Raw_Data))
                throw new Exception("EUPC-SUP2-2: Numeric Data Only");

            var pattern = string.Empty;

            try
            {
                pattern = UPC_SUPP_2[int.Parse(Raw_Data.Trim())%4];
            }
            catch
            {
                throw new Exception("EUPC-SUP2-3: Invalid Data. (Numeric only)");
            }

            var result = "1011";

            var pos = 0;
            foreach (var c in pattern)
            {
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

                if (pos++ == 0) result += "01"; //Inter-character separator
            }
            return result;
        }
    }
}