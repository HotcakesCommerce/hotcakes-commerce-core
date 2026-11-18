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

namespace Hotcakes.Payment
{
    /// <summary>
    /// Represents the normalized result of an address verification service check.
    /// </summary>
    public enum AvsResponseType
    {
        /// <summary>Address verification was unavailable or inconclusive (B, G, P, R, S, or U).</summary>
        Unavailable, // B, G, P, R, S, U
        /// <summary>The payment gateway reported an address verification error (E).</summary>
        Error, // E
        /// <summary>Both the address and postal code matched (X or Y).</summary>
        FullMatch, // X, Y
        /// <summary>Neither the address nor postal code matched (N).</summary>
        NoMatch, // N
        /// <summary>The postal code matched but the address did not fully match (W or Z).</summary>
        PartialMatchPostalCode, // W, Z
        /// <summary>The address matched but the postal code did not fully match (A).</summary>
        PartialMatchAddress // A
    }
}
