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

namespace Hotcakes.Shipping.FedEx
{
    public enum ServiceType
    {
        PRIORITYOVERNIGHT = 1,
        STANDARDOVERNIGHT = 2,
        FIRSTOVERNIGHT = 3,
        FEDEX2DAY = 4,
        FEDEX2DAY_AM = 20,
        FEDEXEXPRESSSAVER = 5,
        INTERNATIONALPRIORITY = 6,
        INTERNATIONALECONOMY = 7,
        INTERNATIONALFIRST = 8,
        FEDEX1DAYFREIGHT = 9,
        FEDEX2DAYFREIGHT = 10,
        FEDEX3DAYFREIGHT = 11,
        FEDEXGROUND = 12,
        GROUNDHOMEDELIVERY = 13,
        INTERNATIONALPRIORITYFREIGHT = 14,
        INTERNATIONALECONOMYFREIGHT = 15,
        EUROPEFIRSTINTERNATIONALPRIORITY = 16,
        SMARTPOST = 21
    }
}