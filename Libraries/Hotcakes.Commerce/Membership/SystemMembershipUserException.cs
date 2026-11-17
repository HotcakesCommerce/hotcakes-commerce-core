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
using System.Runtime.Serialization;

namespace Hotcakes.Commerce.Membership
{
    /// <summary>
    /// Represents a membership-account failure together with its normalized <see cref="CreateUserStatus"/>.
    /// </summary>
    /// <remarks>The default status is <see cref="CreateUserStatus.UserRejected"/>. The exception preserves the standard <see cref="Exception"/> constructors and serialization shape.</remarks>
    public class SystemMembershipUserException : Exception
    {
        private CreateUserStatus _status = CreateUserStatus.UserRejected;

        /// <summary>Initializes an exception with the default <see cref="CreateUserStatus.UserRejected"/> status.</summary>
        public SystemMembershipUserException()
        {
        }

        /// <summary>Initializes an exception with a message and the default rejection status.</summary>
        /// <param name="message">The exception message.</param>
        public SystemMembershipUserException(string message) : base(message)
        {
        }

        /// <summary>Initializes an exception with a message and membership status.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="userstatus">The normalized membership outcome.</param>
        public SystemMembershipUserException(string message, CreateUserStatus userstatus) : base(message)
        {
            _status = userstatus;
        }

        /// <summary>Initializes an exception with a message, inner exception, and default rejection status.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The exception that caused this exception.</param>
        public SystemMembershipUserException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>Initializes an exception with a message, inner exception, and membership status.</summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The exception that caused this exception.</param>
        /// <param name="userstatus">The normalized membership outcome.</param>
        public SystemMembershipUserException(string message, Exception inner, CreateUserStatus userstatus)
            : base(message, inner)
        {
            _status = userstatus;
        }

        /// <summary>Initializes a serialized exception with the default rejection status.</summary>
        /// <param name="info">The serialized exception data.</param>
        /// <param name="context">The serialization context.</param>
        public SystemMembershipUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>Initializes a serialized exception with a membership status.</summary>
        /// <param name="info">The serialized exception data.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="userstatus">The normalized membership outcome.</param>
        public SystemMembershipUserException(SerializationInfo info, StreamingContext context,
            CreateUserStatus userstatus) : base(info, context)
        {
            _status = userstatus;
        }

        /// <summary>Gets or sets the normalized membership outcome associated with this exception.</summary>
        public CreateUserStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
