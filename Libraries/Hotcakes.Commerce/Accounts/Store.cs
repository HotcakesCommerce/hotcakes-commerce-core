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

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public class Store
    {
        private string _CustomUrl = string.Empty;


        public Store()
        {
            Id = 0;
            StoreGuid = Guid.NewGuid();
            StoreName = string.Empty;
            DateCreated = DateTime.UtcNow;
            CustomUrl = string.Empty;
            Settings = new StoreSettings(this);
        }

        public long Id { get; set; }
        public Guid StoreGuid { get; set; }
        public string StoreName { get; set; }
        public DateTime DateCreated { get; set; }

        public string CustomUrl
        {
            get { return _CustomUrl; }
            set { _CustomUrl = value.Trim().ToLowerInvariant(); }
        }

        public StoreSettings Settings { get; set; }

        public string RootUrl()
        {
            return Factory.CreateHccUrlResolver().GetStoreRootUrl(this);
        }

        public string RootUrlSecure()
        {
            return Factory.CreateHccUrlResolver().GetStoreRootUrl(this, true);
        }
    }
}