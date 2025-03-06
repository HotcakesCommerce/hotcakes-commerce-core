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

namespace Hotcakes.Payment.Gateways
{
    [Serializable]
    public class TestGateway : PaymentGateway
    {
        // Constructor
        public TestGateway()
        {
            Settings = new TestGatewaySettings();
        }

        // Properties
        public override string Name
        {
            get { return "Test Gateway"; }
        }

        public override string Id
        {
            get { return "FCACE46F-7B9C-4b49-82B6-426CF522C0C6"; }
        }

        public TestGatewaySettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        // Methods
        public override void ProcessTransaction(Transaction t)
        {
            var result = false;

            switch (t.Action)
            {
                case ActionType.CreditCardCapture:
                    result = Settings.ResponseForCapture;
                    break;
                case ActionType.CreditCardCharge:
                    result = Settings.ResponseForCharge;
                    break;
                case ActionType.CreditCardHold:
                    result = Settings.ResponseForHold;
                    break;
                case ActionType.CreditCardRefund:
                    result = Settings.ResponseForRefund;
                    break;
                case ActionType.CreditCardVoid:
                    result = Settings.ResponseForVoid;
                    break;
            }

            t.Result.ReferenceNumber = Guid.NewGuid().ToString();
            t.Result.ReferenceNumber2 = Guid.NewGuid().ToString();

            t.Result.Succeeded = result;
        }
    }
}