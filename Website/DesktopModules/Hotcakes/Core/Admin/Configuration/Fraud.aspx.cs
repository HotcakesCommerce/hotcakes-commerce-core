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

using System;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Security;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class Fraud : BaseAdminPage
    {
        private FraudRuleRepository repository;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            repository = new FraudRuleRepository(HccApp.CurrentRequestContext);

            if (!Page.IsPostBack)
            {
                LoadLists();
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("FraudScreening");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        private void LoadLists()
        {
            var rules = repository.FindForStore(HccApp.CurrentStore.Id);

            var emailRules = new SortableCollection<FraudRule>();
            var domainRules = new SortableCollection<FraudRule>();
            var ipRules = new SortableCollection<FraudRule>();
            var PHrules = new SortableCollection<FraudRule>();
            var CCrules = new SortableCollection<FraudRule>();

            for (var i = 0; i <= rules.Count - 1; i++)
            {
                switch (rules[i].RuleType)
                {
                    case FraudRuleType.DomainName:
                        domainRules.Add(rules[i]);
                        break;
                    case FraudRuleType.EmailAddress:
                        emailRules.Add(rules[i]);
                        break;
                    case FraudRuleType.IPAddress:
                        ipRules.Add(rules[i]);
                        break;
                    case FraudRuleType.PhoneNumber:
                        PHrules.Add(rules[i]);
                        break;
                    case FraudRuleType.CreditCardNumber:
                        CCrules.Add(rules[i]);
                        break;
                }
            }

            emailRules.Sort("RuleValue");
            domainRules.Sort("RuleValue");
            ipRules.Sort("RuleValue");
            PHrules.Sort("RuleValue");
            CCrules.Sort("RuleValue");

            lstEmail.DataSource = emailRules;
            lstEmail.DataTextField = "RuleValue";
            lstEmail.DataValueField = "Bvin";
            lstEmail.DataBind();

            lstDomain.DataTextField = "RuleValue";
            lstDomain.DataValueField = "Bvin";
            lstDomain.DataSource = domainRules;
            lstDomain.DataBind();

            lstIP.DataTextField = "RuleValue";
            lstIP.DataValueField = "Bvin";
            lstIP.DataSource = ipRules;
            lstIP.DataBind();

            lstPhoneNumber.DataTextField = "RuleValue";
            lstPhoneNumber.DataValueField = "Bvin";
            lstPhoneNumber.DataSource = PHrules;
            lstPhoneNumber.DataBind();

            lstCreditCard.DataTextField = "RuleValue";
            lstCreditCard.DataValueField = "Bvin";
            lstCreditCard.DataSource = CCrules;
            lstCreditCard.DataBind();

            EmailField.Text = string.Empty;
            DomainField.Text = string.Empty;
            IPField.Text = string.Empty;
            PhoneNumberField.Text = string.Empty;
            CreditCardField.Text = string.Empty;
        }

        // Email Address
        protected void btnNewEmail_Click(object sender, EventArgs e)
        {
            if (EmailField.Text.Trim().Length > 0)
            {
                var r = new FraudRule();
                r.RuleType = FraudRuleType.EmailAddress;
                r.RuleValue = EmailField.Text.Trim().ToLower();
                repository.Create(r);
            }
            LoadLists();
        }

        protected void btnDeleteEmail_Click(object sender, EventArgs e)
        {
            for (var i = 0; i <= lstEmail.Items.Count - 1; i++)
            {
                if (lstEmail.Items[i].Selected)
                {
                    DeleteRule(lstEmail.Items[i].Value);
                }
            }
            LoadLists();
        }

        // IP Address
        protected void btnNewIP_Click(object sender, EventArgs e)
        {
            if (IPField.Text.Trim().Length > 0)
            {
                var r = new FraudRule();
                r.RuleType = FraudRuleType.IPAddress;
                r.RuleValue = IPField.Text.Trim().ToLower();
                repository.Create(r);
            }
            LoadLists();
        }

        protected void btnDeleteIP_Click(object sender, EventArgs e)
        {
            for (var i = 0; i <= lstIP.Items.Count - 1; i++)
            {
                if (lstIP.Items[i].Selected)
                {
                    DeleteRule(lstIP.Items[i].Value);
                }
            }
            LoadLists();
        }

        // Domain Name
        protected void btnNewDomain_Click(object sender, EventArgs e)
        {
            if (DomainField.Text.Trim().Length > 0)
            {
                var r = new FraudRule();
                r.RuleType = FraudRuleType.DomainName;
                r.RuleValue = DomainField.Text.Trim().ToLower();
                repository.Create(r);
            }
            LoadLists();
        }

        protected void btnDeleteDomain_Click(object sender, EventArgs e)
        {
            for (var i = 0; i <= lstDomain.Items.Count - 1; i++)
            {
                if (lstDomain.Items[i].Selected)
                {
                    DeleteRule(lstDomain.Items[i].Value);
                }
            }
            LoadLists();
        }

        // Phone Number
        protected void btnNewPhoneNumber_Click(object sender, EventArgs e)
        {
            if (PhoneNumberField.Text.Trim().Length > 0)
            {
                var r = new FraudRule();
                r.RuleType = FraudRuleType.PhoneNumber;
                r.RuleValue = PhoneNumberField.Text.Trim().ToLower();
                repository.Create(r);
            }
            LoadLists();
        }

        protected void btnDeletePhoneNumber_Click(object sender, EventArgs e)
        {
            for (var i = 0; i <= lstPhoneNumber.Items.Count - 1; i++)
            {
                if (lstPhoneNumber.Items[i].Selected)
                {
                    DeleteRule(lstPhoneNumber.Items[i].Value);
                }
            }
            LoadLists();
        }

        // Credit Card Number
        protected void btnNewCCNumber_Click(object sender, EventArgs e)
        {
            if (CreditCardField.Text.Trim().Length > 0)
            {
                var r = new FraudRule();
                r.RuleType = FraudRuleType.CreditCardNumber;
                r.RuleValue = CreditCardField.Text.Trim().ToLower();
                repository.Create(r);
            }
            LoadLists();
        }

        protected void btnDeleteCCNumber_Click(object sender, EventArgs e)
        {
            for (var i = 0; i <= lstCreditCard.Items.Count - 1; i++)
            {
                if (lstCreditCard.Items[i].Selected)
                {
                    DeleteRule(lstCreditCard.Items[i].Value);
                }
            }
            LoadLists();
        }

        private void DeleteRule(string bvin)
        {
            repository.Delete(bvin);
        }
    }
}