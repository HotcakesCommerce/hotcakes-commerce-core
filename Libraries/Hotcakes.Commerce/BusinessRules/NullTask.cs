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

namespace Hotcakes.Commerce.BusinessRules
{
    /// <summary>Represents the empty task used when no executable task is available.</summary>
    /// <remarks>This compatibility task deliberately fails execution, has no identifier, and reports successful rollback.</remarks>
    public class NullTask : Task
    {
        /// <summary>Gets the literal compatibility name <c>Null Task</c>.</summary>
        /// <returns><c>Null Task</c>.</returns>
        public override string TaskName()
        {
            return "Null Task";
        }

        /// <summary>Attempts to execute the null task.</summary>
        /// <param name="context">The workflow context; it is not modified.</param>
        /// <returns>Always <see langword="false"/>, so a workflow stops if this task is executed.</returns>
        public override bool Execute(TaskContext context)
        {
            return false;
        }

        /// <summary>Rolls back the null task.</summary>
        /// <param name="context">The workflow context; it is not modified.</param>
        /// <returns>Always <c>true</c>.</returns>
        public override bool Rollback(TaskContext context)
        {
            return true;
        }

        /// <summary>Gets the null task identifier.</summary>
        /// <returns>An empty string.</returns>
        public override string TaskId()
        {
            return string.Empty;
        }

        /// <summary>Creates another null task.</summary>
        /// <returns>A new <see cref="NullTask"/> instance.</returns>
        public override Task Clone()
        {
            var result = new NullTask();
            return result;
        }
    }
}
