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
    /// Identifies the card network or brand associated with a payment card.
    /// </summary>
    public enum CardType
    {
        /// <summary>No card network is specified or recognized.</summary>
        Unknown = 0,
        /// <summary>American Express.</summary>
        Amex = 1,
        /// <summary>Discover.</summary>
        Discover = 2,
        /// <summary>Diners Club.</summary>
        DinersClub = 3,
        /// <summary>JCB.</summary>
        JCB = 4,
        /// <summary>Maestro.</summary>
        Maestro = 5,
        /// <summary>Mastercard.</summary>
        MasterCard = 6,
        /// <summary>Solo.</summary>
        Solo = 7,
        /// <summary>Switch.</summary>
        Switch = 8,
        /// <summary>Visa.</summary>
        Visa = 9
    }
}
