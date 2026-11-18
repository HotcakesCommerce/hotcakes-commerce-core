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
using System.Web.UI;

namespace Hotcakes.Commerce.Content
{
    /// <summary>Provides a Web Forms user-control base that raises a click notification and reports handler errors.</summary>
    public class NotifyClickControl : UserControl
    {
        /// <summary>Occurs when the control notifies listeners of a click.</summary>
        public event EventHandler<ClickedEventArgs> Clicked;

        /// <summary>Raises <see cref="Clicked"/> with supplied information.</summary>
        /// <param name="data">Information passed to the event arguments.</param>
        /// <returns><see langword="true"/> unless an event handler sets <see cref="ClickedEventArgs.ErrorOccurred"/>.</returns>
        protected bool NotifyClicked(string data)
        {
            var args = new ClickedEventArgs(data);
            if (Clicked != null)
            {
                Clicked(this, args);
            }
            return !args.ErrorOccurred;
        }

        /// <summary>Raises <see cref="Clicked"/> without additional information.</summary>
        /// <returns><see langword="true"/> unless an event handler sets <see cref="ClickedEventArgs.ErrorOccurred"/>.</returns>
        protected bool NotifyClicked()
        {
            var args = new ClickedEventArgs();
            if (Clicked != null)
            {
                Clicked(this, args);
            }
            return !args.ErrorOccurred;
        }

        /// <summary>Contains information and error state for a <see cref="NotifyClickControl.Clicked"/> notification.</summary>
        public class ClickedEventArgs : EventArgs
        {
            private string _info = string.Empty;

            /// <summary>Initializes arguments with no information and no error.</summary>
            public ClickedEventArgs()
            {
                ErrorOccurred = false;
            }

            /// <summary>Initializes arguments with the supplied information and no error.</summary>
            /// <param name="data">Information to expose through <see cref="Info"/>; <see langword="null"/> leaves it empty.</param>
            public ClickedEventArgs(string data)
            {
                ErrorOccurred = false;
                if (data != null)
                {
                    _info = data;
                }
            }

            /// <summary>Gets or sets the information associated with the notification.</summary>
            public string Info
            {
                get { return _info; }
                set { _info = value; }
            }

            /// <summary>Gets or sets whether an event handler reported an error.</summary>
            public bool ErrorOccurred { get; set; }
        }
    }
}
