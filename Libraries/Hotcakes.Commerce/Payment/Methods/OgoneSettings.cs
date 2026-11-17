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
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Payment.Methods
{
    /// <summary>
    /// Stores configuration for the Ogone payment method.
    /// </summary>
    /// <remarks>
    /// The property names correspond to the exact method-setting keys used by
    /// the implementation. <see cref="HashAlgorithm"/> caches the parsed
    /// value for the lifetime of this settings instance.
    /// </remarks>
    [Serializable]
    public class OgoneSettings : MethodSettings
    {
        private OgoneHashAlgorithm? _hashAlgorithm;

        /// <summary>
        /// Gets or sets the Ogone payment service provider identifier stored
        /// under <c>PaymentServiceProviderId</c>.
        /// </summary>
        public string PaymentServiceProviderId
        {
            get { return GetSettingOrEmpty("PaymentServiceProviderId"); }
            set { AddOrUpdate("PaymentServiceProviderId", value); }
        }

        /// <summary>
        /// Gets or sets the hash algorithm stored under <c>HashAlgorithm</c>.
        /// </summary>
        /// <remarks>
        /// When no setting value exists, reading returns
        /// <see cref="OgoneHashAlgorithm.Sha1"/>. A stored non-empty value is
        /// parsed with <see cref="Enum.Parse(Type, string)"/> using the exact
        /// enum member name; the setter stores <see cref="Enum.ToString()"/>.
        /// </remarks>
        public OgoneHashAlgorithm HashAlgorithm
        {
            get
            {
                if (!_hashAlgorithm.HasValue)
                {
                    var hashAlgorithm = GetSettingOrEmpty("HashAlgorithm");
                    if (!string.IsNullOrEmpty(hashAlgorithm))
                        _hashAlgorithm = (OgoneHashAlgorithm) Enum.Parse(typeof (OgoneHashAlgorithm), hashAlgorithm);
                    else
                        _hashAlgorithm = OgoneHashAlgorithm.Sha1;
                }
                return _hashAlgorithm.Value;
            }
            set
            {
                _hashAlgorithm = value;
                AddOrUpdate("HashAlgorithm", value.ToString());
            }
        }

        /// <summary>Gets or sets the SHA-in pass phrase stored under <c>ShaInPassPhrase</c>.</summary>
        public string ShaInPassPhrase
        {
            get { return GetSettingOrEmpty("ShaInPassPhrase"); }
            set { AddOrUpdate("ShaInPassPhrase", value); }
        }

        /// <summary>Gets or sets the SHA-out pass phrase stored under <c>ShaOutPassPhrase</c>.</summary>
        public string ShaOutPassPhrase
        {
            get { return GetSettingOrEmpty("ShaOutPassPhrase"); }
            set { AddOrUpdate("ShaOutPassPhrase", value); }
        }

        /// <summary>Gets or sets the template page stored under <c>TemplatePage</c>.</summary>
        public string TemplatePage
        {
            get { return GetSettingOrEmpty("TemplatePage"); }
            set { AddOrUpdate("TemplatePage", value); }
        }

        /// <summary>Gets or sets the debug-mode flag stored under <c>DebugMode</c>.</summary>
        public bool DebugMode
        {
            get { return GetBoolSetting("DebugMode"); }
            set { SetBoolSetting("DebugMode", value); }
        }

        /// <summary>Gets or sets the developer-mode flag stored under <c>DeveloperMode</c>.</summary>
        public bool DeveloperMode
        {
            get { return GetBoolSetting("DeveloperMode"); }
            set { SetBoolSetting("DeveloperMode", value); }
        }
    }
}
