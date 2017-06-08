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
using System.Web.UI;

namespace Hotcakes.Commerce.Content
{
    public class NotifyClickControl : UserControl
    {
        public event EventHandler<ClickedEventArgs> Clicked;

        protected bool NotifyClicked(string data)
        {
            var args = new ClickedEventArgs(data);
            if (Clicked != null)
            {
                Clicked(this, args);
            }
            return !args.ErrorOccurred;
        }

        protected bool NotifyClicked()
        {
            var args = new ClickedEventArgs();
            if (Clicked != null)
            {
                Clicked(this, args);
            }
            return !args.ErrorOccurred;
        }

        public class ClickedEventArgs : EventArgs
        {
            private string _info = string.Empty;

            public ClickedEventArgs()
            {
                ErrorOccurred = false;
            }

            public ClickedEventArgs(string data)
            {
                ErrorOccurred = false;
                if (data != null)
                {
                    _info = data;
                }
            }

            public string Info
            {
                get { return _info; }
                set { _info = value; }
            }

            public bool ErrorOccurred { get; set; }
        }
    }
}