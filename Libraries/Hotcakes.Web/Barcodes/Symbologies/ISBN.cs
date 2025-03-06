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
    internal class ISBN : BarcodeCommon, IBarcode
    {
        public ISBN(string input)
        {
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_ISBN_Bookland(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the Bookland/ISBN algorithm.
        /// </summary>
        private string Encode_ISBN_Bookland()
        {
            if (!Barcode.CheckNumericOnly(Raw_Data))
                throw new Exception("EBOOKLANDISBN-1: Numeric Data Only");

            var type = "UNKNOWN";
            if (Raw_Data.Length == 10 || Raw_Data.Length == 9)
            {
                if (Raw_Data.Length == 10) Raw_Data = Raw_Data.Remove(9, 1);
                Raw_Data = "978" + Raw_Data;
                type = "ISBN";
            }
            else if (Raw_Data.Length == 12 && Raw_Data.StartsWith("978"))
            {
                type = "BOOKLAND-NOCHECKDIGIT";
            }
            else if (Raw_Data.Length == 13 && Raw_Data.StartsWith("978"))
            {
                type = "BOOKLAND-CHECKDIGIT";
                Raw_Data = Raw_Data.Remove(12, 1);
            }

            //check to see if its an unknown type
            if (type == "UNKNOWN")
                throw new Exception(
                    "EBOOKLANDISBN-2: Invalid input.  Must start with 978 and be length must be 9, 10, 12, 13 characters.");

            var ean13 = new EAN13(Raw_Data);
            return ean13.Encoded_Value;
        }
    }
}