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
    internal class Code11 : BarcodeCommon, IBarcode
    {
        private readonly string[] C11_Code =
        {
            "101011", "1101011", "1001011", "1100101", "1011011", "1101101", "1001101",
            "1010011", "1101001", "110101", "101101", "1011001"
        };

        public Code11(string input)
        {
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Code11(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the Code 11 algorithm.
        /// </summary>
        private string Encode_Code11()
        {
            if (!Barcode.CheckNumericOnly(Raw_Data.Replace("-", string.Empty)))
                throw new Exception("EC11-1: Numeric data and '-' Only");

            //calculate the checksums
            var weight = 1;
            var CTotal = 0;
            var Data_To_Encode_with_Checksums = Raw_Data;

            //figure the C checksum
            for (var i = Raw_Data.Length - 1; i >= 0; i--)
            {
                //C checksum weights go 1-10
                if (weight == 10) weight = 1;

                if (Raw_Data[i] != '-')
                    CTotal += int.Parse(Raw_Data[i].ToString())*weight++;
                else
                    CTotal += 10*weight++;
            }
            var checksumC = CTotal%11;

            Data_To_Encode_with_Checksums += checksumC.ToString();

            //K checksums are recommended on any message length greater than or equal to 10
            if (Raw_Data.Length >= 1)
            {
                weight = 1;
                var KTotal = 0;

                //calculate K checksum
                for (var i = Data_To_Encode_with_Checksums.Length - 1; i >= 0; i--)
                {
                    //K checksum weights go 1-9
                    if (weight == 9) weight = 1;

                    if (Data_To_Encode_with_Checksums[i] != '-')
                        KTotal += int.Parse(Data_To_Encode_with_Checksums[i].ToString())*weight++;
                    else
                        KTotal += 10*weight++;
                }
                var checksumK = KTotal%11;
                Data_To_Encode_with_Checksums += checksumK.ToString();
            }

            //encode data
            var space = "0";
            var result = C11_Code[11] + space; //start-stop char + interchar space

            foreach (var c in Data_To_Encode_with_Checksums)
            {
                var index = c == '-' ? 10 : int.Parse(c.ToString());
                result += C11_Code[index];

                //inter-character space
                result += space;
            }

            //stop bars
            result += C11_Code[11];

            return result;
        }
    }
}