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

namespace Hotcakes.Commerce.Marketing
{
    /// <summary>
    ///     This helps us determine which promotion qualifications
    ///     should be evaluated first. Lower cost qualifications
    ///     will be checked first so that if they fail, we never
    ///     run the expensive operations that might need to check
    ///     the database.
    /// </summary>
    public enum RelativeProcessingCost
    {
        /// <summary>Lowest relative cost; evaluated first.</summary>
        Lowest = 0,
        /// <summary>Lower-than-normal relative cost.</summary>
        Lower = 250,
        /// <summary>Default relative cost.</summary>
        Normal = 500,
        /// <summary>Higher-than-normal relative cost.</summary>
        Higher = 750,
        /// <summary>Highest relative cost; evaluated last.</summary>
        Highest = 1000
    }
}
