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

namespace Hotcakes.Shipping.Ups
{
    [Serializable]
    public class UPSServiceGlobalSettings
    {
        public UPSServiceGlobalSettings()
        {
            AccountNumber = string.Empty;
            ClientId = string.Empty;
            ClientSecret = string.Empty;
            DiagnosticsMode = false;
            ForceResidential = true;
            IgnoreDimensions = true;
            PickUpType = PickupType.CustomerCounter;
            DefaultPackaging = PackagingType.CustomerSupplied;
        }

        public string AccountNumber { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public bool TestingMode { get; set; }
        public bool DiagnosticsMode { get; set; }
        public bool ForceResidential { get; set; }
        public bool IgnoreDimensions { get; set; }
        public PickupType PickUpType { get; set; }
        public PackagingType DefaultPackaging { get; set; }
    }
}