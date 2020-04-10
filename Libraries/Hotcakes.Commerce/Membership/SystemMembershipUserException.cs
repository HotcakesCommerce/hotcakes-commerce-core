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
using System.Runtime.Serialization;

namespace Hotcakes.Commerce.Membership
{
    public class SystemMembershipUserException : Exception
    {
        private CreateUserStatus _status = CreateUserStatus.UserRejected;

        public SystemMembershipUserException()
        {
        }

        public SystemMembershipUserException(string message) : base(message)
        {
        }

        public SystemMembershipUserException(string message, CreateUserStatus userstatus) : base(message)
        {
            _status = userstatus;
        }

        public SystemMembershipUserException(string message, Exception inner) : base(message, inner)
        {
        }

        public SystemMembershipUserException(string message, Exception inner, CreateUserStatus userstatus)
            : base(message, inner)
        {
            _status = userstatus;
        }

        public SystemMembershipUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SystemMembershipUserException(SerializationInfo info, StreamingContext context,
            CreateUserStatus userstatus) : base(info, context)
        {
            _status = userstatus;
        }

        public CreateUserStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}