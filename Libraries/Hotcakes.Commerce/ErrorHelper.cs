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

namespace Hotcakes.Commerce
{
    /// <summary>
    /// Throws <see cref="StoreException"/> instances carrying view message metadata.
    /// </summary>
    public class StoreExceptionHelper
    {
        /// <summary>
        /// Public API member: <c>public static void ShowError(string content)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public static void ShowError(string content)
        {
            ShowError(null, content);
        }

        /// <summary>
        /// Public API member: <c>public static void ShowInfo(string content)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public static void ShowInfo(string content)
        {
            ShowInfo(null, content);
        }

        /// <summary>
        /// Public API member: <c>public static void ShowWarning(string content)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public static void ShowWarning(string content)
        {
            ShowWarning(null, content);
        }

        /// <summary>
        /// Public API member: <c>public static void ShowSuccess(string content)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public static void ShowSuccess(string content)
        {
            ShowSuccess(null, content);
        }


        /// <summary>
        /// Public API member: <c>public static void ShowError(string header, string content)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public static void ShowError(string header, string content)
        {
            ShowMessage(header, content, ViewMessageType.Error);
        }

        /// <summary>
        /// Public API member: <c>public static void ShowInfo(string header, string content)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public static void ShowInfo(string header, string content)
        {
            ShowMessage(header, content, ViewMessageType.Info);
        }

        /// <summary>
        /// Public API member: <c>public static void ShowWarning(string header, string content)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public static void ShowWarning(string header, string content)
        {
            ShowMessage(header, content, ViewMessageType.Warning);
        }

        /// <summary>
        /// Public API member: <c>public static void ShowSuccess(string header, string content)</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public static void ShowSuccess(string header, string content)
        {
            ShowMessage(header, content, ViewMessageType.Success);
        }


        private static void ShowMessage(string header, string content, ViewMessageType type)
        {
            throw new StoreException(header, content, type);
        }
    }
}
