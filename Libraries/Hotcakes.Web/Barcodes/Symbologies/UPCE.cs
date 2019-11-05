#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
    internal class UPCE : BarcodeCommon, IBarcode
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

        private string[] EAN_CodeC =
        {
            "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000",
            "1000100", "1001000", "1110100"
        };

        private string[] EAN_Pattern =
        {
            "aaaaaa", "aababb", "aabbab", "aabbba", "abaabb", "abbaab", "abbbaa", "ababab",
            "ababba", "abbaba"
        };

        private readonly string[] UPCE_Code_0 =
        {
            "bbbaaa", "bbabaa", "bbaaba", "bbaaab", "babbaa", "baabba", "baaabb",
            "bababa", "babaab", "baabab"
        };

        private readonly string[] UPCE_Code_1 =
        {
            "aaabbb", "aababb", "aabbab", "aabbba", "abaabb", "abbaab", "abbbaa",
            "ababab", "ababba", "abbaba"
        };

        public UPCE(string input)
        {
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_UPCE(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the UPC-E algorithm.
        /// </summary>
        private string Encode_UPCE()
        {
            if (Raw_Data.Length != 6 && Raw_Data.Length != 8 && Raw_Data.Length != 12)
                throw new Exception("EUPCE-1: Invalid data length. (8 or 12 numbers only)");

            if (!Barcode.CheckNumericOnly(Raw_Data)) throw new Exception("EUPCE-2: Numeric only.");

            var CheckDigit = int.Parse(Raw_Data[Raw_Data.Length - 1].ToString());
            var NumberSystem = int.Parse(Raw_Data[0].ToString());

            //Convert to UPC-E from UPC-A if necessary
            if (Raw_Data.Length == 12)
            {
                var UPCECode = string.Empty;

                //break apart into components
                var Manufacturer = Raw_Data.Substring(1, 5);
                var ProductCode = Raw_Data.Substring(6, 5);

                //check for a valid number system
                if (NumberSystem != 0 && NumberSystem != 1)
                    throw new Exception("EUPCE-3: Invalid Number System (only 0 & 1 are valid)");

                if (Manufacturer.EndsWith("000") || Manufacturer.EndsWith("100") ||
                    Manufacturer.EndsWith("200") && int.Parse(ProductCode) <= 999)
                {
                    //rule 1
                    UPCECode += Manufacturer.Substring(0, 2); //first two of manufacturer
                    UPCECode += ProductCode.Substring(2, 3); //last three of product
                    UPCECode += Manufacturer[2].ToString(); //third of manufacturer
                }
                else if (Manufacturer.EndsWith("00") && int.Parse(ProductCode) <= 99)
                {
                    //rule 2
                    UPCECode += Manufacturer.Substring(0, 3); //first three of manufacturer
                    UPCECode += ProductCode.Substring(3, 2); //last two of product
                    UPCECode += "3"; //number 3
                }
                else if (Manufacturer.EndsWith("0") && int.Parse(ProductCode) <= 9)
                {
                    //rule 3
                    UPCECode += Manufacturer.Substring(0, 4); //first four of manufacturer
                    UPCECode += ProductCode[4]; //last digit of product
                    UPCECode += "4"; //number 4
                }
                else if (!Manufacturer.EndsWith("0") && int.Parse(ProductCode) <= 9 && int.Parse(ProductCode) >= 5)
                {
                    //rule 4
                    UPCECode += Manufacturer; //manufacturer
                    UPCECode += ProductCode[4]; //last digit of product
                }
                else
                    throw new Exception("EUPCE-4: Illegal UPC-A entered for conversion.  Unable to convert.");

                Raw_Data = UPCECode;
            }

            //get encoding pattern 
            var pattern = string.Empty;

            if (NumberSystem == 0) pattern = UPCE_Code_0[CheckDigit];
            else pattern = UPCE_Code_1[CheckDigit];

            //encode the data
            var result = "101";

            var pos = 0;
            foreach (var c in pattern)
            {
                var i = int.Parse(Raw_Data[pos++].ToString());
                if (c == 'a')
                {
                    result += EAN_CodeA[i];
                } //if
                else if (c == 'b')
                {
                    result += EAN_CodeB[i];
                }
            }

            //guard bars
            result += "01010";

            //end bars
            result += "1";

            return result;
        }
    }
}