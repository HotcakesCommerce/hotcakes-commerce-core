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

namespace Hotcakes.Web.Validation
{
    /// <summary>Describes a single validation failure.</summary>
    [CLSCompliant(true)]
    [Serializable]
    public class RuleViolation
    {
        /// <summary>Initializes an empty validation failure.</summary>
        public RuleViolation()
        {
        }

        /// <summary>Initializes a validation failure without a control name.</summary>
        /// <param name="name">The name of the invalid property.</param><param name="value">The invalid property value.</param><param name="message">The message describing the failure.</param>
        public RuleViolation(string name, string value, string message)
        {
            PropertyName = name;
            PropertyValue = value;
            ErrorMessage = message;
            ControlName = string.Empty;
        }

        /// <summary>Initializes a validation failure and associates it with a control.</summary>
        /// <param name="name">The name of the invalid property.</param><param name="value">The invalid property value.</param><param name="message">The message describing the failure.</param><param name="controlName">The name of the associated control.</param>
        public RuleViolation(string name, string value, string message, string controlName)
        {
            PropertyName = name;
            PropertyValue = value;
            ErrorMessage = message;
            ControlName = controlName;
        }

        /// <summary>Gets or sets the name of the invalid property.</summary>
        public string PropertyName { get; set; }
        /// <summary>Gets or sets the value that failed validation.</summary>
        public string PropertyValue { get; set; }
        /// <summary>Gets or sets the name of the associated control.</summary>
        public string ControlName { get; set; }
        /// <summary>Gets or sets the message describing the validation failure.</summary>
        public string ErrorMessage { get; set; }
    }
}
