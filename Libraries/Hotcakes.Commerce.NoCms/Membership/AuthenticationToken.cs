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

namespace Hotcakes.Commerce.Membership
{
    [Serializable]
    public class AuthenticationToken
    {
        public AuthenticationToken()
        {
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            ExpirationDateUTC = DateTime.MinValue.AddDays(1);
            UserBvin = string.Empty;
            TokenRejected = true;
        }

        public string Bvin { get; set; }
        public DateTime LastUpdated { get; set; }

        public DateTime ExpirationDate
        {
            get { return ExpirationDateUTC.ToLocalTime(); }
            set { ExpirationDateUTC = value.ToUniversalTime(); }
        }

        public DateTime ExpirationDateUTC { get; set; }
        public string UserBvin { get; set; }
        public bool TokenRejected { get; set; }

        public bool IsExpired
        {
            get
            {
                if (DateTime.Compare(DateTime.Now, ExpirationDate) <= 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}