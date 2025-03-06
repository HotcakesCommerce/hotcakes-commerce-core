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

namespace Hotcakes.Commerce
{
    public class StoreExceptionHelper
    {
        public static void ShowError(string content)
        {
            ShowError(null, content);
        }

        public static void ShowInfo(string content)
        {
            ShowInfo(null, content);
        }

        public static void ShowWarning(string content)
        {
            ShowWarning(null, content);
        }

        public static void ShowSuccess(string content)
        {
            ShowSuccess(null, content);
        }


        public static void ShowError(string header, string content)
        {
            ShowMessage(header, content, ViewMessageType.Error);
        }

        public static void ShowInfo(string header, string content)
        {
            ShowMessage(header, content, ViewMessageType.Info);
        }

        public static void ShowWarning(string header, string content)
        {
            ShowMessage(header, content, ViewMessageType.Warning);
        }

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