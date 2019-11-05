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
    public class StoreSettingsMailServer
    {
        private readonly StoreSettings parent;

        public StoreSettingsMailServer(StoreSettings s)
        {
            parent = s;
        }

        public bool UseCustomMailServer
        {
            get { return parent.GetPropBool("UseCustomMailServer"); }
            set { parent.SetProp("UseCustomMailServer", value); }
        }

        public string HostAddress
        {
            get { return parent.GetProp("MailServer"); }
            set { parent.SetProp("MailServer", value); }
        }

        public bool UseAuthentication
        {
            get { return parent.GetPropBool("MailServerUseAuthentication"); }
            set { parent.SetProp("MailServerUseAuthentication", value); }
        }

        public string Username
        {
            get { return parent.GetProp("MailServerUsername"); }
            set { parent.SetProp("MailServerUsername", value); }
        }

        public string Password
        {
            get { return parent.GetProp("MailServerPassword"); }
            set { parent.SetProp("MailServerPassword", value); }
        }

        public string Port
        {
            get { return parent.GetProp("MailServerPort"); }
            set { parent.SetProp("MailServerPort", value); }
        }

        public bool UseSsl
        {
            get { return parent.GetPropBool("MailServerUseSsl"); }
            set { parent.SetProp("MailServerUseSsl", value); }
        }

        public string EmailForGeneral
        {
            get { return parent.GetProp("EmailForGeneral"); }
            set { parent.SetProp("EmailForGeneral", value); }
        }

        public string EmailForNewOrder
        {
            get { return parent.GetProp("EmailForNewOrder"); }
            set { parent.SetProp("EmailForNewOrder", value); }
        }

        public string FromEmail
        {
            get { return parent.GetProp("FromEmail"); }
            set { parent.SetProp("FromEmail", value); }
        }
    }
}