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

namespace Hotcakes.Commerce.BusinessRules
{
    /// <summary>Represents a named message emitted by a workflow task.</summary>
    /// <remarks><see cref="TaskContext.GetCustomerVisibleErrors"/> uses <see cref="CustomerVisible"/> to select messages shown to customers.</remarks>
    public class WorkflowMessage
    {
        private string _Description = string.Empty;
        private string _Name = string.Empty;

        /// <summary>Initializes a message that is hidden from customers.</summary>
        public WorkflowMessage()
        {
            CustomerVisible = false;
        }

        /// <summary>Initializes a workflow message with its name, description, and customer visibility.</summary>
        /// <param name="messageName">The stable or display name of the message.</param>
        /// <param name="messageDescription">The descriptive message text.</param>
        /// <param name="showToCustomer"><c>true</c> to expose the message through customer-visible error filtering; otherwise, <c>false</c>.</param>
        public WorkflowMessage(string messageName, string messageDescription, bool showToCustomer)
        {
            CustomerVisible = showToCustomer;
            _Name = messageName;
            _Description = messageDescription;
        }

        /// <summary>Gets or sets whether this message may be shown to the customer.</summary>
        /// <value><c>true</c> when customer-facing code may include this message.</value>
        public bool CustomerVisible { get; set; }

        /// <summary>Gets or sets the message name.</summary>
        /// <value>The message name, or an empty string by default.</value>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>Gets or sets the descriptive message text.</summary>
        /// <value>The description, or an empty string by default.</value>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }
    }
}
