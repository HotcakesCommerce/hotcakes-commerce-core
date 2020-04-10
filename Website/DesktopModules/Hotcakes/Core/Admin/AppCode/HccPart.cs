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

using System;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public class HccPartEventArgs : EventArgs
    {
        private string _info = string.Empty;

        public HccPartEventArgs()
        {
        }

        public HccPartEventArgs(string data)
        {
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
    }

    [Serializable]
    public abstract class HccPart : HccUserControl
    {
        public virtual bool HasOwnButtons
        {
            get { return false; }
        }

        public event EventHandler<HccPartEventArgs> EditingComplete;

        protected void NotifyFinishedEditing()
        {
            if (EditingComplete != null)
            {
                EditingComplete(this, new HccPartEventArgs());
            }
        }

        protected void NotifyFinishedEditing(string info)
        {
            if (EditingComplete != null)
            {
                EditingComplete(this, new HccPartEventArgs(info));
            }
        }
    }
}