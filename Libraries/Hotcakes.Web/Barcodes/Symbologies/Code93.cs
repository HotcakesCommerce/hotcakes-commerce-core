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
using System.Data;

namespace Hotcakes.Web.Barcodes
{
    [Serializable]
    internal class Code93 : BarcodeCommon, IBarcode
    {
        private readonly DataTable C93_Code = new DataTable("C93_Code");

        /// <summary>
        ///     Encodes with Code93.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        public Code93(string input)
        {
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Code93(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the Code 93 algorithm.
        /// </summary>
        private string Encode_Code93()
        {
            init_Code93();

            var FormattedData = Add_CheckDigits(Raw_Data);

            var result = C93_Code.Select("Character = '*'")[0]["Encoding"].ToString();
            foreach (var c in FormattedData)
            {
                try
                {
                    result += C93_Code.Select("Character = '" + c + "'")[0]["Encoding"].ToString();
                }
                catch
                {
                    throw new Exception("EC93-1: Invalid data.");
                }
            }

            result += C93_Code.Select("Character = '*'")[0]["Encoding"].ToString();

            //termination bar
            result += "1";

            //clear the hashtable so it no longer takes up memory
            C93_Code.Clear();

            return result;
        }

        private void init_Code93()
        {
            C93_Code.Rows.Clear();
            C93_Code.Columns.Clear();
            C93_Code.Columns.Add("Value");
            C93_Code.Columns.Add("Character");
            C93_Code.Columns.Add("Encoding");
            C93_Code.Rows.Add("0", "0", "100010100");
            C93_Code.Rows.Add("1", "1", "101001000");
            C93_Code.Rows.Add("2", "2", "101000100");
            C93_Code.Rows.Add("3", "3", "101000010");
            C93_Code.Rows.Add("4", "4", "100101000");
            C93_Code.Rows.Add("5", "5", "100100100");
            C93_Code.Rows.Add("6", "6", "100100010");
            C93_Code.Rows.Add("7", "7", "101010000");
            C93_Code.Rows.Add("8", "8", "100010010");
            C93_Code.Rows.Add("9", "9", "100001010");
            C93_Code.Rows.Add("10", "A", "110101000");
            C93_Code.Rows.Add("11", "B", "110100100");
            C93_Code.Rows.Add("12", "C", "110100010");
            C93_Code.Rows.Add("13", "D", "110010100");
            C93_Code.Rows.Add("14", "E", "110010010");
            C93_Code.Rows.Add("15", "F", "110001010");
            C93_Code.Rows.Add("16", "G", "101101000");
            C93_Code.Rows.Add("17", "H", "101100100");
            C93_Code.Rows.Add("18", "I", "101100010");
            C93_Code.Rows.Add("19", "J", "100110100");
            C93_Code.Rows.Add("20", "K", "100011010");
            C93_Code.Rows.Add("21", "L", "101011000");
            C93_Code.Rows.Add("22", "M", "101001100");
            C93_Code.Rows.Add("23", "N", "101000110");
            C93_Code.Rows.Add("24", "O", "100101100");
            C93_Code.Rows.Add("25", "P", "100010110");
            C93_Code.Rows.Add("26", "Q", "110110100");
            C93_Code.Rows.Add("27", "R", "110110010");
            C93_Code.Rows.Add("28", "S", "110101100");
            C93_Code.Rows.Add("29", "T", "110100110");
            C93_Code.Rows.Add("30", "U", "110010110");
            C93_Code.Rows.Add("31", "V", "110011010");
            C93_Code.Rows.Add("32", "W", "101101100");
            C93_Code.Rows.Add("33", "X", "101100110");
            C93_Code.Rows.Add("34", "Y", "100110110");
            C93_Code.Rows.Add("35", "Z", "100111010");
            C93_Code.Rows.Add("36", "-", "100101110");
            C93_Code.Rows.Add("37", ".", "111010100");
            C93_Code.Rows.Add("38", " ", "111010010");
            C93_Code.Rows.Add("39", "$", "111001010");
            C93_Code.Rows.Add("40", "/", "101101110");
            C93_Code.Rows.Add("41", "+", "101110110");
            C93_Code.Rows.Add("42", "%", "110101110");
            C93_Code.Rows.Add("43", "(", "110101110"); //dont know what character actually goes here
            C93_Code.Rows.Add("44", ")", "110101110"); //dont know what character actually goes here
            C93_Code.Rows.Add("45", "#", "110101110"); //dont know what character actually goes here
            C93_Code.Rows.Add("46", "@", "110101110"); //dont know what character actually goes here
            C93_Code.Rows.Add("-", "*", "101011110");
        }

        private string Add_CheckDigits(string input)
        {
            //populate the C weights
            var aryCWeights = new int[input.Length];
            var curweight = 1;
            for (var i = input.Length - 1; i >= 0; i--)
            {
                if (curweight > 20)
                    curweight = 1;
                aryCWeights[i] = curweight;
                curweight++;
            }

            //populate the K weights
            var aryKWeights = new int[input.Length + 1];
            curweight = 1;
            for (var i = input.Length; i >= 0; i--)
            {
                if (curweight > 15)
                    curweight = 1;
                aryKWeights[i] = curweight;
                curweight++;
            }

            //calculate C checksum
            var SUM = 0;
            for (var i = 0; i < input.Length; i++)
            {
                SUM += aryCWeights[i]*
                       int.Parse(C93_Code.Select("Character = '" + input[i] + "'")[0]["Value"].ToString());
            }
            var ChecksumValue = SUM%47;

            input += C93_Code.Select("Value = '" + ChecksumValue + "'")[0]["Character"].ToString();

            //calculate K checksum
            SUM = 0;
            for (var i = 0; i < input.Length; i++)
            {
                SUM += aryKWeights[i]*
                       int.Parse(C93_Code.Select("Character = '" + input[i] + "'")[0]["Value"].ToString());
            }
            ChecksumValue = SUM%47;

            input += C93_Code.Select("Value = '" + ChecksumValue + "'")[0]["Character"].ToString();

            return input;
        }
    }
}