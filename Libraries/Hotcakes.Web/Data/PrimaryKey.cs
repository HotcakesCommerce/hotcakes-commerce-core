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

namespace Hotcakes.Web.Data
{
    [Serializable]
    public class PrimaryKey
    {
        public PrimaryKey(string bvin)
        {
            BvinValue = bvin;
            KeyType = PrimaryKeyType.Bvin;
            KeyName = "bvin";
        }

        public PrimaryKey(int id)
        {
            IntValue = id;
            KeyType = PrimaryKeyType.Integer;
            KeyName = "Id";
        }

        public PrimaryKey(long id)
            : this(id, "Id")
        {
        }

        public PrimaryKey(long id, string keyName)
        {
            LongValue = id;
            KeyType = PrimaryKeyType.Long;
            KeyName = keyName;
        }

        public PrimaryKey(Guid id)
            : this(id, "Id")
        {
        }

        public PrimaryKey(Guid id, string keyName)
        {
            GuidValue = id;
            KeyType = PrimaryKeyType.Guid;
            KeyName = keyName;
        }

        public PrimaryKeyType KeyType { get; private set; }
        public string BvinValue { get; private set; }
        public int IntValue { get; private set; }
        public long LongValue { get; private set; }
        public Guid GuidValue { get; private set; }

        public string KeyName { get; set; }

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