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

namespace Hotcakes.Commerce.Payment.Methods
{
    /// <summary>Built-in Moneris checkout method.</summary>
    [Serializable]
    public class Moneris : PaymentMethod
    {
        /// <summary>Gets the stable Moneris method identifier.</summary>
        public override string MethodId
        {
            get { return Id(); }
        }

        /// <summary>Gets the serialized display name, <c>Moneris</c>.</summary>
        public override string MethodName
        {
            get { return "Moneris"; }
        }

        /// <summary>Gets the checkout ordering value, 201.</summary>
        public override int SortIndex
        {
            get { return 201; }
        }

        /// <summary>Gets <see langword="true"/> because this method pays before order placement.</summary>
        public override bool PayBeforePlacement
        {
            get { return true; }
        }

        /// <summary>Returns the stable identifier used by <see cref="PaymentMethods"/>.</summary>
        /// <returns>The Moneris method identifier.</returns>
        public static string Id()
        {
            return PaymentMethods.MonerisId;
        }
    }
}
