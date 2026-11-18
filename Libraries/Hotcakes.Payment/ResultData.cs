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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hotcakes.Payment
{
    /// <summary>
    /// Contains the outcome, provider response details, and messages from a payment operation.
    /// </summary>
    [Serializable]
    public class ResultData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultData"/> class with unsuccessful status and empty response collections.
        /// </summary>
        public ResultData()
        {
            Succeeded = false;
            AvsCode = AvsResponseType.Unavailable;
            AvsCodeDescription = string.Empty;
            CvvCode = CvnResponseType.Unavailable;
            CvvCodeDescription = string.Empty;
            ResponseCode = string.Empty;
            ResponseCodeDescription = string.Empty;
            Messages = new List<Message>();
            ReferenceNumber = string.Empty;
            ReferenceNumber2 = string.Empty;
            BalanceAvailable = 0;
            PointsAvailable = 0;
        }

        /// <summary>Gets or sets whether the payment operation succeeded.</summary>
        public bool Succeeded { get; set; }

        /// <summary>Gets or sets the address verification response.</summary>
        public AvsResponseType AvsCode { get; set; }
        /// <summary>Gets or sets the human-readable address verification response.</summary>
        public string AvsCodeDescription { get; set; }
        /// <summary>Gets or sets the card security-code verification response.</summary>
        public CvnResponseType CvvCode { get; set; }
        /// <summary>Gets or sets the human-readable card security-code verification response.</summary>
        public string CvvCodeDescription { get; set; }
        /// <summary>Gets or sets the provider response code.</summary>
        public string ResponseCode { get; set; }
        /// <summary>Gets or sets the human-readable provider response description.</summary>
        public string ResponseCodeDescription { get; set; }
        /// <summary>Gets or sets the messages returned by the payment operation.</summary>
        /// <seealso cref="Warnings"/>
        /// <seealso cref="Errors"/>
        public List<Message> Messages { get; set; }
        /// <summary>Gets or sets the primary provider reference number.</summary>
        public string ReferenceNumber { get; set; }
        /// <summary>Gets or sets the secondary provider reference number.</summary>
        public string ReferenceNumber2 { get; set; }
        /// <summary>Gets or sets the available monetary balance returned by the provider.</summary>
        public decimal BalanceAvailable { get; set; }
        /// <summary>Gets or sets the available reward-point balance returned by the provider.</summary>
        public int PointsAvailable { get; set; }

        #region Helper Properties

        /// <summary>Gets the warning messages from <see cref="Messages"/>.</summary>
        public ReadOnlyCollection<Message> Warnings
        {
            get
            {
                var result = new List<Message>();
                foreach (var m in Messages)
                {
                    if (m.Severity == MessageType.Warning)
                    {
                        result.Add(m);
                    }
                }
                return result.AsReadOnly();
            }
        }

        /// <summary>Gets a value indicating whether <see cref="Warnings"/> contains any messages.</summary>
        public bool HasWarnings
        {
            get { return Warnings.Count > 0; }
        }

        /// <summary>Gets the error messages from <see cref="Messages"/>.</summary>
        public ReadOnlyCollection<Message> Errors
        {
            get
            {
                var result = new List<Message>();
                foreach (var m in Messages)
                {
                    if (m.Severity == MessageType.Error)
                    {
                        result.Add(m);
                    }
                }
                return result.AsReadOnly();
            }
        }

        /// <summary>Gets a value indicating whether <see cref="Errors"/> contains any messages.</summary>
        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }

        #endregion
    }
}
