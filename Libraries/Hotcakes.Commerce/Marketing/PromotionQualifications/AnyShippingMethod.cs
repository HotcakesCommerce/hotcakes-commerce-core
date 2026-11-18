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

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    /// <summary>Matches any non-sale order or line-item shipping evaluation.</summary>
    /// <remarks>This is the lowest-cost, optionless shipping qualification.</remarks>
    public class AnyShippingMethod : PromotionQualificationBase
    {
        /// <summary>Initializes an instance with <see cref="RelativeProcessingCost.Lowest"/>.</summary>
        public AnyShippingMethod()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
        }

        /// <summary>Gets the stable identifier for the any-shipping-method qualification.</summary>
        public override Guid TypeId
        {
            get { return new Guid(TypeIdAnyShippingMethod); }
        }

        /// <summary>Gets a value indicating that this qualification has no settings.</summary>
        public override bool HasOptions
        {
            get { return false; }
        }

        /// <summary>Gets the display description for the catch-all shipping rule.</summary>
        /// <param name="app">The application context; unused by this rule.</param>
        /// <returns>The literal description <c>When Any Shipping Method</c>.</returns>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            return "When Any Shipping Method";
        }

        /// <summary>Matches order or line-item scope when an order exists and the context is not a sale.</summary>
        /// <param name="context">The promotion context to inspect.</param>
        /// <param name="mode">The requested evaluation scope.</param>
        /// <returns><c>true</c> when the applicable order and non-sale checks pass.</returns>
        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.Orders &&
                mode != PromotionQualificationMode.LineItems) return false;
            if (context == null) return false;
            if (context.Order == null) return false;
            if (context.Mode == PromotionType.Sale) return false;

            return true;
        }
    }
}
