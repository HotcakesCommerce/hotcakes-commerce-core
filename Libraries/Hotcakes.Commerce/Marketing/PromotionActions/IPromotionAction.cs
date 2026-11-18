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
using System.Collections.Generic;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    /// <summary>
    /// Defines a promotion operation that can describe, apply, and cancel a promotion effect.
    /// </summary>
    /// <remarks>
    /// Implementations persist their configuration in <see cref="Settings"/> and are selected by
    /// <see cref="TypeId"/>. Applying an action mutates the supplied <see cref="PromotionContext"/>
    /// or its current price/order state; cancellation is responsible for reversing effects that
    /// the implementation owns.
    /// </remarks>
    public interface IPromotionAction
    {
        /// <summary>Gets or sets the persisted action identifier.</summary>
        /// <remarks>A newly constructed action normally has an unsaved identifier.</remarks>
        long Id { get; set; }

        /// <summary>Gets the stable identifier used to deserialize this action type.</summary>
        Guid TypeId { get; }

        /// <summary>Gets or sets the string-valued settings persisted for this action.</summary>
        /// <remarks>Setting keys and value formats are compatibility-sensitive.</remarks>
        Dictionary<string, string> Settings { get; set; }

        /// <summary>Builds the customer-facing description of this action.</summary>
        /// <param name="app">The application context used to resolve display names where needed.</param>
        /// <returns>HTML suitable for the existing promotion-description surface.</returns>
        string FriendlyDescription(HotcakesApplication app);

        /// <summary>Applies the action to the promotion target in <paramref name="context"/>.</summary>
        /// <param name="context">The promotion evaluation context.</param>
        /// <returns><see langword="true"/> when the action was handled successfully; otherwise, <see langword="false"/>.</returns>
        bool ApplyAction(PromotionContext context);

        /// <summary>Cancels or rolls back the action's effects in <paramref name="context"/>.</summary>
        /// <param name="context">The promotion evaluation context containing the affected order or target.</param>
        /// <returns><see langword="true"/> when cancellation completed successfully; otherwise, <see langword="false"/>.</returns>
        bool CancelAction(PromotionContext context);
    }
}
