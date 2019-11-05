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
    internal class EAN8 : BarcodeCommon, IBarcode
    {
        private readonly string[] EAN_CodeA =
        {
            "0001101", "0011001", "0010011", "0111101", "0100011", "0110001",
            "0101111", "0111011", "0110111", "0001011"
        };

        private readonly string[] EAN_CodeC =
        {
            "1110010", "1100110", "1101100", "1000010", "1011100", "1001110",
            "1010000", "1000100", "1001000", "1110100"
        };

        public EAN8(string input)
        {
            Raw_Data = input;

            CheckDigit();
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_EAN8(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the EAN-8 algorithm.
        /// </summary>
        private string Encode_EAN8()
        {
            //check length
            if (Raw_Data.Length != 8 && Raw_Data.Length != 7)
                throw new Exception("EEAN8-1: Invalid data length. (7 or 8 numbers only)");

            //check numeric only
            if (!Barcode.CheckNumericOnly(Raw_Data)) throw new Exception("EEAN8-2: Numeric only.");

            //encode the data
            var result = "101";

            //first half (Encoded using left hand / odd parity)
            for (var i = 0; i < Raw_Data.Length/2; i++)
            {
                result += EAN_CodeA[int.Parse(Raw_Data[i].ToString())];
            }

            //center guard bars
            result += "01010";

            //second half (Encoded using right hand / even parity)
            for (var i = Raw_Data.Length/2; i < Raw_Data.Length; i++)
            {
                result += EAN_CodeC[int.Parse(Raw_Data[i].ToString())];
            }

            result += "101";

            return result;
        }

        private void CheckDigit()
        {
            //calculate the checksum digit if necessary
            if (Raw_Data.Length == 7)
            {
                //calculate the checksum digit
                var even = 0;
                var odd = 0;

                //odd
                for (var i = 0; i <= 6; i += 2)
                {
                    odd += int.Parse(Raw_Data.Substring(i, 1))*3;
                }

                //even
                for (var i = 1; i <= 5; i += 2)
                {
                    even += int.Parse(Raw_Data.Substring(i, 1));
                }

                var total = even + odd;
                var checksum = total%10;
                checksum = 10 - checksum;
                if (checksum == 10)
                    checksum = 0;

                //add the checksum to the end of the 
                Raw_Data += checksum.ToString();
            }
        }
    }
}