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

namespace Hotcakes.Web.Data
{
    /// <summary>
    /// Represents a repository key and its strongly typed value.
    /// </summary>
    [Serializable]
    public class PrimaryKey
    {
        /// <summary>
        /// Initializes a key containing a string bvin value.
        /// </summary>
        /// <param name="bvin">The bvin value.</param>
        public PrimaryKey(string bvin)
        {
            BvinValue = bvin;
            KeyType = PrimaryKeyType.Bvin;
            KeyName = "bvin";
        }

        /// <summary>
        /// Initializes a key containing an integer value.
        /// </summary>
        /// <param name="id">The integer key value.</param>
        public PrimaryKey(int id)
        {
            IntValue = id;
            KeyType = PrimaryKeyType.Integer;
            KeyName = "Id";
        }

        /// <summary>
        /// Initializes a long key using <c>Id</c> as the member name.
        /// </summary>
        /// <param name="id">The long key value.</param>
        public PrimaryKey(long id)
            : this(id, "Id")
        {
        }

        /// <summary>
        /// Initializes a long key with the specified member name.
        /// </summary>
        /// <param name="id">The long key value.</param>
        /// <param name="keyName">The name of the key member.</param>
        public PrimaryKey(long id, string keyName)
        {
            LongValue = id;
            KeyType = PrimaryKeyType.Long;
            KeyName = keyName;
        }

        /// <summary>
        /// Initializes a GUID key using <c>Id</c> as the member name.
        /// </summary>
        /// <param name="id">The GUID key value.</param>
        public PrimaryKey(Guid id)
            : this(id, "Id")
        {
        }

        /// <summary>
        /// Initializes a GUID key with the specified member name.
        /// </summary>
        /// <param name="id">The GUID key value.</param>
        /// <param name="keyName">The name of the key member.</param>
        public PrimaryKey(Guid id, string keyName)
        {
            GuidValue = id;
            KeyType = PrimaryKeyType.Guid;
            KeyName = keyName;
        }

        /// <summary>
        /// Gets the kind of value represented by this key.
        /// </summary>
        public PrimaryKeyType KeyType { get; private set; }

        /// <summary>
        /// Gets the string bvin value.
        /// </summary>
        public string BvinValue { get; private set; }

        /// <summary>
        /// Gets the integer value.
        /// </summary>
        public int IntValue { get; private set; }

        /// <summary>
        /// Gets the long value.
        /// </summary>
        public long LongValue { get; private set; }

        /// <summary>
        /// Gets the GUID value.
        /// </summary>
        public Guid GuidValue { get; private set; }

        /// <summary>
        /// Gets or sets the name of the key member.
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// Gets the key value in its underlying CLR type.
        /// </summary>
        /// <returns>The typed key value, or <see langword="null"/> for an unsupported key type.</returns>
        public object KeyAsObject()
        {
            switch (KeyType)
            {
                case PrimaryKeyType.Bvin:
                    return BvinValue;
                case PrimaryKeyType.Guid:
                    return GuidValue;
                case PrimaryKeyType.Integer:
                    return IntValue;
                case PrimaryKeyType.Long:
                    return LongValue;
            }
            return null;
        }
    }
}
