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
    /// <summary>Provides outgoing-mail settings owned by a <see cref="StoreSettings"/> instance.</summary>
    /// <remarks>Values are store-scoped and staged through the parent façade. <see cref="Password"/> is a sensitive credential. Setters do not independently persist to the database.</remarks>
    [Serializable]
    public class StoreSettingsMailServer
    {
        private readonly StoreSettings parent;

        /// <summary>Initializes a new mail-server settings façade.</summary>
        /// <param name="s">The owning store settings façade.</param>
        public StoreSettingsMailServer(StoreSettings s)
        {
            parent = s;
        }

        /// <summary>Gets or sets whether the store uses a custom mail server.</summary>
        /// <value>The <c>UseCustomMailServer</c> setting; unset values read as <see langword="false"/>.</value>
        public bool UseCustomMailServer
        {
            get { return parent.GetPropBool("UseCustomMailServer"); }
            set { parent.SetProp("UseCustomMailServer", value); }
        }

        /// <summary>Gets or sets the custom mail-server host address.</summary>
        /// <value>The <c>MailServer</c> setting.</value>
        public string HostAddress
        {
            get { return parent.GetProp("MailServer"); }
            set { parent.SetProp("MailServer", value); }
        }

        /// <summary>Gets or sets whether mail-server authentication is used.</summary>
        /// <value>The <c>MailServerUseAuthentication</c> setting; unset values read as <see langword="false"/>.</value>
        public bool UseAuthentication
        {
            get { return parent.GetPropBool("MailServerUseAuthentication"); }
            set { parent.SetProp("MailServerUseAuthentication", value); }
        }

        /// <summary>Gets or sets the mail-server username.</summary>
        /// <value>The <c>MailServerUsername</c> setting.</value>
        public string Username
        {
            get { return parent.GetProp("MailServerUsername"); }
            set { parent.SetProp("MailServerUsername", value); }
        }

        /// <summary>Gets or sets the mail-server password.</summary>
        /// <value>The sensitive <c>MailServerPassword</c> setting.</value>
        public string Password
        {
            get { return parent.GetProp("MailServerPassword"); }
            set { parent.SetProp("MailServerPassword", value); }
        }

        /// <summary>Gets or sets the mail-server port.</summary>
        /// <value>The <c>MailServerPort</c> setting.</value>
        public string Port
        {
            get { return parent.GetProp("MailServerPort"); }
            set { parent.SetProp("MailServerPort", value); }
        }

        /// <summary>Gets or sets whether SSL is used for the mail connection.</summary>
        /// <value>The <c>MailServerUseSsl</c> setting; unset values read as <see langword="false"/>.</value>
        public bool UseSsl
        {
            get { return parent.GetPropBool("MailServerUseSsl"); }
            set { parent.SetProp("MailServerUseSsl", value); }
        }

        /// <summary>Gets or sets the general notification recipient.</summary>
        /// <value>The <c>EmailForGeneral</c> setting.</value>
        public string EmailForGeneral
        {
            get { return parent.GetProp("EmailForGeneral"); }
            set { parent.SetProp("EmailForGeneral", value); }
        }

        /// <summary>Gets or sets the new-order notification recipient.</summary>
        /// <value>The <c>EmailForNewOrder</c> setting.</value>
        public string EmailForNewOrder
        {
            get { return parent.GetProp("EmailForNewOrder"); }
            set { parent.SetProp("EmailForNewOrder", value); }
        }

        /// <summary>Gets or sets the sender email address.</summary>
        /// <value>The <c>FromEmail</c> setting.</value>
        public string FromEmail
        {
            get { return parent.GetProp("FromEmail"); }
            set { parent.SetProp("FromEmail", value); }
        }
    }
}
