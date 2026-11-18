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

namespace Hotcakes.Commerce.Scheduling
{
    /// <summary>
    /// Specifies the interval, in minutes, at which a scheduled event runs.
    /// </summary>
    /// <remarks>
    /// The numeric values are minute counts used by scheduling configuration and are compatibility-sensitive.
    /// </remarks>
    public enum ScheduledEventFrequency
    {
        /// <summary>Runs every 5 minutes.</summary>
        Every5Minutes = 5,

        /// <summary>Runs every 30 minutes.</summary>
        Every30Minutes = 30,

        /// <summary>Runs every hour (60 minutes).</summary>
        EveryHour = 60,

        /// <summary>Runs every 6 hours (360 minutes).</summary>
        Every6Hours = 360,

        /// <summary>Runs every 12 hours (720 minutes).</summary>
        Every12Hours = 720,

        /// <summary>Runs once a day (1,440 minutes).</summary>
        OnceADay = 1440,

        /// <summary>Runs once a week (10,080 minutes).</summary>
        OnceAWeek = 10080
    }
}
