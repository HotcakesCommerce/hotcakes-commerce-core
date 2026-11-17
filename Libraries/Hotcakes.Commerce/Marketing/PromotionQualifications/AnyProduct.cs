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
    /// <summary>Matches any product evaluation with a product and user price.</summary>
    /// <remarks>This is the lowest-cost, optionless product qualification.</remarks>
    public class AnyProduct : PromotionQualificationBase
    {
        /// <summary>Initializes an instance with <see cref="RelativeProcessingCost.Lowest"/>.</summary>
        public AnyProduct()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
        }

        /// <summary>Gets the stable identifier for the any-product qualification.</summary>
        public override Guid TypeId
        {
            get { return new Guid(TypeIdAnyProduct); }
        }

        /// <summary>Gets a value indicating that this qualification has no settings.</summary>
        public override bool HasOptions
        {
            get { return false; }
        }

        /// <summary>Gets the display description for the catch-all product rule.</summary>
        /// <param name="app">The application context; unused by this rule.</param>
        /// <returns>The literal description <c>When Any Product</c>.</returns>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            return "When Any Product";
        }

        /// <summary>Matches product scope when the context contains both a product and user price.</summary>
        /// <param name="context">The promotion context to inspect.</param>
        /// <param name="mode">The requested evaluation scope.</param>
        /// <returns><c>true</c> only for <see cref="PromotionQualificationMode.Products"/> with the required context values.</returns>
        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.Products) return false;
            if (context == null) return false;
            if (context.Product == null) return false;
            if (context.UserPrice == null) return false;

            return true;
        }
    }
}
