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

namespace Hotcakes.Web
{
    [Serializable]
    public class PasswordGenerator
    {
        public static string GeneratePassword(int length)
        {
            var result = string.Empty;

            var r = new Random();
            r.Next();

            for (var i = 0; i < length; i++)
            {
                r.Next();
                if (i == 0)
                {
                    result += GetRandomPrintableLetter(r);
                }
                else
                {
                    result += GetRandomPrintableCharacter(r);
                }
            }

            return result;
        }

        private static string GetRandomPrintableCharacter(Random r)
        {
            const string passwordCharacters = "abcdefghijkmnopqrstuvwxyz23456789";
            var location = r.Next(passwordCharacters.Length - 1);
            return passwordCharacters.Substring(location, 1);
        }

        private static string GetRandomPrintableLetter(Random r)
        {
            const string passwordCharacters = "abcdefghijkmnopqrstuvwxyz";
            var location = r.Next(passwordCharacters.Length - 1);
            return passwordCharacters.Substring(location, 1);
        }
    }
}