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
    internal class MSI : BarcodeCommon, IBarcode
    {
        private readonly TYPE Encoded_Type = TYPE.UNSPECIFIED;

        private readonly string[] MSI_Code =
        {
            "100100100100", "100100100110", "100100110100", "100100110110",
            "100110100100", "100110100110", "100110110100", "100110110110", "110100100100", "110100100110"
        };

        public MSI(string input, TYPE EncodedType)
        {
            Encoded_Type = EncodedType;
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_MSI(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the MSI algorithm.
        /// </summary>
        private string Encode_MSI()
        {
            //check for non-numeric chars
            if (!Barcode.CheckNumericOnly(Raw_Data))
                throw new Exception("EMSI-1: Numeric Data Only");

            var PreEncoded = Raw_Data;

            //get checksum
            if (Encoded_Type == TYPE.MSI_Mod10 || Encoded_Type == TYPE.MSI_2Mod10)
            {
                var odds = string.Empty;
                var evens = string.Empty;
                for (var i = PreEncoded.Length - 1; i >= 0; i -= 2)
                {
                    odds = PreEncoded[i] + odds;
                    if (i - 1 >= 0)
                        evens = PreEncoded[i - 1] + evens;
                }

                //multiply odds by 2
                odds = Convert.ToString(int.Parse(odds)*2);

                var evensum = 0;
                var oddsum = 0;
                foreach (var c in evens)
                    evensum += int.Parse(c.ToString());
                foreach (var c in odds)
                    oddsum += int.Parse(c.ToString());
                var checksum = 10 - (oddsum + evensum)%10;
                PreEncoded += checksum.ToString();
            }

            if (Encoded_Type == TYPE.MSI_Mod11 || Encoded_Type == TYPE.MSI_Mod11_Mod10)
            {
                var sum = 0;
                var weight = 2;
                for (var i = PreEncoded.Length - 1; i >= 0; i--)
                {
                    if (weight > 7) weight = 2;
                    sum += int.Parse(PreEncoded[i].ToString())*weight++;
                }
                var checksum = 11 - sum%11;

                PreEncoded += checksum.ToString();
            }

            if (Encoded_Type == TYPE.MSI_2Mod10 || Encoded_Type == TYPE.MSI_Mod11_Mod10)
            {
                //get second check digit if 2 mod 10 was selected or Mod11/Mod10
                var odds = string.Empty;
                var evens = string.Empty;
                for (var i = PreEncoded.Length - 1; i >= 0; i -= 2)
                {
                    odds = PreEncoded[i] + odds;
                    if (i - 1 >= 0)
                        evens = PreEncoded[i - 1] + evens;
                }

                //multiply odds by 2
                odds = Convert.ToString(int.Parse(odds)*2);

                var evensum = 0;
                var oddsum = 0;
                foreach (var c in evens)
                    evensum += int.Parse(c.ToString());
                foreach (var c in odds)
                    oddsum += int.Parse(c.ToString());
                var checksum = 10 - (oddsum + evensum)%10;
                PreEncoded += checksum.ToString();
            }

            var result = "110";
            foreach (var c in PreEncoded)
            {
                result += MSI_Code[int.Parse(c.ToString())];
            }

            //add stop character
            result += "1001";

            return result;
        }
    }
}