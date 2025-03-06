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
using System.Collections;

namespace Hotcakes.Web.Barcodes
{
    [Serializable]
    internal class Code39 : BarcodeCommon, IBarcode
    {
        private readonly bool _AllowExtended;
        private readonly Hashtable C39_Code = new Hashtable(); //is initialized by init_Code39()
        private readonly Hashtable ExtC39_Translation = new Hashtable();

        /// <summary>
        ///     Encodes with Code39.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        public Code39(string input)
        {
            Raw_Data = input;
        }

        /// <summary>
        ///     Encodes with Code39.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        /// <param name="AllowExtended">Allow Extended Code 39 (Full Ascii mode).</param>
        public Code39(string input, bool AllowExtended)
        {
            Raw_Data = input;
            _AllowExtended = AllowExtended;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Code39(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the Code 39 algorithm.
        /// </summary>
        private string Encode_Code39()
        {
            init_Code39();
            init_ExtendedCode39();

            var strFormattedData = "*" + Raw_Data.Replace("*", "") + "*";

            if (_AllowExtended)
                InsertExtendedCharsIfNeeded(ref strFormattedData);

            FormattedData = strFormattedData;

            var result = "";
            foreach (var c in FormattedData)
            {
                try
                {
                    result += C39_Code[c].ToString();
                    result += "0"; //whitespace
                }
                catch
                {
                    if (_AllowExtended)
                        throw new Exception("EC39-1: Invalid data.");
                    throw new Exception("EC39-1: Invalid data. (Try using Extended Code39)");
                }
            }

            result = result.Substring(0, result.Length - 1);

            //clear the hashtable so it no longer takes up memory
            C39_Code.Clear();

            return result;
        }

        private void init_Code39()
        {
            C39_Code.Clear();
            C39_Code.Add('0', "101001101101");
            C39_Code.Add('1', "110100101011");
            C39_Code.Add('2', "101100101011");
            C39_Code.Add('3', "110110010101");
            C39_Code.Add('4', "101001101011");
            C39_Code.Add('5', "110100110101");
            C39_Code.Add('6', "101100110101");
            C39_Code.Add('7', "101001011011");
            C39_Code.Add('8', "110100101101");
            C39_Code.Add('9', "101100101101");
            C39_Code.Add('A', "110101001011");
            C39_Code.Add('B', "101101001011");
            C39_Code.Add('C', "110110100101");
            C39_Code.Add('D', "101011001011");
            C39_Code.Add('E', "110101100101");
            C39_Code.Add('F', "101101100101");
            C39_Code.Add('G', "101010011011");
            C39_Code.Add('H', "110101001101");
            C39_Code.Add('I', "101101001101");
            C39_Code.Add('J', "101011001101");
            C39_Code.Add('K', "110101010011");
            C39_Code.Add('L', "101101010011");
            C39_Code.Add('M', "110110101001");
            C39_Code.Add('N', "101011010011");
            C39_Code.Add('O', "110101101001");
            C39_Code.Add('P', "101101101001");
            C39_Code.Add('Q', "101010110011");
            C39_Code.Add('R', "110101011001");
            C39_Code.Add('S', "101101011001");
            C39_Code.Add('T', "101011011001");
            C39_Code.Add('U', "110010101011");
            C39_Code.Add('V', "100110101011");
            C39_Code.Add('W', "110011010101");
            C39_Code.Add('X', "100101101011");
            C39_Code.Add('Y', "110010110101");
            C39_Code.Add('Z', "100110110101");
            C39_Code.Add('-', "100101011011");
            C39_Code.Add('.', "110010101101");
            C39_Code.Add(' ', "100110101101");
            C39_Code.Add('$', "100100100101");
            C39_Code.Add('/', "100100101001");
            C39_Code.Add('+', "100101001001");
            C39_Code.Add('%', "101001001001");
            C39_Code.Add('*', "100101101101");
        }

        private void init_ExtendedCode39()
        {
            ExtC39_Translation.Clear();
            ExtC39_Translation.Add(Convert.ToChar(0).ToString(), "%U");
            ExtC39_Translation.Add(Convert.ToChar(1).ToString(), "$A");
            ExtC39_Translation.Add(Convert.ToChar(2).ToString(), "$B");
            ExtC39_Translation.Add(Convert.ToChar(3).ToString(), "$C");
            ExtC39_Translation.Add(Convert.ToChar(4).ToString(), "$D");
            ExtC39_Translation.Add(Convert.ToChar(5).ToString(), "$E");
            ExtC39_Translation.Add(Convert.ToChar(6).ToString(), "$F");
            ExtC39_Translation.Add(Convert.ToChar(7).ToString(), "$G");
            ExtC39_Translation.Add(Convert.ToChar(8).ToString(), "$H");
            ExtC39_Translation.Add(Convert.ToChar(9).ToString(), "$I");
            ExtC39_Translation.Add(Convert.ToChar(10).ToString(), "$J");
            ExtC39_Translation.Add(Convert.ToChar(11).ToString(), "$K");
            ExtC39_Translation.Add(Convert.ToChar(12).ToString(), "$L");
            ExtC39_Translation.Add(Convert.ToChar(13).ToString(), "$M");
            ExtC39_Translation.Add(Convert.ToChar(14).ToString(), "$N");
            ExtC39_Translation.Add(Convert.ToChar(15).ToString(), "$O");
            ExtC39_Translation.Add(Convert.ToChar(16).ToString(), "$P");
            ExtC39_Translation.Add(Convert.ToChar(17).ToString(), "$Q");
            ExtC39_Translation.Add(Convert.ToChar(18).ToString(), "$R");
            ExtC39_Translation.Add(Convert.ToChar(19).ToString(), "$S");
            ExtC39_Translation.Add(Convert.ToChar(20).ToString(), "$T");
            ExtC39_Translation.Add(Convert.ToChar(21).ToString(), "$U");
            ExtC39_Translation.Add(Convert.ToChar(22).ToString(), "$V");
            ExtC39_Translation.Add(Convert.ToChar(23).ToString(), "$W");
            ExtC39_Translation.Add(Convert.ToChar(24).ToString(), "$X");
            ExtC39_Translation.Add(Convert.ToChar(25).ToString(), "$Y");
            ExtC39_Translation.Add(Convert.ToChar(26).ToString(), "$Z");
            ExtC39_Translation.Add(Convert.ToChar(27).ToString(), "%A");
            ExtC39_Translation.Add(Convert.ToChar(28).ToString(), "%B");
            ExtC39_Translation.Add(Convert.ToChar(29).ToString(), "%C");
            ExtC39_Translation.Add(Convert.ToChar(30).ToString(), "%D");
            ExtC39_Translation.Add(Convert.ToChar(31).ToString(), "%E");
            ExtC39_Translation.Add("!", "/A");
            ExtC39_Translation.Add("\"", "/B");
            ExtC39_Translation.Add("#", "/C");
            ExtC39_Translation.Add("$", "/D");
            ExtC39_Translation.Add("%", "/E");
            ExtC39_Translation.Add("&", "/F");
            ExtC39_Translation.Add("'", "/G");
            ExtC39_Translation.Add("(", "/H");
            ExtC39_Translation.Add(")", "/I");
            ExtC39_Translation.Add("*", "/J");
            ExtC39_Translation.Add("+", "/K");
            ExtC39_Translation.Add(",", "/L");
            ExtC39_Translation.Add("/", "/O");
            ExtC39_Translation.Add(":", "/Z");
            ExtC39_Translation.Add(";", "%F");
            ExtC39_Translation.Add("<", "%G");
            ExtC39_Translation.Add("=", "%H");
            ExtC39_Translation.Add(">", "%I");
            ExtC39_Translation.Add("?", "%J");
            ExtC39_Translation.Add("[", "%K");
            ExtC39_Translation.Add("\\", "%L");
            ExtC39_Translation.Add("]", "%M");
            ExtC39_Translation.Add("^", "%N");
            ExtC39_Translation.Add("_", "%O");
            ExtC39_Translation.Add("{", "%P");
            ExtC39_Translation.Add("|", "%Q");
            ExtC39_Translation.Add("}", "%R");
            ExtC39_Translation.Add("~", "%S");
            ExtC39_Translation.Add("`", "%W");
            ExtC39_Translation.Add("@", "%V");
            ExtC39_Translation.Add("a", "+A");
            ExtC39_Translation.Add("b", "+B");
            ExtC39_Translation.Add("c", "+C");
            ExtC39_Translation.Add("d", "+D");
            ExtC39_Translation.Add("e", "+E");
            ExtC39_Translation.Add("f", "+F");
            ExtC39_Translation.Add("g", "+G");
            ExtC39_Translation.Add("h", "+H");
            ExtC39_Translation.Add("i", "+I");
            ExtC39_Translation.Add("j", "+J");
            ExtC39_Translation.Add("k", "+K");
            ExtC39_Translation.Add("l", "+L");
            ExtC39_Translation.Add("m", "+M");
            ExtC39_Translation.Add("n", "+N");
            ExtC39_Translation.Add("o", "+O");
            ExtC39_Translation.Add("p", "+P");
            ExtC39_Translation.Add("q", "+Q");
            ExtC39_Translation.Add("r", "+R");
            ExtC39_Translation.Add("s", "+S");
            ExtC39_Translation.Add("t", "+T");
            ExtC39_Translation.Add("u", "+U");
            ExtC39_Translation.Add("v", "+V");
            ExtC39_Translation.Add("w", "+W");
            ExtC39_Translation.Add("x", "+X");
            ExtC39_Translation.Add("y", "+Y");
            ExtC39_Translation.Add("z", "+Z");
            ExtC39_Translation.Add(Convert.ToChar(127).ToString(), "%T"); //also %X, %Y, %Z 
        }

        private void InsertExtendedCharsIfNeeded(ref string FormattedData)
        {
            var output = string.Empty;
            foreach (var c in Raw_Data)
            {
                try
                {
                    var s = C39_Code[c].ToString();
                    output += c;
                }
                catch
                {
                    //insert extended substitution
                    var oTrans = ExtC39_Translation[c.ToString()];
                    output += oTrans.ToString();
                }
            }

            FormattedData = output;
        }
    }
}