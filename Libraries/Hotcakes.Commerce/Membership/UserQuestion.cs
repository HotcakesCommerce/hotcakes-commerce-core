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
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace Hotcakes.Commerce.Membership
{
    public class UserQuestion
    {
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

        public string Bvin { get; set; }
        public long StoreId { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Name { get; set; }
        public UserQuestionType Type { get; set; }
        public Collection<UserQuestionOption> Values { get; private set; }
        public int Order { get; set; }

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