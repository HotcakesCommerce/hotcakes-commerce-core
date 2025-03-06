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
    internal class Postnet : BarcodeCommon, IBarcode
    {
        private readonly string[] POSTNET_Code =
        {
            "11000", "00011", "00101", "00110", "01001", "01010", "01100", "10001",
            "10010", "10100"
        };

        public Postnet(string input)
        {
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Postnet(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the PostNet algorithm.
        /// </summary>
        private string Encode_Postnet()
        {
            //remove dashes if present
            Raw_Data = Raw_Data.Replace("-", "");

            switch (Raw_Data.Length)
            {
                case 5:
                case 6:
                case 9:
                case 11:
                    break;
                default:
                    throw new Exception("EPOSTNET-2: Invalid data length. (5, 6, 9, or 11 digits only)");
            }

            //Note: 0 = half bar and 1 = full bar
            //initialize the result with the starting bar
            var result = "1";
            var checkdigitsum = 0;

            foreach (var c in Raw_Data)
            {
                try
                {
                    var index = Convert.ToInt32(c.ToString());
                    result += POSTNET_Code[index];
                    checkdigitsum += index;
                }
                catch (Exception ex)
                {
                    throw new Exception("EPOSTNET-2: Invalid data. (Numeric only) --> " + ex.Message);
                }
            }

            //calculate and add check digit
            var temp = checkdigitsum%10;
            var checkdigit = 10 - (temp == 0 ? 10 : temp);

            result += POSTNET_Code[checkdigit];

            //ending bar
            result += "1";

            return result;
        }
    }
}