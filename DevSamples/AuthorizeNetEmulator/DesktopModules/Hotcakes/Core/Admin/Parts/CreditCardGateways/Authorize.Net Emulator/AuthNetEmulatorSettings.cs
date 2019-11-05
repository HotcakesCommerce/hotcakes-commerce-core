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
using Hotcakes.Payment;

namespace AuthorizeNet.Emulator
{
    [Serializable]
    public class AuthNetEmulatorSettings : MethodSettings
    {
        public string MerchantLoginId
        {
            get { return GetSettingOrEmpty("MerchantLoginId"); }
            set { AddOrUpdate("MerchantLoginId", value); }
        }

        public string TransactionKey
        {
            get { return GetSettingOrEmpty("TransactionKey"); }
            set { AddOrUpdate("TransactionKey", value); }
        }

        public bool SendEmailToCustomer
        {
            get { return GetBoolSetting("SendEmailToCustomer"); }
            set { SetBoolSetting("SendEmailToCustomer", value); }
        }

        public bool DeveloperMode
        {
            get { return GetBoolSetting("DeveloperMode"); }
            set { SetBoolSetting("DeveloperMode", value); }
        }

        public bool TestMode
        {
            get { return GetBoolSetting("TestMode"); }
            set { SetBoolSetting("TestMode", value); }
        }

        public bool DebugMode
        {
            get { return GetBoolSetting("DebugMode"); }
            set { SetBoolSetting("DebugMode", value); }
        }
    }
}