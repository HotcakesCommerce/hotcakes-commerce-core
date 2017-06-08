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

namespace Hotcakes.Commerce.BusinessRules
{
    public enum WorkflowNames
    {
        DropShip, // cf1bc2b5-b371-4b57-80c5-f26fbce94064
        OrderEdited, // ee046427-9da3-431f-92b8-5665ec20a59c
        OrderStatusChanged,
        PackageShipped, // f9d6b245-8809-4500-bd72-b54618b2d0a7
        PaymentChanged, // db14f186-4005-4726-9a37-d67c2dc284ec
        PaymentComplete, // c84658fb-fc72-458a-b0e6-c1708d9e84f2
        ProcessNewOrder, // eac003c6-a354-489f-ba2c-029f4311851a
        ProcessNewOrderPayments,
        ProcessNewOrderAfterPayments,
        VerifyOrderSize, // 242848e0-1e5a-4a85-be62-887a21fdf0f9
        ShippingChanged, // 0bd9414e-00a0-414e-8eb7-61121358a609
        ShippingComplete, // d139a636-04c7-41b9-adca-0533432735bc
        ThirdPartyCheckoutSelected, // 09739bec-8974-4c38-b026-f6ca8aed615d
        ProcessNewReturn
    }
}