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
using Hotcakes.CommerceDTO.v1;

namespace Hotcakes.Commerce
{
    /// <summary>
    ///     Custom properties are used to store settings for objects that cannot be put in the user interface.
    /// </summary>
    [Serializable]
    public class CustomProperty
    {
        /// <summary>Initializes an empty custom property with empty developer, key, and value strings.</summary>
        public CustomProperty()
        {
        }

        /// <summary>Initializes a custom property with its owner, key, and value.</summary>
        /// <param name="devId">The developer or integration identifier.</param><param name="propertyKey">The property key.</param><param name="propertyValue">The property value.</param>
        public CustomProperty(string devId, string propertyKey, string propertyValue)
        {
            _DeveloperId = devId;
            _Key = propertyKey;
            _Value = propertyValue;
        }

        /// <summary>
        ///     This is a unique ID that you use to group your own properties together.
        /// </summary>
        /// <remarks>Use any value that you wish here as long as it is unique to your company and you use it consistently.</remarks>
        /// <value>The identifier of the developer or integration that owns the property.</value>
        public string DeveloperId
        {
            get { return _DeveloperId; }
            set { _DeveloperId = value; }
        }

        /// <summary>
        ///     This is the unique name that is used to store and retrieve the custom property.
        /// </summary>
        /// <value>The case-insensitive lookup key for the property.</value>
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        /// <summary>
        ///     This is the information that you are saving to later retrieve.
        /// </summary>
        /// <remarks>You are not limited in the number of characters that you can use.</remarks>
        /// <value>The stored property value.</value>
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        /// <summary>
        ///     Allows you to retrieve the value of a custom property already converted into an integer.
        /// </summary>
        /// <returns>If unsuccessful, 0 is returned.</returns>
        /// <remarks>Parsing uses the runtime integer format; an unparsable value, including an empty value, returns zero.</remarks>
        public int GetValueAsInt()
        {
            var result = 0;
            if (int.TryParse(Value, out result))
            {
                return result;
            }
            return 0;
        }

        /// <summary>
        ///     Allows you to assign an integer to the Value property without having to convert it.
        /// </summary>
        /// <param name="value">Integer - the value to assign to the Value property</param>
        /// <remarks>The integer is converted with <see cref="int.ToString()"/> before assignment.</remarks>
        public void SetValueAsInt(int value)
        {
            Value = value.ToString();
        }

        /// <summary>
        ///     This method allows you to create a copy of the current custom property.
        /// </summary>
        /// <returns>A copy of this custom property</returns>
        public CustomProperty Clone()
        {
            return new CustomProperty(DeveloperId, Key, Value);
        }

        #region Private Members

        private string _DeveloperId = string.Empty;
        private string _Key = string.Empty;
        private string _Value = string.Empty;

        #endregion

        #region DTO

        /// <summary>
        ///     Allows you to populate the current custom property object using a CustomPropertyDTO instance
        /// </summary>
        /// <param name="dto">An instance of the custom property from the REST API</param>
        /// <remarks>The DTO's developer identifier, key, and value replace the current values.</remarks>
        public void FromDto(CustomPropertyDTO dto)
        {
            DeveloperId = dto.DeveloperId;
            Key = dto.Key;
            Value = dto.Value;
        }

        /// <summary>
        ///     Allows you to convert the current custom property object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of CustomPropertyDTO</returns>
        /// <remarks>The returned DTO contains the current developer identifier, key, and value.</remarks>
        public CustomPropertyDTO ToDto()
        {
            var dto = new CustomPropertyDTO();
            dto.Value = Value;
            dto.Key = Key;
            dto.DeveloperId = DeveloperId;

            return dto;
        }

        #endregion
    }
}
