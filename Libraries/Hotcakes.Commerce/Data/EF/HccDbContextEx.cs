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
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace Hotcakes.Commerce.Data.EF
{
    /// <summary>
    /// Provides the manually defined Entity Framework context constructors used by Hotcakes Commerce.
    /// </summary>
    [Serializable]
    public partial class HccDbContext
    {
        /// <summary>
        /// Initializes a context using a named connection string or connection string.
        /// </summary>
        /// <param name="nameOrConnectionString">The connection string or the name of a connection string in application configuration.</param>
        public HccDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// Initializes a context over an existing database connection.
        /// </summary>
        /// <param name="existingConnection">The existing database connection.</param>
        /// <param name="contextOwnsConnection">A value indicating whether disposing the context also disposes the connection.</param>
        /// <remarks>
        /// The ownership value is passed unchanged to the Entity Framework base context and controls the
        /// lifetime relationship between this context and <paramref name="existingConnection"/>.
        /// </remarks>
        public HccDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        /// <summary>
        /// Initializes a context over an existing object context.
        /// </summary>
        /// <param name="objectContext">The existing Entity Framework object context.</param>
        /// <param name="dbContextOwnsObjectContext">A value indicating whether this context owns the object context.</param>
        /// <remarks>
        /// The ownership value is passed unchanged to the Entity Framework base context and controls whether
        /// disposing this context also disposes <paramref name="objectContext"/>.
        /// </remarks>
        public HccDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
        }

        /// <summary>
        /// Initializes a context using a named connection string or connection string and a compiled model.
        /// </summary>
        /// <param name="nameOrConnectionString">The connection string or the name of a connection string in application configuration.</param>
        /// <param name="model">The compiled Entity Framework model to use.</param>
        public HccDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        /// <summary>
        /// Initializes a context over an existing database connection and compiled model.
        /// </summary>
        /// <param name="existingConnection">The existing database connection.</param>
        /// <param name="model">The compiled Entity Framework model to use.</param>
        /// <param name="contextOwnsConnection">A value indicating whether disposing the context also disposes the connection.</param>
        /// <remarks>
        /// The connection ownership value is passed unchanged to the Entity Framework base context.
        /// </remarks>
        public HccDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }
    }
}
