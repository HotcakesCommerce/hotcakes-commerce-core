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

using System.Collections.Generic;
using System.Linq;

namespace Hotcakes.Commerce.Catalog
{
    public class CategoryFacetKeyHelper
    {
        /// <summary>
        ///     Converts a text key to a list of longs
        /// </summary>
        /// <param name="key">Text key with comma separated values</param>
        /// <returns>Return list of long number</returns>
        public static List<long> ParseKeyToList(string key)
        {
            var results = new List<long>();

            var splitter = '-';
            if (key.Contains(',')) splitter = ',';

            var temp = key.Trim();
            var parts = key.Split(splitter);
            foreach (var part in parts)
            {
                long number = 0;
                if (long.TryParse(part, out number))
                {
                    results.Add(number);
                }
            }

            return results;
        }


        /// <summary>
        ///     Converts a string key to a list of non-zero
        ///     long values for passing to SQL
        /// </summary>
        /// <param name="key">Comma separated list of numbers</param>
        /// <returns>Return the string for SQL query</returns>
        public static string ParseKeyToSqlList(string key)
        {
            var result = string.Empty;
            var parts = ParseKeyToList(key);
            var nonzeroparts = parts.Where(y => y > 0).ToList();
            result = BuildKey(nonzeroparts, ",");
            return result;
        }


        /// <summary>
        ///     Converts a list of longs to a string key using the default separator of "-"
        /// </summary>
        /// <param name="numbers">List of numbers</param>
        /// <returns>Return string based on given list of numbers</returns>
        public static string BuildKey(List<long> numbers)
        {
            return BuildKey(numbers, "-");
        }


        /// <summary>
        ///     Converts a list of longs to a string key with a separator specified
        /// </summary>
        /// <param name="numbers">List of long number</param>
        /// <param name="separator">seperator by whihch needs to join the numbers</param>
        /// <returns>Returns string by combining list of numbers</returns>
        public static string BuildKey(List<long> numbers, string separator)
        {
            var result = string.Empty;

            for (var i = 0; i < numbers.Count; i++)
            {
                result += numbers[i].ToString();
                if (i < numbers.Count - 1)
                {
                    result += separator;
                }
            }

            return result;
        }

        /// <summary>
        ///     Creates an empty string key with the specified number of values
        /// </summary>
        /// <param name="count">How many places to hold</param>
        /// <returns>Return key string</returns>
        public static string BuildEmptyKey(int count)
        {
            var key = string.Empty;
            for (var i = 0; i < count; i++)
            {
                key += "0";
                if (i < count - 1)
                {
                    key += "-";
                }
            }
            return key;
        }

        /// <summary>
        ///     Takes a string based key and replaced the value at a specific index with a new value
        /// </summary>
        /// <param name="key">key to modify</param>
        /// <param name="indexToReplace">index of location to modify</param>
        /// <param name="newValue">value to replace</param>
        /// <returns></returns>
        public static string ReplaceKeyValue(string key, int indexToReplace, long newValue)
        {
            var parts = ParseKeyToList(key);
            if (indexToReplace < parts.Count)
            {
                parts[indexToReplace] = newValue;
            }
            return BuildKey(parts);
        }

        /// <summary>
        ///     Clear the key value for the given facet id
        /// </summary>
        /// <param name="key">Key to be cleared</param>
        /// <param name="allFacets">List of facet info</param>
        /// <param name="idToClear">Id of the facet for which needs to clear the key </param>
        /// <returns>Return new cleared key for the given facet id</returns>
        public static string ClearSelfAndChildrenFromKey(string key, List<CategoryFacet> allFacets, long idToClear)
        {
            var result = key;

            // replace self
            for (var i = 0; i < allFacets.Count; i++)
            {
                if (allFacets[i].Id == idToClear)
                {
                    result = ReplaceKeyValue(key, i, 0);
                    break;
                }
            }

            //replace children
            for (var i = 0; i < allFacets.Count; i++)
            {
                if (allFacets[i].ParentPropertyId == idToClear)
                {
                    result = ClearSelfAndChildrenFromKey(result, allFacets, allFacets[i].Id);
                }
            }

            return result;
        }
    }
}