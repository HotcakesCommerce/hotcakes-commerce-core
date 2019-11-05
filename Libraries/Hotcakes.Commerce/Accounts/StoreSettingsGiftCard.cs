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

using System;

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public class StoreSettingsGiftCard
    {
        private readonly StoreSettings parent;

        public StoreSettingsGiftCard(StoreSettings s)
        {
            parent = s;
        }

        public bool GiftCardsEnabled
        {
            get { return parent.GetPropBool("GCEnabled"); }
            set { parent.SetProp("GCEnabled", value); }
        }

        public int ExpirationPeriodMonths
        {
            get { return parent.GetPropInt("GCExpirationPeriodMonths", 12); }
            set { parent.SetProp("GCExpirationPeriodMonths", value); }
        }

        public string CardNumberFormat
        {
            get { return parent.GetProp("GCCardNumberFormat", "GIFTXXXX-XXXX-XXXX-XXXX"); }
            set { parent.SetProp("GCCardNumberFormat", value); }
        }

        public bool UseAZSymbols
        {
            get { return parent.GetPropBool("GCUseAZSymbols"); }
            set { parent.SetProp("GCUseAZSymbols", value); }
        }

        public string PredefinedAmounts
        {
            get { return parent.GetProp("GCPredefinedAmounts", "25,50,100,250,500"); }
            set { parent.SetProp("GCPredefinedAmounts", value); }
        }

        public decimal MinAmount
        {
            get { return parent.GetPropDecimal("GCMinAmount", 1); }
            set { parent.SetProp("GCMinAmount", value.ToString()); }
        }

        public decimal MaxAmount
        {
            get { return parent.GetPropDecimal("GCMaxAmount", 500); }
            set { parent.SetProp("GCMaxAmount", value.ToString()); }
        }
    }
}