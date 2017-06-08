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
    internal class ITF14 : BarcodeCommon, IBarcode
    {
        private readonly string[] ITF14_Code =
        {
            "NNWWN", "WNNNW", "NWNNW", "WWNNN", "NNWNW", "WNWNN", "NWWNN", "NNNWW",
            "WNNWN", "NWNWN"
        };

        public ITF14(string input)
        {
            Raw_Data = input;

            CheckDigit();
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_ITF14(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the ITF-14 algorithm.
        /// </summary>
        private string Encode_ITF14()
        {
            //check length of input
            if (Raw_Data.Length > 14 || Raw_Data.Length < 13)
                throw new Exception("EITF14-1: Data length invalid. (Length must be 13 or 14)");

            if (!Barcode.CheckNumericOnly(Raw_Data))
                throw new Exception("EITF14-2: Numeric data only.");

            var result = "1010";

            for (var i = 0; i < Raw_Data.Length; i += 2)
            {
                var bars = true;
                var patternbars = ITF14_Code[int.Parse(Raw_Data[i].ToString())];
                var patternspaces = ITF14_Code[int.Parse(Raw_Data[i + 1].ToString())];
                var patternmixed = string.Empty;

                //interleave
                while (patternbars.Length > 0)
                {
                    patternmixed += patternbars[0] + patternspaces[0].ToString();
                    patternbars = patternbars.Substring(1);
                    patternspaces = patternspaces.Substring(1);
                }

                foreach (var c1 in patternmixed)
                {
                    if (bars)
                    {
                        if (c1 == 'N')
                            result += "1";
                        else
                            result += "11";
                    }
                    else
                    {
                        if (c1 == 'N')
                            result += "0";
                        else
                            result += "00";
                    }
                    bars = !bars;
                }
            }

            //add ending bars
            result += "1101";
            return result;
        }

        private void CheckDigit()
        {
            //calculate and include checksum if it is necessary
            if (Raw_Data.Length == 13)
            {
                var even = 0;
                var odd = 0;

                //odd
                for (var i = 0; i <= 10; i += 2)
                {
                    odd += int.Parse(Raw_Data.Substring(i, 1));
                }

                //even
                for (var i = 1; i <= 11; i += 2)
                {
                    even += int.Parse(Raw_Data.Substring(i, 1))*3;
                }

                var total = even + odd;
                var cs = total%10;
                cs = 10 - cs;
                if (cs == 10)
                    cs = 0;

                Raw_Data += cs.ToString();
            }
        }
    }
}