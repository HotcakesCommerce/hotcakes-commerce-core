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
using System.Text;
using System.Threading;

namespace Hotcakes.Commerce.Utilities
{
    public class RandomNumbers
    {
        /// <summary>
        ///     Generates a random integer that is between maxNumber and minNumber inclusive of maxNumber and minNumber
        /// </summary>
        /// <param name="maxNumber">The maximum possible value that the method will return</param>
        /// <param name="minNumber">The minimum possible value that the method will return</param>
        /// <returns>An integer between the max and min parameters inclusive of the parameters</returns>
        public static int RandomInteger(int maxNumber, int minNumber)
        {
            return RandomInteger(maxNumber, minNumber, DateTime.Now.Millisecond);
        }

        public static int RandomIntegerRepeatable(int maxNumber, int minNumber)
        {
            Thread.Sleep(10);
            return RandomInteger(maxNumber, minNumber, DateTime.Now.Millisecond);
        }

        /// <summary>
        ///     Generates a random integer that is between maxNumber and minNumber inclusive of maxNumber and minNumber
        /// </summary>
        /// <param name="maxNumber">The maximum possible value that the method will return</param>
        /// <param name="minNumber">The minimum possible value that the method will return</param>
        /// <param name="seed">The random integer to use as a seed</param>
        /// <returns>An integer between the max and min parameters inclusive of the parameters</returns>
        public static int RandomInteger(int maxNumber, int minNumber, int seed)
        {
            if (maxNumber < minNumber)
            {
                var temp = maxNumber;
                maxNumber = minNumber;
                minNumber = temp;
            }
            var r = new Random(seed);

            // .NET random function is not inclusive of max value so add one
            maxNumber = maxNumber + 1;

            return r.Next(minNumber, maxNumber);
        }
        
        public static string Create16DigitString()
        {
            var RNG = new Random();
            var builder = new StringBuilder();
            while (builder.Length < 16)
            {
                builder.Append(RNG.Next(10).ToString());
            }
            return builder.ToString();
        }
    }
}