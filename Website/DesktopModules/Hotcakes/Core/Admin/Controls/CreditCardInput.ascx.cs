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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment;
using Hotcakes.Web.Validation;

namespace Hotcakes.Modules.Core.Controls
{
    partial class CreditCardInput : HccUserControl, IValidatable
    {
        private int _tabIndex = -1;

        public string CardNumber
        {
            get
            {
                var result = cccardnumber.Text.Trim();
                if (!result.StartsWith("*"))
                {
                    result = CardValidator.CleanCardNumber(result);
                }
                return result;
            }
            set { cccardnumber.Text = value; }
        }

        public string CardHolderName
        {
            get { return cccardholder.Text.Trim(); }
            set { cccardholder.Text = value; }
        }

        public string SecurityCode
        {
            get { return ccsecuritycode.Text.Trim(); }
            set { ccsecuritycode.Text = value; }
        }

        public string CardCode
        {
            get
            {
                var result = CardValidator.GetCardTypeFromNumber(CardNumber);
                switch (result)
                {
                    case CardType.Amex:
                        return "A";
                    case CardType.DinersClub:
                        return "C";
                    case CardType.Discover:
                        return "D";
                    case CardType.JCB:
                        return "J";
                    case CardType.Maestro:
                        return "MAESTRO";
                    case CardType.MasterCard:
                        return "M";
                    case CardType.Solo:
                        return "SOLO";
                    case CardType.Switch:
                        return "SWITCH";
                    case CardType.Visa:
                        return "V";
                    default:
                        return "UNKNOWN";
                }
            }
        }

        public int ExpirationMonth
        {
            get { return int.Parse(ccexpmonth.SelectedValue); }
            set
            {
                if (ccexpmonth.Items.FindByValue(value.ToString()) != null)
                {
                    ccexpmonth.ClearSelection();
                    ccexpmonth.Items.FindByValue(value.ToString()).Selected = true;
                }
            }
        }

        public int ExpirationYear
        {
            get { return int.Parse(ccexpyear.SelectedValue); }
            set
            {
                if (ccexpyear.Items.FindByValue(value.ToString()) != null)
                {
                    ccexpyear.ClearSelection();
                    ccexpyear.Items.FindByValue(value.ToString()).Selected = true;
                }
            }
        }

        public int TabIndex
        {
            get { return _tabIndex; }
            set { _tabIndex = value; }
        }

        public bool ShowSecurityCode { get; set; }

        public List<RuleViolation> GetRuleViolations()
        {
            var violations = new List<RuleViolation>();

            // Card Number
            if (!CardNumber.StartsWith("****-****-****-"))
            {
                CardNumber = CardValidator.CleanCardNumber(CardNumber);
            }

            if (!CardValidator.IsCardNumberValid(CardNumber))
            {
                violations.Add(new RuleViolation("Credit Card Number", string.Empty, "Please enter a valid credit card number",
                    "cccardnumber"));
            }

            var cardTypeCheck = CardValidator.GetCardTypeFromNumber(CardNumber);
            var acceptedCards = HccApp.CurrentStore.Settings.PaymentAcceptedCards;
            if (!acceptedCards.Contains(cardTypeCheck))
            {
                violations.Add(new RuleViolation("Card Type Not Accepted", string.Empty,
                    "That card type is not accepted by this store. Please use a different card.", "cccardnumber"));
            }

            ValidationHelper.RequiredMinimum(1, "Card Expiration Year is required", ExpirationYear, violations,
                "ccexpyear");
            ValidationHelper.RequiredMinimum(1, "Card Expiration Month is required", ExpirationMonth, violations,
                "ccexpmonth");
            ValidationHelper.Required("Name on Card is required", CardHolderName, violations, "cccardholder");

            if (HccApp.CurrentStore.Settings.PaymentCreditCardRequireCVV)
            {
                ValidationHelper.RequiredMinimum(3, "Card Security Code is required", SecurityCode.Length, violations,
                    "ccsecuritycode");
            }

            SetErrorCss(violations);

            return violations;
        }

        public bool IsValid()
        {
            if (GetRuleViolations().Count > 0)
            {
                return false;
            }
            return true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeFields();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (TabIndex != -1)
            {
                cccardnumber.TabIndex = (short) (TabIndex + 1);
                ccexpmonth.TabIndex = (short) (TabIndex + 2);
                ccexpyear.TabIndex = (short) (TabIndex + 3);
                ccsecuritycode.TabIndex = (short) (TabIndex + 4);
                cccardholder.TabIndex = (short) (TabIndex + 5);
            }

            DisplayAcceptedCards();
        }

        private void DisplayAcceptedCards()
        {
            foreach (var t in HccApp.CurrentStore.Settings.PaymentAcceptedCards)
            {
                switch (t)
                {
                    case CardType.Visa:
                        litCardsAccepted.Text += "<span class=\"cc-visa\"></span>";
                        break;
                    case CardType.MasterCard:
                        litCardsAccepted.Text += "<span class=\"cc-mastercard\"></span>";
                        break;
                    case CardType.Amex:
                        litCardsAccepted.Text += "<span class=\"cc-amex\"></span>";
                        break;
                    case CardType.Discover:
                        litCardsAccepted.Text += "<span class=\"cc-discover\"></span>";
                        break;
                    case CardType.DinersClub:
                        litCardsAccepted.Text += "<span class=\"cc-diners\"></span>";
                        break;
                    case CardType.JCB:
                        litCardsAccepted.Text += "<span class=\"cc-jcb\"></span>";
                        break;
                }
            }
        }

        public void InitializeFields()
        {
            securityCodeRow.Visible = ShowSecurityCode;
            LoadMonths();
            LoadYears();
        }

        private void LoadYears()
        {
            ccexpyear.Items.Clear();
            ccexpyear.Items.Add(new ListItem("----", "0"));

            var CurrentYear = DateTime.UtcNow.ToLocalTime().Year;
            for (var iTempCounter = 0; iTempCounter <= 9; iTempCounter += 1)
            {
                var liTemp = new ListItem();
                liTemp.Text = (iTempCounter + CurrentYear).ToString();
                liTemp.Value = (iTempCounter + CurrentYear).ToString();
                ccexpyear.Items.Add(liTemp);
                liTemp = null;
            }
        }

        private void LoadMonths()
        {
            ccexpmonth.Items.Clear();
            ccexpmonth.Items.Add(new ListItem("--", "0"));
            for (var iTempCounter = 1; iTempCounter <= 12; iTempCounter += 1)
            {
                var liTemp = new ListItem();
                liTemp.Text = iTempCounter.ToString();
                liTemp.Value = iTempCounter.ToString();
                ccexpmonth.Items.Add(liTemp);
                liTemp = null;
            }
        }

        public void LoadFromCardData(CardData card)
        {
            ExpirationMonth = card.ExpirationMonth;
            ExpirationYear = card.ExpirationYear;
            CardHolderName = card.CardHolderName;
            if (card.CardNumber.Trim().Length >= 4)
            {
                CardNumber = "****-****-****-" + card.CardNumberLast4Digits;
            }
        }

        public CardData GetCardData()
        {
            var result = new CardData();
            result.CardHolderName = CardHolderName;
            if (CardNumber.StartsWith("*"))
            {
                result.CardNumber = string.Empty;
            }
            else
            {
                result.CardNumber = CardNumber;
            }
            result.SecurityCode = SecurityCode;
            result.ExpirationMonth = ExpirationMonth;
            result.ExpirationYear = ExpirationYear;

            return result;
        }

        public void ClearCssViolations()
        {
            SetErrorCss(new List<RuleViolation>());
        }

        private void SetErrorCss(List<RuleViolation> violations)
        {
            // Clear Out Previous Error Classes
            cccardnumber.CssClass = string.Empty;
            cccardholder.CssClass = string.Empty;
            ccexpyear.CssClass = string.Empty;
            ccexpmonth.CssClass = string.Empty;
            ccsecuritycode.CssClass = string.Empty;

            // Tag controls with violations with CSS class
            foreach (var v in violations)
            {
                var cntrl = (WebControl) FindControl(v.ControlName);
                if (cntrl != null)
                {
                    cntrl.CssClass = "input-validation-error";
                }
            }
        }
    }
}