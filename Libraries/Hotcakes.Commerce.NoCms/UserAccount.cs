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
using Hotcakes.Web.Cryptography;

namespace Hotcakes.Commerce.NoCms
{
    [Serializable]
    public class UserAccount
    {
        private string _hashedPassword = string.Empty;

        public UserAccount()
        {
            Id = -1;
            Email = string.Empty;
            _hashedPassword = string.Empty;
            DateCreated = DateTime.UtcNow;
            Status = UserAccountStatus.Active;
            Salt = string.Empty;
            ResetKey = string.Empty;
        }

        public long Id { get; set; }
        public string Email { get; set; }

        public string HashedPassword
        {
            get { return _hashedPassword; }
            set
            {
                // Hash the password only if we have a salt value
                if (Salt.Trim() != string.Empty)
                {
                    _hashedPassword = EncryptPassword(value);
                }
                else
                {
                    _hashedPassword = value;
                }
            }
        }

        public DateTime DateCreated { get; set; }
        public UserAccountStatus Status { get; set; }
        public string Salt { get; set; }
        public string ResetKey { get; set; }

        public bool ResetPassword(string resetKey, string newPassword)
        {
            if (resetKey != ResetKey) return false;
            HashedPassword = newPassword;
            HashPasswordIfNeeded();
            return true;
        }

        public bool DoesPasswordMatch(string trialPassword)
        {
            if (Salt == string.Empty)
            {
                return HashedPassword.Equals(trialPassword, StringComparison.InvariantCulture);
            }
            var hashedTrial = EncryptPassword(trialPassword);
            return HashedPassword.Equals(hashedTrial, StringComparison.InvariantCulture);
        }

        private string EncryptPassword(string password)
        {
            var result = string.Empty;
            result = Hashing.Md5Hash(password, Salt);
            return result;
        }

        internal void HashPasswordIfNeeded()
        {
            if (Salt.Trim() == string.Empty)
            {
                Salt = Guid.NewGuid().ToString();
                _hashedPassword = EncryptPassword(_hashedPassword);
            }
        }
    }
}