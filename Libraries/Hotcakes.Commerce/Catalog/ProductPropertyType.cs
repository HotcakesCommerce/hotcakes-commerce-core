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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This enumeration allows for ProductProperties to determine how to accept, store, and display product types and
    ///     their respective values.
    /// </summary>
    public enum ProductPropertyType
    {
        /// <summary>
        ///     Only used when the product property type is unknown.
        /// </summary>
        /// <remarks>This is currently not used in the application.</remarks>
        None = 0,

        /// <summary>
        ///     When specified, this type will adjust the UI to display a TextArea control. This is the default type used in the
        ///     application.
        /// </summary>
        TextField = 1,

        /// <summary>
        ///     This product property type renders the product properties in a select (drop down list) HTML element. This is
        ///     primarily used for Choices.
        /// </summary>
        MultipleChoiceField = 2,

        /// <summary>
        ///     The currency option is used when currency should be displayed to a customer. This product property type will cause
        ///     validation of the value as a decimal.
        /// </summary>
        CurrencyField = 3,

        /// <summary>
        ///     When specified, this type will cause the UI to render a TextBox for a date input as well as initiate validation to
        ///     ensure the value saved is a date.
        /// </summary>
        DateField = 4,

        /// <summary>
        ///     A TextBox will be rendered in the UI when this type is used, to allow for a URL to be entered.
        /// </summary>
        HyperLink = 7,

        /// <summary>
        ///     When used, this product property type will cause an upload feature to be rendered in the UI.
        /// </summary>
        /// <remarks>This type is not yet implemented at this time.</remarks>
        FileUpload = 6
    }
}