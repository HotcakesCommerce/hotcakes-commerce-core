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

namespace Hotcakes.Shipping.USPostal.v4
{
    public enum DomesticPackageType
    {
        Ignore = -1,
        Variable = 0,
        FlatRateBox = 1,
        FlatRateBoxSmall = 2,
        FlatRateBoxMedium = 3,
        FlatRateBoxLarge = 4,
        FlatRateEnvelope = 5,
        FlatRateEnvelopePadded = 50,
        FlatRateEnvelopeLegal = 51,
        FlatRateEnvelopeSmall = 52,
        FlatRateEnvelopeWindow = 53,
        FlatRateEnvelopeGiftCard = 54,
        RegionalBoxRateA = 200,
        RegionalBoxRateB = 201,
        Rectangular = 6,
        NonRectangular = 7,
        FirstClassLetter = 100,
        FirstClassFlat = 101,
        FirstClassParcel = 102,
        FirstClassPostCard = 103
    }
}