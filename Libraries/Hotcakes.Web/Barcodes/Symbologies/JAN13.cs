#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
    /// <summary>JAN-13 barcode encoder.</summary>
    /// <remarks>The encoded output is exposed through <see cref="IBarcode.Encoded_Value"/>.</remarks>
    [Serializable]
    internal class JAN13 : BarcodeCommon, IBarcode
    {
        /// <summary>Encodes data using the JAN-13 symbology.</summary>
        /// <param name="input">The data to encode.</param>
        /// <seealso cref="Encoded_Value"/>
        public JAN13(string input)
        {
            Raw_Data = input;
        }

        #region IBarcode Members

        /// <summary>Gets the encoded bar-and-space representation.</summary>
        /// <seealso cref="IBarcode.Encoded_Value"/>
        public string Encoded_Value
        {
            get { return Encode_JAN13(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the JAN-13 algorithm.
        /// </summary>
        private string Encode_JAN13()
        {
            if (!Raw_Data.StartsWith("49"))
                throw new Exception("EJAN13-1: Invalid Country Code for JAN13 (49 required)");
            if (!Barcode.CheckNumericOnly(Raw_Data))
                throw new Exception("EJAN13-2: Numeric Data Only");

            var ean13 = new EAN13(Raw_Data);
            return ean13.Encoded_Value;
        }
    }
}
