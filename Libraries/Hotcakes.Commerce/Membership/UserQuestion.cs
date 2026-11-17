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
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Hotcakes.Commerce.Membership
{
    /// <summary>
    /// Defines a store-scoped security question and, when applicable, its selectable answers.
    /// </summary>
    public class UserQuestion
    {
        /// <summary>
        /// Initializes a question with empty identifiers and name, a UTC timestamp, <see cref="UserQuestionType.FreeAnswer"/>, no options, and order zero.
        /// </summary>
        public UserQuestion()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdated = DateTime.UtcNow;
            Name = string.Empty;
            Type = UserQuestionType.FreeAnswer;
            Values = new Collection<UserQuestionOption>();
            Order = 0;
        }

        /// <summary>Gets or sets the question's string identifier.</summary>
        public string Bvin { get; set; }
        /// <summary>Gets or sets the identifier of the store that owns the question.</summary>
        public long StoreId { get; set; }
        /// <summary>Gets or sets the UTC timestamp for the last model update.</summary>
        public DateTime LastUpdated { get; set; }
        /// <summary>Gets or sets the question text or display name.</summary>
        public string Name { get; set; }
        /// <summary>Gets or sets whether the question is free-answer or multiple-choice.</summary>
        public UserQuestionType Type { get; set; }
        /// <summary>Gets the selectable options associated with the question.</summary>
        /// <remarks>The collection is replaced when <see cref="ReadValuesFromXml(string)"/> is called.</remarks>
        public Collection<UserQuestionOption> Values { get; private set; }
        /// <summary>Gets or sets the display order within the owning store's questions.</summary>
        public int Order { get; set; }

        /// <summary>
        /// Reads question options from serialized XML.
        /// </summary>
        /// <param name="xml">XML representing a <see cref="Collection{T}"/> of <see cref="UserQuestionOption"/> objects.</param>
        /// <remarks>
        /// The implementation deserializes using <see cref="XmlSerializer"/> and then assigns a new empty collection in its <c>finally</c> block, including after a successful deserialize. Consequently, this method currently leaves <see cref="Values"/> empty; that behavior is preserved for compatibility.
        /// </remarks>
        public void ReadValuesFromXml(string xml)
        {
            var xs = new XmlSerializer(typeof (Collection<UserQuestionOption>));
            var sr = new StringReader(xml);
            try
            {
                Values = (Collection<UserQuestionOption>) xs.Deserialize(sr);
            }
            finally
            {
                Values = new Collection<UserQuestionOption>();
                sr.Close();
            }
        }

        /// <summary>
        /// Serializes the current question options to XML.
        /// </summary>
        /// <returns>The XML produced by <see cref="XmlSerializer"/> for the current <see cref="Values"/> collection; an empty string if serialization does not assign a result.</returns>
        public string WriteValuesToXml()
        {
            var result = string.Empty;

            var xs = new XmlSerializer(typeof (Collection<UserQuestionOption>));
            var sw = new StringWriter();
            try
            {
                xs.Serialize(sw, Values);
                result = sw.ToString();
            }
            finally
            {
                sw.Close();
            }

            return result;
        }
    }
}
