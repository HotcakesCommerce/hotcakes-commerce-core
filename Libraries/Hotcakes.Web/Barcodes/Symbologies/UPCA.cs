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
using System.Collections;

namespace Hotcakes.Web.Barcodes
{
    [Serializable]
    internal class UPCA : BarcodeCommon, IBarcode
    {
        private string _Country_Assigning_Manufacturer_Code = "N/A";
        private readonly Hashtable CountryCodes = new Hashtable(); //is initialized by init_CountryCodes()

        private readonly string[] UPC_CodeA =
        {
            "0001101", "0011001", "0010011", "0111101", "0100011", "0110001",
            "0101111", "0111011", "0110111", "0001011"
        };

        private readonly string[] UPC_CodeB =
        {
            "0100111", "0110011", "0011011", "0100001", "0011101", "0111001",
            "0000101", "0010001", "0001001", "0010111"
        };

        private readonly string[] UPC_CodeC =
        {
            "1110010", "1100110", "1101100", "1000010", "1011100", "1001110",
            "1010000", "1000100", "1001000", "1110100"
        };

        private readonly string[] UPC_Pattern =
        {
            "aaaaaa", "aababb", "aabbab", "aabbba", "abaabb", "abbaab", "abbbaa",
            "ababab", "ababba", "abbaba"
        };

        public UPCA(string input)
        {
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_UPCA(); }
        }

        #endregion

        /// <summary>
        ///     Encode the raw data using the UPC-A algorithm.
        /// </summary>
        private string Encode_UPCA()
        {
            //check length of input
            if (Raw_Data.Length != 12)
                throw new Exception("EUPCA-1: Data length invalid. (Length must be 12)");

            if (!Barcode.CheckNumericOnly(Raw_Data))
                throw new Exception("EUPCA-2: Numeric Data Only");

            var result = "101"; //start with guard bars
            var patterncode = UPC_Pattern[int.Parse(Raw_Data[0].ToString())];

            //first number
            result += UPC_CodeA[int.Parse(Raw_Data[0].ToString())];

            //second (group) of numbers
            var pos = 0;
            while (pos < 5)
            {
                if (patterncode[pos + 1] == 'a')
                    result += UPC_CodeA[int.Parse(Raw_Data[pos + 1].ToString())];
                if (patterncode[pos + 1] == 'b')
                    result += UPC_CodeB[int.Parse(Raw_Data[pos + 1].ToString())];
                pos++;
            }

            //add divider bars
            result += "01010";

            //third (group) of numbers
            pos = 0;
            while (pos < 5)
            {
                result += UPC_CodeC[int.Parse(Raw_Data[pos++ + 6].ToString())];
            }

            //forth
            result += UPC_CodeC[int.Parse(Raw_Data[Raw_Data.Length - 1].ToString())];

            //add ending guard bars
            result += "101";

            //get the manufacturer assigning country
            init_CountryCodes();
            var twodigitCode = "0" + Raw_Data.Substring(0, 1);
            try
            {
                _Country_Assigning_Manufacturer_Code = CountryCodes[twodigitCode].ToString();
            }
            catch
            {
                throw new Exception("EUPCA-3: Country assigning manufacturer code not found.");
            }
            finally
            {
                CountryCodes.Clear();
            }

            return result;
        }

        private void init_CountryCodes()
        {
            CountryCodes.Clear();
            CountryCodes.Add("00", "US / CANADA");
            CountryCodes.Add("01", "US / CANADA");
            CountryCodes.Add("02", "US / CANADA");
            CountryCodes.Add("03", "US / CANADA");
            CountryCodes.Add("04", "US / CANADA");
            CountryCodes.Add("05", "US / CANADA");
            CountryCodes.Add("06", "US / CANADA");
            CountryCodes.Add("07", "US / CANADA");
            CountryCodes.Add("08", "US / CANADA");
            CountryCodes.Add("09", "US / CANADA");
            CountryCodes.Add("10", "US / CANADA");
            CountryCodes.Add("11", "US / CANADA");
            CountryCodes.Add("12", "US / CANADA");
            CountryCodes.Add("13", "US / CANADA");

            CountryCodes.Add("20", "IN STORE");
            CountryCodes.Add("21", "IN STORE");
            CountryCodes.Add("22", "IN STORE");
            CountryCodes.Add("23", "IN STORE");
            CountryCodes.Add("24", "IN STORE");
            CountryCodes.Add("25", "IN STORE");
            CountryCodes.Add("26", "IN STORE");
            CountryCodes.Add("27", "IN STORE");
            CountryCodes.Add("28", "IN STORE");
            CountryCodes.Add("29", "IN STORE");

            CountryCodes.Add("30", "FRANCE");
            CountryCodes.Add("31", "FRANCE");
            CountryCodes.Add("32", "FRANCE");
            CountryCodes.Add("33", "FRANCE");
            CountryCodes.Add("34", "FRANCE");
            CountryCodes.Add("35", "FRANCE");
            CountryCodes.Add("36", "FRANCE");
            CountryCodes.Add("37", "FRANCE");

            CountryCodes.Add("40", "GERMANY");
            CountryCodes.Add("41", "GERMANY");
            CountryCodes.Add("42", "GERMANY");
            CountryCodes.Add("43", "GERMANY");
            CountryCodes.Add("44", "GERMANY");

            CountryCodes.Add("45", "JAPAN");
            CountryCodes.Add("46", "RUSSIAN FEDERATION");
            CountryCodes.Add("49", "JAPAN (JAN-13)");

            CountryCodes.Add("50", "UNITED KINGDOM");
            CountryCodes.Add("54", "BELGIUM / LUXEMBOURG");
            CountryCodes.Add("57", "DENMARK");

            CountryCodes.Add("64", "FINLAND");

            CountryCodes.Add("70", "NORWAY");
            CountryCodes.Add("73", "SWEDEN");
            CountryCodes.Add("76", "SWITZERLAND");

            CountryCodes.Add("80", "ITALY");
            CountryCodes.Add("81", "ITALY");
            CountryCodes.Add("82", "ITALY");
            CountryCodes.Add("83", "ITALY");
            CountryCodes.Add("84", "SPAIN");
            CountryCodes.Add("87", "NETHERLANDS");

            CountryCodes.Add("90", "AUSTRIA");
            CountryCodes.Add("91", "AUSTRIA");
            CountryCodes.Add("93", "AUSTRALIA");
            CountryCodes.Add("94", "NEW ZEALAND");
            CountryCodes.Add("99", "COUPONS");

            CountryCodes.Add("471", "TAIWAN");
            CountryCodes.Add("474", "ESTONIA");
            CountryCodes.Add("475", "LATVIA");
            CountryCodes.Add("477", "LITHUANIA");
            CountryCodes.Add("479", "SRI LANKA");
            CountryCodes.Add("480", "PHILIPPINES");
            CountryCodes.Add("482", "UKRAINE");
            CountryCodes.Add("484", "MOLDOVA");
            CountryCodes.Add("485", "ARMENIA");
            CountryCodes.Add("486", "GEORGIA");
            CountryCodes.Add("487", "KAZAKHSTAN");
            CountryCodes.Add("489", "HONG KONG");

            CountryCodes.Add("520", "GREECE");
            CountryCodes.Add("528", "LEBANON");
            CountryCodes.Add("529", "CYPRUS");
            CountryCodes.Add("531", "MACEDONIA");
            CountryCodes.Add("535", "MALTA");
            CountryCodes.Add("539", "IRELAND");
            CountryCodes.Add("560", "PORTUGAL");
            CountryCodes.Add("569", "ICELAND");
            CountryCodes.Add("590", "POLAND");
            CountryCodes.Add("594", "ROMANIA");
            CountryCodes.Add("599", "HUNGARY");

            CountryCodes.Add("600", "SOUTH AFRICA");
            CountryCodes.Add("601", "SOUTH AFRICA");
            CountryCodes.Add("609", "MAURITIUS");
            CountryCodes.Add("611", "MOROCCO");
            CountryCodes.Add("613", "ALGERIA");
            CountryCodes.Add("619", "TUNISIA");
            CountryCodes.Add("622", "EGYPT");
            CountryCodes.Add("625", "JORDAN");
            CountryCodes.Add("626", "IRAN");
            CountryCodes.Add("690", "CHINA");
            CountryCodes.Add("691", "CHINA");
            CountryCodes.Add("692", "CHINA");

            CountryCodes.Add("729", "ISRAEL");
            CountryCodes.Add("740", "GUATEMALA");
            CountryCodes.Add("741", "EL SALVADOR");
            CountryCodes.Add("742", "HONDURAS");
            CountryCodes.Add("743", "NICARAGUA");
            CountryCodes.Add("744", "COSTA RICA");
            CountryCodes.Add("746", "DOMINICAN REPUBLIC");
            CountryCodes.Add("750", "MEXICO");
            CountryCodes.Add("759", "VENEZUELA");
            CountryCodes.Add("770", "COLOMBIA");
            CountryCodes.Add("773", "URUGUAY");
            CountryCodes.Add("775", "PERU");
            CountryCodes.Add("777", "BOLIVIA");
            CountryCodes.Add("779", "ARGENTINA");
            CountryCodes.Add("780", "CHILE");
            CountryCodes.Add("784", "PARAGUAY");
            CountryCodes.Add("785", "PERU");
            CountryCodes.Add("786", "ECUADOR");
            CountryCodes.Add("789", "BRAZIL");

            CountryCodes.Add("850", "CUBA");
            CountryCodes.Add("858", "SLOVAKIA");
            CountryCodes.Add("859", "CZECH REPUBLIC");
            CountryCodes.Add("860", "YUGLOSLAVIA");
            CountryCodes.Add("869", "TURKEY");
            CountryCodes.Add("880", "SOUTH KOREA");
            CountryCodes.Add("885", "THAILAND");
            CountryCodes.Add("888", "SINGAPORE");
            CountryCodes.Add("890", "INDIA");
            CountryCodes.Add("893", "VIETNAM");
            CountryCodes.Add("899", "INDONESIA");

            CountryCodes.Add("955", "MALAYSIA");
            CountryCodes.Add("977", "INTERNATIONAL STANDARD SERIAL NUMBER FOR PERIODICALS (ISSN)");
            CountryCodes.Add("978", "INTERNATIONAL STANDARD BOOK NUMBERING (ISBN)");
            CountryCodes.Add("979", "INTERNATIONAL STANDARD MUSIC NUMBER (ISMN)");
            CountryCodes.Add("980", "REFUND RECEIPTS");
            CountryCodes.Add("981", "COMMON CURRENCY COUPONS");
            CountryCodes.Add("982", "COMMON CURRENCY COUPONS");
        }
    }
}