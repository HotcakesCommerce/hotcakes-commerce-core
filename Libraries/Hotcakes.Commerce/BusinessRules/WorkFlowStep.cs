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

namespace Hotcakes.Commerce.BusinessRules
{
    /// <summary>Describes a persisted, ordered step in a configurable workflow.</summary>
    /// <remarks>The public type intentionally retains the historical <c>WorkFlow</c> capitalization for compatibility.</remarks>
    public class WorkFlowStep
    {
        private string _StepName = string.Empty;

        /// <summary>Initializes a workflow step with empty identifiers and a default sort order of <c>1</c>.</summary>
        public WorkFlowStep()
        {
            WorkFlowBvin = string.Empty;
            SortOrder = 1;
            ControlName = string.Empty;
            DisplayName = string.Empty;
            _StepName = string.Empty;
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>Gets or sets the step's unique business identifier.</summary>
        public string Bvin { get; set; }
        /// <summary>Gets or sets the UTC time when the step was last updated.</summary>
        public DateTime LastUpdated { get; set; }
        /// <summary>Gets or sets the identifier of the workflow containing this step.</summary>
        public string WorkFlowBvin { get; set; }
        /// <summary>Gets or sets the one-based display order of the step.</summary>
        public int SortOrder { get; set; }
        /// <summary>Gets or sets the control name associated with the step.</summary>
        public string ControlName { get; set; }
        /// <summary>Gets or sets the display label used when no explicit step name is set.</summary>
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the step name.</summary>
        /// <value>The explicit name, or <see cref="DisplayName"/> when the explicit name is empty.</value>
        public string StepName
        {
            get
            {
                if (_StepName == string.Empty)
                {
                    return DisplayName;
                }
                return _StepName;
            }
            set { _StepName = value; }
        }
    }
}
