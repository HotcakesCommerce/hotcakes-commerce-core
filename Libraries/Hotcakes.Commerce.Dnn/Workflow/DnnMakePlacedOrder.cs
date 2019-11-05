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

using System.Web;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.BusinessRules.OrderTasks;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Dnn.Workflow
{
    public class DnnMakePlacedOrder : MakePlacedOrder
    {
        protected override DeviceType DetermineDeviceType(HttpRequestBase request)
        {
            // We use same characteristics for Phone vs Table detection as in _views_common.less
            // There are @media-phone variable which defines phone as everything that has width less or equal to 480 pixels
            if (request.Browser.IsMobileDevice)
            {
                if (request.Browser.ScreenPixelsWidth <= 480)
                    return DeviceType.Phone;
                return DeviceType.Tablet;
            }

            return DeviceType.Desktop;
        }

        public override Task Clone()
        {
            return new DnnMakePlacedOrder();
        }
    }
}