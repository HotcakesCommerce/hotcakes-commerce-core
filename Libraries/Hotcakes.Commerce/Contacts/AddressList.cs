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
using System.Runtime.Serialization;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Contacts
{
    [DataContract]
    public class AddressList : List<Address>
    {
        public string ToJson()
        {
            return Json.ObjectToJson(this);
        }

        public static AddressList FromJson(string jsonValues)
        {
            var result =
                Json.ObjectFromJson<List<Address>>(jsonValues);
            if (result == null)
            {
                result = new List<Address>();
            }

            var trueResult = new AddressList();
            trueResult.AddRange(result);
            return trueResult;
        }

        public new bool Contains(Address addressToCompare)
        {
            foreach (var address in this)
            {
                if (address.IsEqualTo(addressToCompare))
                {
                    return true;
                }
            }

            return false;
        }
    }
}