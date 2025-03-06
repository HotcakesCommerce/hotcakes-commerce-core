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
using System.Collections.Generic;
using System.Data;

namespace Hotcakes.Web.Barcodes
{
    [Serializable]
    internal class Code128 : BarcodeCommon, IBarcode
    {
        public enum TYPES
        {
            DYNAMIC,
            A,
            B,
            C
        }

        private readonly List<string> _EncodedData = new List<string>();
        private readonly List<string> _FormattedData = new List<string>();
        private readonly DataTable C128_Code = new DataTable("C128");
        private DataRow StartCharacter;
        private readonly TYPES type = TYPES.DYNAMIC;

        /// <summary>
        ///     Encodes data in Code128 format.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        public Code128(string input)
        {
            Raw_Data = input;
        }

        /// <summary>
        ///     Encodes data in Code128 format.
        /// </summary>
        /// <param name="input">Data to encode.</param>
        /// <param name="type">Type of encoding to lock to. (Code 128A, Code 128B, Code 128C)</param>
        public Code128(string input, TYPES type)
        {
            this.type = type;
            Raw_Data = input;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Code128(); }
        }

        #endregion

        private string Encode_Code128()
        {
            //initialize datastructure to hold encoding information
            init_Code128();

            return GetEncoding();
        }

        private void init_Code128()
        {
            //set the table to case sensitive since there are upper and lower case values
            C128_Code.CaseSensitive = true;

            //set up columns
            C128_Code.Columns.Add("Value", typeof (string));
            C128_Code.Columns.Add("A", typeof (string));
            C128_Code.Columns.Add("B", typeof (string));
            C128_Code.Columns.Add("C", typeof (string));
            C128_Code.Columns.Add("Encoding", typeof (string));

            //populate data
            C128_Code.Rows.Add("0", " ", " ", "00", "11011001100");
            C128_Code.Rows.Add("1", "!", "!", "01", "11001101100");
            C128_Code.Rows.Add("2", "\"", "\"", "02", "11001100110");
            C128_Code.Rows.Add("3", "#", "#", "03", "10010011000");
            C128_Code.Rows.Add("4", "$", "$", "04", "10010001100");
            C128_Code.Rows.Add("5", "%", "%", "05", "10001001100");
            C128_Code.Rows.Add("6", "&", "&", "06", "10011001000");
            C128_Code.Rows.Add("7", "'", "'", "07", "10011000100");
            C128_Code.Rows.Add("8", "(", "(", "08", "10001100100");
            C128_Code.Rows.Add("9", ")", ")", "09", "11001001000");
            C128_Code.Rows.Add("10", "*", "*", "10", "11001000100");
            C128_Code.Rows.Add("11", "+", "+", "11", "11000100100");
            C128_Code.Rows.Add("12", ",", ",", "12", "10110011100");
            C128_Code.Rows.Add("13", "-", "-", "13", "10011011100");
            C128_Code.Rows.Add("14", ".", ".", "14", "10011001110");
            C128_Code.Rows.Add("15", "/", "/", "15", "10111001100");
            C128_Code.Rows.Add("16", "0", "0", "16", "10011101100");
            C128_Code.Rows.Add("17", "1", "1", "17", "10011100110");
            C128_Code.Rows.Add("18", "2", "2", "18", "11001110010");
            C128_Code.Rows.Add("19", "3", "3", "19", "11001011100");
            C128_Code.Rows.Add("20", "4", "4", "20", "11001001110");
            C128_Code.Rows.Add("21", "5", "5", "21", "11011100100");
            C128_Code.Rows.Add("22", "6", "6", "22", "11001110100");
            C128_Code.Rows.Add("23", "7", "7", "23", "11101101110");
            C128_Code.Rows.Add("24", "8", "8", "24", "11101001100");
            C128_Code.Rows.Add("25", "9", "9", "25", "11100101100");
            C128_Code.Rows.Add("26", ":", ":", "26", "11100100110");
            C128_Code.Rows.Add("27", ";", ";", "27", "11101100100");
            C128_Code.Rows.Add("28", "<", "<", "28", "11100110100");
            C128_Code.Rows.Add("29", "=", "=", "29", "11100110010");
            C128_Code.Rows.Add("30", ">", ">", "30", "11011011000");
            C128_Code.Rows.Add("31", "?", "?", "31", "11011000110");
            C128_Code.Rows.Add("32", "@", "@", "32", "11000110110");
            C128_Code.Rows.Add("33", "A", "A", "33", "10100011000");
            C128_Code.Rows.Add("34", "B", "B", "34", "10001011000");
            C128_Code.Rows.Add("35", "C", "C", "35", "10001000110");
            C128_Code.Rows.Add("36", "D", "D", "36", "10110001000");
            C128_Code.Rows.Add("37", "E", "E", "37", "10001101000");
            C128_Code.Rows.Add("38", "F", "F", "38", "10001100010");
            C128_Code.Rows.Add("39", "G", "G", "39", "11010001000");
            C128_Code.Rows.Add("40", "H", "H", "40", "11000101000");
            C128_Code.Rows.Add("41", "I", "I", "41", "11000100010");
            C128_Code.Rows.Add("42", "J", "J", "42", "10110111000");
            C128_Code.Rows.Add("43", "K", "K", "43", "10110001110");
            C128_Code.Rows.Add("44", "L", "L", "44", "10001101110");
            C128_Code.Rows.Add("45", "M", "M", "45", "10111011000");
            C128_Code.Rows.Add("46", "N", "N", "46", "10111000110");
            C128_Code.Rows.Add("47", "O", "O", "47", "10001110110");
            C128_Code.Rows.Add("48", "P", "P", "48", "11101110110");
            C128_Code.Rows.Add("49", "Q", "Q", "49", "11010001110");
            C128_Code.Rows.Add("50", "R", "R", "50", "11000101110");
            C128_Code.Rows.Add("51", "S", "S", "51", "11011101000");
            C128_Code.Rows.Add("52", "T", "T", "52", "11011100010");
            C128_Code.Rows.Add("53", "U", "U", "53", "11011101110");
            C128_Code.Rows.Add("54", "V", "V", "54", "11101011000");
            C128_Code.Rows.Add("55", "W", "W", "55", "11101000110");
            C128_Code.Rows.Add("56", "X", "X", "56", "11100010110");
            C128_Code.Rows.Add("57", "Y", "Y", "57", "11101101000");
            C128_Code.Rows.Add("58", "Z", "Z", "58", "11101100010");
            C128_Code.Rows.Add("59", "[", "[", "59", "11100011010");
            C128_Code.Rows.Add("60", @"\", @"\", "60", "11101111010");
            C128_Code.Rows.Add("61", "]", "]", "61", "11001000010");
            C128_Code.Rows.Add("62", "^", "^", "62", "11110001010");
            C128_Code.Rows.Add("63", "_", "_", "63", "10100110000");
            C128_Code.Rows.Add("64", "\0", "`", "64", "10100001100");
            C128_Code.Rows.Add("65", Convert.ToChar(1).ToString(), "a", "65", "10010110000");
            C128_Code.Rows.Add("66", Convert.ToChar(2).ToString(), "b", "66", "10010000110");
            C128_Code.Rows.Add("67", Convert.ToChar(3).ToString(), "c", "67", "10000101100");
            C128_Code.Rows.Add("68", Convert.ToChar(4).ToString(), "d", "68", "10000100110");
            C128_Code.Rows.Add("69", Convert.ToChar(5).ToString(), "e", "69", "10110010000");
            C128_Code.Rows.Add("70", Convert.ToChar(6).ToString(), "f", "70", "10110000100");
            C128_Code.Rows.Add("71", Convert.ToChar(7).ToString(), "g", "71", "10011010000");
            C128_Code.Rows.Add("72", Convert.ToChar(8).ToString(), "h", "72", "10011000010");
            C128_Code.Rows.Add("73", Convert.ToChar(9).ToString(), "i", "73", "10000110100");
            C128_Code.Rows.Add("74", Convert.ToChar(10).ToString(), "j", "74", "10000110010");
            C128_Code.Rows.Add("75", Convert.ToChar(11).ToString(), "k", "75", "11000010010");
            C128_Code.Rows.Add("76", Convert.ToChar(12).ToString(), "l", "76", "11001010000");
            C128_Code.Rows.Add("77", Convert.ToChar(13).ToString(), "m", "77", "11110111010");
            C128_Code.Rows.Add("78", Convert.ToChar(14).ToString(), "n", "78", "11000010100");
            C128_Code.Rows.Add("79", Convert.ToChar(15).ToString(), "o", "79", "10001111010");
            C128_Code.Rows.Add("80", Convert.ToChar(16).ToString(), "p", "80", "10100111100");
            C128_Code.Rows.Add("81", Convert.ToChar(17).ToString(), "q", "81", "10010111100");
            C128_Code.Rows.Add("82", Convert.ToChar(18).ToString(), "r", "82", "10010011110");
            C128_Code.Rows.Add("83", Convert.ToChar(19).ToString(), "s", "83", "10111100100");
            C128_Code.Rows.Add("84", Convert.ToChar(20).ToString(), "t", "84", "10011110100");
            C128_Code.Rows.Add("85", Convert.ToChar(21).ToString(), "u", "85", "10011110010");
            C128_Code.Rows.Add("86", Convert.ToChar(22).ToString(), "v", "86", "11110100100");
            C128_Code.Rows.Add("87", Convert.ToChar(23).ToString(), "w", "87", "11110010100");
            C128_Code.Rows.Add("88", Convert.ToChar(24).ToString(), "x", "88", "11110010010");
            C128_Code.Rows.Add("89", Convert.ToChar(25).ToString(), "y", "89", "11011011110");
            C128_Code.Rows.Add("90", Convert.ToChar(26).ToString(), "z", "90", "11011110110");
            C128_Code.Rows.Add("91", Convert.ToChar(27).ToString(), "{", "91", "11110110110");
            C128_Code.Rows.Add("92", Convert.ToChar(28).ToString(), "|", "92", "10101111000");
            C128_Code.Rows.Add("93", Convert.ToChar(29).ToString(), "}", "93", "10100011110");
            C128_Code.Rows.Add("94", Convert.ToChar(30).ToString(), "~", "94", "10001011110");

            C128_Code.Rows.Add("95", Convert.ToChar(31).ToString(), Convert.ToChar(127).ToString(), "95", "10111101000");
            C128_Code.Rows.Add("96", Convert.ToChar(202).ToString(), Convert.ToChar(202).ToString(), "96", "10111100010");
            C128_Code.Rows.Add("97", Convert.ToChar(201).ToString(), Convert.ToChar(201).ToString(), "97", "11110101000");
            C128_Code.Rows.Add("98", "SHIFT", "SHIFT", "98", "11110100010");
            C128_Code.Rows.Add("99", "CODE_C", "CODE_C", "99", "10111011110");
            C128_Code.Rows.Add("100", "CODE_B", Convert.ToChar(203).ToString(), "CODE_B", "10111101110");
            C128_Code.Rows.Add("101", Convert.ToChar(203).ToString(), "CODE_A", "CODE_A", "11101011110");
            C128_Code.Rows.Add("102", Convert.ToChar(200).ToString(), Convert.ToChar(200).ToString(),
                Convert.ToChar(200).ToString(), "11110101110");
            C128_Code.Rows.Add("103", "START_A", "START_A", "START_A", "11010000100");
            C128_Code.Rows.Add("104", "START_B", "START_B", "START_B", "11010010000");
            C128_Code.Rows.Add("105", "START_C", "START_C", "START_C", "11010011100");
            C128_Code.Rows.Add("", "STOP", "STOP", "STOP", "11000111010");
        }

        private List<DataRow> FindStartorCodeCharacter(string s, ref int col)
        {
            var rows = new List<DataRow>();

            //if two chars are numbers then START_C or CODE_C
            if (s.Length > 1 && char.IsNumber(s[0]) && char.IsNumber(s[1]))
            {
                if (StartCharacter == null)
                {
                    StartCharacter = C128_Code.Select("A = 'START_C'")[0];
                    rows.Add(StartCharacter);
                }
                else
                    rows.Add(C128_Code.Select("A = 'CODE_C'")[0]);

                col = 1;
            }
            else
            {
                var AFound = false;
                var BFound = false;
                foreach (DataRow row in C128_Code.Rows)
                {
                    try
                    {
                        if (!AFound && s == row["A"].ToString())
                        {
                            AFound = true;
                            col = 2;

                            if (StartCharacter == null)
                            {
                                StartCharacter = C128_Code.Select("A = 'START_A'")[0];
                                rows.Add(StartCharacter);
                            }
                            else
                            {
                                rows.Add(C128_Code.Select("B = 'CODE_A'")[0]); //first column is FNC4 so use B
                            }
                        }
                        else if (!BFound && s == row["B"].ToString())
                        {
                            BFound = true;
                            col = 1;

                            if (StartCharacter == null)
                            {
                                StartCharacter = C128_Code.Select("A = 'START_B'")[0];
                                rows.Add(StartCharacter);
                            }
                            else
                                rows.Add(C128_Code.Select("A = 'CODE_B'")[0]);
                        }
                        else if (AFound && BFound)
                            break;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("EC128-1: " + ex.Message);
                    }
                }

                if (rows.Count <= 0)
                    throw new Exception("EC128-2: Could not determine start character.");
            }

            return rows;
        }

        private string CalculateCheckDigit()
        {
            var currentStartChar = _FormattedData[0];
            uint CheckSum = 0;

            for (uint i = 0; i < _FormattedData.Count; i++)
            {
                //replace apostrophes with double apostrophes for escape chars
                var s = _FormattedData[(int) i].Replace("'", "''");

                //try to find value in the A column
                var rows = C128_Code.Select("A = '" + s + "'");

                //try to find value in the B column
                if (rows.Length <= 0)
                    rows = C128_Code.Select("B = '" + s + "'");

                //try to find value in the C column
                if (rows.Length <= 0)
                    rows = C128_Code.Select("C = '" + s + "'");

                var value = uint.Parse(rows[0]["Value"].ToString());
                var addition = value*(i == 0 ? 1 : i);
                CheckSum += addition;
            }

            var Remainder = CheckSum%103;
            var RetRows = C128_Code.Select("Value = '" + Remainder + "'");
            return RetRows[0]["Encoding"].ToString();
        }

        private void BreakUpDataForEncoding()
        {
            var temp = string.Empty;
            var tempRawData = Raw_Data;

            //breaking the raw data up for code A and code B will mess up the encoding
            if (type == TYPES.A || type == TYPES.B)
            {
                foreach (var c in Raw_Data)
                    _FormattedData.Add(c.ToString());
                return;
            }
            if (type == TYPES.C)
            {
                try
                {
                    Convert.ToInt64(Raw_Data);
                }
                catch
                {
                    throw new Exception("EC128-6: Only numeric values can be encoded with C128-C.");
                }

                //CODE C: adds a 0 to the front of the Raw_Data if the length is not divisible by 2
                if (Raw_Data.Length%2 > 0)
                    tempRawData = "0" + Raw_Data;
            }

            foreach (var c in tempRawData)
            {
                if (char.IsNumber(c))
                {
                    if (string.IsNullOrEmpty(temp))
                    {
                        temp += c;
                    }
                    else
                    {
                        temp += c;
                        _FormattedData.Add(temp);
                        temp = string.Empty;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(temp))
                    {
                        _FormattedData.Add(temp);
                        temp = string.Empty;
                    }
                    _FormattedData.Add(c.ToString());
                }
            }

            //if something is still in temp go ahead and push it onto the queue
            if (!string.IsNullOrEmpty(temp))
            {
                _FormattedData.Add(temp);
                temp = string.Empty;
            }
        }

        private void InsertStartandCodeCharacters()
        {
            DataRow CurrentCodeSet = null;
            var CurrentCodeString = string.Empty;

            if (type != TYPES.DYNAMIC)
            {
                switch (type)
                {
                    case TYPES.A:
                        _FormattedData.Insert(0, "START_A");
                        break;
                    case TYPES.B:
                        _FormattedData.Insert(0, "START_B");
                        break;
                    case TYPES.C:
                        _FormattedData.Insert(0, "START_C");
                        break;
                    default:
                        throw new Exception("EC128-4: Unknown start type in fixed type encoding.");
                }
            }
            else
            {
                try
                {
                    for (var i = 0; i < _FormattedData.Count; i++)
                    {
                        var col = 0;
                        var tempStartChars = FindStartorCodeCharacter(_FormattedData[i], ref col);

                        //check all the start characters and see if we need to stay with the same codeset or if a change of sets is required
                        var sameCodeSet = false;
                        foreach (var row in tempStartChars)
                        {
                            if (row["A"].ToString().EndsWith(CurrentCodeString) ||
                                row["B"].ToString().EndsWith(CurrentCodeString) ||
                                row["C"].ToString().EndsWith(CurrentCodeString))
                            {
                                sameCodeSet = true;
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(CurrentCodeString) || !sameCodeSet)
                        {
                            CurrentCodeSet = tempStartChars[0];

                            var error = true;
                            while (error)
                            {
                                try
                                {
                                    CurrentCodeString = CurrentCodeSet[col].ToString().Split('_')[1];
                                    error = false;
                                }
                                catch
                                {
                                    error = true;

                                    if (col++ > CurrentCodeSet.ItemArray.Length)
                                        throw new Exception("No start character found in CurrentCodeSet.");
                                }
                            }

                            _FormattedData.Insert(i++, CurrentCodeSet[col].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("EC128-3: Could not insert start and code characters.\n Message: " + ex.Message);
                }
            }
        }

        private string GetEncoding()
        {
            //break up data for encoding
            BreakUpDataForEncoding();

            //insert the start characters
            InsertStartandCodeCharacters();

            var CheckDigit = CalculateCheckDigit();

            var Encoded_Data = string.Empty;
            foreach (var s in _FormattedData)
            {
                //handle exception with apostrophes in select statements
                var s1 = s.Replace("'", "''");
                DataRow[] E_Row;

                //select encoding only for type selected
                switch (type)
                {
                    case TYPES.A:
                        E_Row = C128_Code.Select("A = '" + s1 + "'");
                        break;
                    case TYPES.B:
                        E_Row = C128_Code.Select("B = '" + s1 + "'");
                        break;
                    case TYPES.C:
                        E_Row = C128_Code.Select("C = '" + s1 + "'");
                        break;
                    case TYPES.DYNAMIC:
                        E_Row = C128_Code.Select("A = '" + s1 + "'");

                        if (E_Row.Length <= 0)
                        {
                            E_Row = C128_Code.Select("B = '" + s1 + "'");

                            if (E_Row.Length <= 0)
                            {
                                E_Row = C128_Code.Select("C = '" + s1 + "'");
                            }
                        }
                        break;
                    default:
                        E_Row = null;
                        break;
                }

                if (E_Row == null || E_Row.Length <= 0)
                    throw new Exception("EC128-5: Could not find encoding of a value( " + s1 + " ) in C128 type " + type);

                Encoded_Data += E_Row[0]["Encoding"].ToString();
                _EncodedData.Add(E_Row[0]["Encoding"].ToString());
            }

            //add the check digit
            Encoded_Data += CalculateCheckDigit();
            _EncodedData.Add(CalculateCheckDigit());

            //add the stop character
            Encoded_Data += C128_Code.Select("A = 'STOP'")[0]["Encoding"].ToString();
            _EncodedData.Add(C128_Code.Select("A = 'STOP'")[0]["Encoding"].ToString());

            //add the termination bars
            Encoded_Data += "11";
            _EncodedData.Add("11");

            return Encoded_Data;
        }
    }
}