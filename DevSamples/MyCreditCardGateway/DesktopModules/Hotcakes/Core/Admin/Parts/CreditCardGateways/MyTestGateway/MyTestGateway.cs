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

namespace MyCompany.MyCreditCardGateway
{
    [Serializable]
    public class MyTestGateway : PaymentGateway
    {
        public MyTestGateway()
        {
            Settings = new MyTestGatewaySettings();
        }

        public override string Name
        {
            get { return "MyTestGateway"; }
        }

        public override string Id
        {
            get { return "09C46164-E3B4-4ECE-9890-4BFA4FF4A57A"; }
        }

        public MyTestGatewaySettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        protected override void Authorize(Transaction t)
        {
            // Perform Authorize operation throught webservice
            // and fill Transaction object with result data
            ProcessOperation(t, "Authorize");
        }

        protected override void Charge(Transaction t)
        {
            // Perform Charge operation throught webservice
            // and fill Transaction object with result data
            ProcessOperation(t, "Charge");
        }

        protected override void Capture(Transaction t)
        {
            // Perform Capture operation throught webservice
            // and fill Transaction object with result data
            ProcessOperation(t, "Capture");
        }

        protected override void Refund(Transaction t)
        {
            // Perform Refund operation throught webservice
            // and fill Transaction object with result data
            ProcessOperation(t, "Refund");
        }

        protected override void Void(Transaction t)
        {
            // Perform Void operation throught webservice
            // and fill Transaction object with result data
            ProcessOperation(t, "Void");
        }

        private static void ProcessOperation(Transaction t, string type)
        {
            var succeeded = true;
            //succeeded = <Service result> (you can use gateway settings here: Settings.UserName, Settings.Password)
            if (succeeded)
            {
                t.Result.ReferenceNumber = "<transactionId>";
                t.Result.Succeeded = true;
            }
            else
            {
                t.Result.Messages.Add(new Message("MyTestGateway " + type + " Transaction Failed", "FAILED",
                    MessageType.Error));
                t.Result.Messages.Add(new Message("<Error Description>", "<Error Code>", MessageType.Error));
                t.Result.Succeeded = false;
            }
        }
    }
}