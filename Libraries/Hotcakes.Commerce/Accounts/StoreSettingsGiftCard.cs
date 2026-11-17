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

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>Provides gift-card settings owned by a <see cref="StoreSettings"/> instance.</summary>
    /// <remarks>Values are scoped to the current store and staged in the parent façade. Setters do not independently commit to the database; persistence is performed by the caller.</remarks>
    [Serializable]
    public class StoreSettingsGiftCard
    {
        private readonly StoreSettings parent;

        /// <summary>Initializes a new gift-card settings façade.</summary>
        /// <param name="s">The owning store settings façade.</param>
        public StoreSettingsGiftCard(StoreSettings s)
        {
            parent = s;
        }

        /// <summary>Gets or sets whether gift cards are enabled.</summary>
        /// <value>The <c>GCEnabled</c> setting; unset values read as <see langword="false"/>.</value>
        public bool GiftCardsEnabled
        {
            get { return parent.GetPropBool("GCEnabled"); }
            set { parent.SetProp("GCEnabled", value); }
        }

        /// <summary>Gets or sets the gift-card expiration period in months.</summary>
        /// <value>The <c>GCExpirationPeriodMonths</c> setting; default is <c>12</c>.</value>
        public int ExpirationPeriodMonths
        {
            get { return parent.GetPropInt("GCExpirationPeriodMonths", 12); }
            set { parent.SetProp("GCExpirationPeriodMonths", value); }
        }

        /// <summary>Gets or sets the gift-card number format.</summary>
        /// <value>The <c>GCCardNumberFormat</c> setting; default is <c>GIFTXXXX-XXXX-XXXX-XXXX</c>.</value>
        public string CardNumberFormat
        {
            get { return parent.GetProp("GCCardNumberFormat", "GIFTXXXX-XXXX-XXXX-XXXX"); }
            set { parent.SetProp("GCCardNumberFormat", value); }
        }

        /// <summary>Gets or sets whether gift-card numbers may use A–Z symbols.</summary>
        /// <value>The <c>GCUseAZSymbols</c> setting; unset values read as <see langword="false"/>.</value>
        public bool UseAZSymbols
        {
            get { return parent.GetPropBool("GCUseAZSymbols"); }
            set { parent.SetProp("GCUseAZSymbols", value); }
        }

        /// <summary>Gets or sets the comma-separated predefined gift-card amounts.</summary>
        /// <value>The <c>GCPredefinedAmounts</c> setting; default is <c>25,50,100,250,500</c>.</value>
        public string PredefinedAmounts
        {
            get { return parent.GetProp("GCPredefinedAmounts", "25,50,100,250,500"); }
            set { parent.SetProp("GCPredefinedAmounts", value); }
        }

        /// <summary>Gets or sets the minimum gift-card amount.</summary>
        /// <value>The <c>GCMinAmount</c> setting; default is <c>1</c>.</value>
        public decimal MinAmount
        {
            get { return parent.GetPropDecimal("GCMinAmount", 1); }
            set { parent.SetProp("GCMinAmount", value.ToString()); }
        }

        /// <summary>Gets or sets the maximum gift-card amount.</summary>
        /// <value>The <c>GCMaxAmount</c> setting; default is <c>500</c>.</value>
        public decimal MaxAmount
        {
            get { return parent.GetPropDecimal("GCMaxAmount", 500); }
            set { parent.SetProp("GCMaxAmount", value.ToString()); }
        }
    }
}
