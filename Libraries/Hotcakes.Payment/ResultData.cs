#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
    [Serializable]
    public class ResultData
    {
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

        public bool Succeeded { get; set; }

        public AvsResponseType AvsCode { get; set; }
        public string AvsCodeDescription { get; set; }
        public CvnResponseType CvvCode { get; set; }
        public string CvvCodeDescription { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseCodeDescription { get; set; }
        public List<Message> Messages { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReferenceNumber2 { get; set; }
        public decimal BalanceAvailable { get; set; }
        public int PointsAvailable { get; set; }

        #region Helper Properties

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

        public bool HasWarnings
        {
            get { return Warnings.Count > 0; }
        }

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

        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }

        #endregion
    }
}