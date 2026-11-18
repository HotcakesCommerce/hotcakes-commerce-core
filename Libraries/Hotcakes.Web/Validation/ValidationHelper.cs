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
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hotcakes.Web.Validation
{
    /// <summary>Provides reusable validation methods that append failures to a list of <see cref="RuleViolation"/> objects.</summary>
    [Serializable]
    public class ValidationHelper
    {
        /// <summary>Gets the regular expression used by <see cref="IsEmailValid(string)"/>.</summary>
        public const string EmailValidationRegex = 
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z_]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z_])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        /// <summary>Adds a violation when a value is null, empty, or whitespace.</summary>
        /// <param name="errorMessage">The failure message.</param><param name="propertyValue">The value to check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void Required(string errorMessage, string propertyValue, List<RuleViolation> violations,
            string controlName)
        {
            Required(errorMessage, string.Empty, propertyValue, violations, controlName);
        }

        /// <summary>Adds a violation when a value is null, empty, or whitespace.</summary>
        /// <param name="errorMessage">The failure message.</param><param name="propertyName">The property name.</param><param name="propertyValue">The value to check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void Required(string errorMessage, string propertyName, string propertyValue,
            List<RuleViolation> violations, string controlName)
        {
            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                violations.Add(new RuleViolation(propertyName, propertyValue, errorMessage, controlName));
            }
        }

        /// <summary>Adds a violation when an integer is below the required minimum.</summary>
        /// <param name="minimum">The inclusive minimum value.</param><param name="errorMessage">The failure message.</param><param name="propertyValue">The value to check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void RequiredMinimum(int minimum, string errorMessage, int propertyValue,
            List<RuleViolation> violations, string controlName)
        {
            RequiredMinimum(minimum, errorMessage, string.Empty, propertyValue, violations, controlName);
        }

        /// <summary>Adds a violation when an integer is below the required minimum.</summary>
        /// <param name="minimum">The inclusive minimum value.</param><param name="errorMessage">The failure message.</param><param name="propertyName">The property name.</param><param name="propertyValue">The value to check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void RequiredMinimum(int minimum, string errorMessage, string propertyName, int propertyValue,
            List<RuleViolation> violations, string controlName)
        {
            if (propertyValue < minimum)
            {
                violations.Add(new RuleViolation(propertyName, propertyValue.ToString(), errorMessage, controlName));
            }
        }

        /// <summary>Adds a violation when an integer falls outside the inclusive range.</summary>
        /// <param name="minimum">The inclusive lower bound.</param><param name="maximum">The inclusive upper bound.</param><param name="errorMessage">The failure message.</param><param name="propertyName">The property name.</param><param name="propertyValue">The value to check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void RangeCheck(int minimum, int maximum, string errorMessage, string propertyName,
            int propertyValue, List<RuleViolation> violations, string controlName)
        {
            if (propertyValue < minimum || propertyValue > maximum)
            {
                violations.Add(new RuleViolation(propertyName,
                    propertyValue.ToString(),
                    errorMessage,
                    controlName));
            }
        }

        /// <summary>Adds a violation when the trimmed string length is outside the inclusive bounds.</summary>
        /// <param name="minimum">The inclusive minimum length.</param><param name="maximum">The inclusive maximum length.</param><param name="errorMessage">The failure message.</param><param name="propertyName">The property name.</param><param name="propertyValue">The value to check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void LengthCheck(int minimum, int maximum, string errorMessage, string propertyName,
            string propertyValue, List<RuleViolation> violations, string controlName)
        {
            if (propertyValue.Trim().Length < minimum)
            {
                violations.Add(new RuleViolation(propertyName,
                    propertyValue,
                    errorMessage,
                    controlName));
            }
            else
            {
                if (propertyValue.Trim().Length > maximum)
                {
                    violations.Add(new RuleViolation(propertyName,
                        propertyValue,
                        errorMessage,
                        controlName));
                }
            }
        }

        /// <summary>Adds a violation when the trimmed string exceeds the maximum length.</summary>
        /// <param name="maxmimum">The inclusive maximum length.</param><param name="errorMessage">The failure message.</param><param name="propertyName">The property name.</param><param name="propertyValue">The value to check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void MaxLength(int maxmimum, string errorMessage, string propertyName, string propertyValue,
            List<RuleViolation> violations, string controlName)
        {
            LengthCheck(0, maxmimum, errorMessage, propertyName, propertyValue, violations, controlName);
        }

        /// <summary>Adds a violation when a value is not a valid email address.</summary>
        /// <param name="errorMessage">The failure message.</param><param name="propertyValue">The value to check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void ValidEmail(string errorMessage, string propertyValue, List<RuleViolation> violations,
            string controlName)
        {
            ValidEmail(errorMessage, string.Empty, propertyValue, violations, controlName);
        }

        /// <summary>Adds a violation when a value is not a valid email address.</summary>
        /// <param name="errorMessage">The failure message.</param><param name="propertyName">The property name.</param><param name="propertyValue">The value to check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void ValidEmail(string errorMessage, string propertyName, string propertyValue,
            List<RuleViolation> violations, string controlName)
        {
            if (!IsEmailValid(propertyValue))
            {
                violations.Add(new RuleViolation(propertyName,
                    propertyValue,
                    errorMessage,
                    controlName));
            }
        }

        /// <summary>Adds a violation when a Boolean value is <see langword="false"/>.</summary>
        /// <param name="valueToCheck">The value to check.</param><param name="errorMessage">The failure message.</param><param name="propertyName">The property name.</param><param name="propertyValue">The value represented by the check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void ValidateTrue(bool valueToCheck, string errorMessage, string propertyName,
            string propertyValue, List<RuleViolation> violations, string controlName)
        {
            if (!valueToCheck)
            {
                violations.Add(new RuleViolation(propertyName,
                    propertyValue,
                    errorMessage,
                    controlName));
            }
        }

        /// <summary>Adds a violation when a Boolean value is <see langword="true"/>.</summary>
        /// <param name="valueToCheck">The value to check.</param><param name="errorMessage">The failure message.</param><param name="propertyName">The property name.</param><param name="propertyValue">The value represented by the check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void ValidateFalse(bool valueToCheck, string errorMessage, string propertyName,
            string propertyValue, List<RuleViolation> violations, string controlName)
        {
            if (valueToCheck)
            {
                violations.Add(new RuleViolation(propertyName,
                    propertyValue,
                    errorMessage,
                    controlName));
            }
        }

        /// <summary>Adds a violation when a decimal value is negative.</summary>
        /// <param name="errorMessage">The failure message.</param><param name="propertyName">The property name.</param><param name="propertyValue">The value to check.</param><param name="violations">The list receiving a violation.</param><param name="controlName">The associated control name.</param>
        public static void GreaterThanZero(string errorMessage, string propertyName, decimal propertyValue,
            List<RuleViolation> violations, string controlName)
        {
            if (propertyValue < 0)
            {
                violations.Add(new RuleViolation(propertyName, propertyValue.ToString(), errorMessage, controlName));
            }
        }

        /// <summary>Determines whether a value matches the configured email validation expression.</summary>
        /// <param name="email">The email value to check.</param>
        /// <returns><see langword="true"/> when the value is valid; otherwise, <see langword="false"/>.</returns>
        public static bool IsEmailValid(string email)
        {
            var tester = email.Trim().ToLower();
            if (tester.Length < 6)
                return false;

            return Regex.IsMatch(tester, EmailValidationRegex, RegexOptions.CultureInvariant);
        }
    }
}
