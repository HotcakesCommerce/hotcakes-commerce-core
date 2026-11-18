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

namespace Hotcakes.Commerce.Controls
{
    /// <summary>
    /// Provides a user control contract for applying control-entered changes to an item.
    /// </summary>
    /// <typeparam name="T">The type of item that the control reads or modifies.</typeparam>
    /// <remarks>
    /// The control inherits the Web Forms lifecycle from <see cref="ModificationControlBase"/>
    /// and does not define when <see cref="ApplyChanges(T)"/> is called. Derived controls
    /// implement the method using the values available at the time of the call.
    /// </remarks>
    public abstract class ModificationControl<T> : ModificationControlBase
    {
        /// <summary>
        /// Applies the current values held by the control to an item.
        /// </summary>
        /// <param name="item">The item to which the control's changes are applied.</param>
        /// <returns>The item resulting from applying the control's changes.</returns>
        /// <exception cref="System.Exception">A derived implementation may throw when the current control values cannot be applied.</exception>
        public abstract T ApplyChanges(T item);
    }
}
