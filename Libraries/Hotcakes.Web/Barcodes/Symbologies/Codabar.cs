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
using System.Collections;

namespace Hotcakes.Web.Barcodes
{
    [Serializable]
    internal class Codabar : BarcodeCommon, IBarcode
    {
        private readonly Hashtable Codabar_Code = new Hashtable(); //is initialized by init_Codabar()

        public Codabar(string input)
        {
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Codabar(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the Codabar algorithm.
        /// </summary>
        private string Encode_Codabar()
        {
            if (Raw_Data.Length < 2) throw new Exception("ECODABAR-1: Data format invalid. (Invalid length)");

            //check first char to make sure its a start/stop char
            switch (Raw_Data[0].ToString().ToUpper().Trim())
            {
                case "A":
                    break;
                case "B":
                    break;
                case "C":
                    break;
                case "D":
                    break;
                default:
                    throw new Exception("ECODABAR-2: Data format invalid. (Invalid START character)");
            }

            //check the ending char to make sure its a start/stop char
            switch (Raw_Data[Raw_Data.Trim().Length - 1].ToString().ToUpper().Trim())
            {
                case "A":
                    break;
                case "B":
                    break;
                case "C":
                    break;
                case "D":
                    break;
                default:
                    throw new Exception("ECODABAR-3: Data format invalid. (Invalid STOP character)");
            }

            var result = string.Empty;

            //populate the hashtable to begin the process
            init_Codabar();

            foreach (var c in Raw_Data)
            {
                result += Codabar_Code[c].ToString();
                result += "0"; //inter-character space
            }

            //remove the extra 0 at the end of the result
            result = result.Remove(result.Length - 1);

            //clears the hashtable so it no longer takes up memory
            Codabar_Code.Clear();

            return result;
        }

        private void init_Codabar()
        {
            Codabar_Code.Clear();
            Codabar_Code.Add('0', "101010011"); //"101001101101");
            Codabar_Code.Add('1', "101011001"); //"110100101011");
            Codabar_Code.Add('2', "101001011"); //"101100101011");
            Codabar_Code.Add('3', "110010101"); //"110110010101");
            Codabar_Code.Add('4', "101101001"); //"101001101011");
            Codabar_Code.Add('5', "110101001"); //"110100110101");
            Codabar_Code.Add('6', "100101011"); //"101100110101");
            Codabar_Code.Add('7', "100101101"); //"101001011011");
            Codabar_Code.Add('8', "100110101"); //"110100101101");
            Codabar_Code.Add('9', "110100101"); //"101100101101");
            Codabar_Code.Add('-', "101001101"); //"110101001011");
            Codabar_Code.Add('$', "101100101"); //"101101001011");
            Codabar_Code.Add(':', "1101011011"); //"110110100101");
            Codabar_Code.Add('/', "1101101011"); //"101011001011");
            Codabar_Code.Add('.', "1101101101"); //"110101100101");
            Codabar_Code.Add('+', "101100110011"); //"101101100101");
            Codabar_Code.Add('A', "1011001001"); //"110110100101");
            Codabar_Code.Add('B', "1010010011"); //"101011001011");
            Codabar_Code.Add('C', "1001001011"); //"110101100101");
            Codabar_Code.Add('D', "1010011001"); //"101101100101");
            Codabar_Code.Add('a', "1011001001"); //"110110100101");
            Codabar_Code.Add('b', "1010010011"); //"101011001011");
            Codabar_Code.Add('c', "1001001011"); //"110101100101");
            Codabar_Code.Add('d', "1010011001"); //"101101100101");
        }
    }
}