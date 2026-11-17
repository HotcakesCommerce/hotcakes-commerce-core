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

namespace Hotcakes.Commerce.BusinessRules
{
    /// <summary>Names of the built-in order and shipping workflows.</summary>
    /// <remarks>These names are compatibility identifiers used by <see cref="WorkflowFactory"/>; do not rename or reorder them.</remarks>
    /// <seealso cref="WorkflowFactory.CreateWorkflow(WorkflowNames)"/>
    public enum WorkflowNames
    {
        /// <summary>Runs drop-shipping tasks when an order is drop shipped.</summary>
        DropShip, // cf1bc2b5-b371-4b57-80c5-f26fbce94064
        /// <summary>Runs after an order is edited.</summary>
        OrderEdited, // ee046427-9da3-431f-92b8-5665ec20a59c
        /// <summary>Runs after an order status changes.</summary>
        OrderStatusChanged,
        /// <summary>Runs after a package is marked shipped.</summary>
        PackageShipped, // f9d6b245-8809-4500-bd72-b54618b2d0a7
        /// <summary>Runs after the payment method changes.</summary>
        PaymentChanged, // db14f186-4005-4726-9a37-d67c2dc284ec
        /// <summary>Runs after payment is completed.</summary>
        PaymentComplete, // c84658fb-fc72-458a-b0e6-c1708d9e84f2
        /// <summary>Processes a new order before payment processing.</summary>
        ProcessNewOrder, // eac003c6-a354-489f-ba2c-029f4311851a
        /// <summary>Processes payments for a new order.</summary>
        ProcessNewOrderPayments,
        /// <summary>Runs after payments for a new order have been applied.</summary>
        ProcessNewOrderAfterPayments,
        /// <summary>Verifies configured order size limits.</summary>
        VerifyOrderSize, // 242848e0-1e5a-4a85-be62-887a21fdf0f9
        /// <summary>Runs after shipping status changes.</summary>
        ShippingChanged, // 0bd9414e-00a0-414e-8eb7-61121358a609
        /// <summary>Runs after shipping is complete.</summary>
        ShippingComplete, // d139a636-04c7-41b9-adca-0533432735bc
        /// <summary>Runs the selected external checkout provider.</summary>
        ThirdPartyCheckoutSelected, // 09739bec-8974-4c38-b026-f6ca8aed615d
        /// <summary>Compatibility name for return processing; its factory mapping currently has no tasks.</summary>
        ProcessNewReturn
    }
}
