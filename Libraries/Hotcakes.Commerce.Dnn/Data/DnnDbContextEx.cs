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
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Reflection;
using BrandonHaynes.ModelAdapter.EntityFramework;
using DotNetNuke.Common.Utilities;

namespace Hotcakes.Commerce.Dnn.Data
{
    /// <summary>
    /// Extends the generated Entity Framework context with constructors and connection creation
    /// that honor the active DNN database owner and object qualifier settings.
    /// </summary>
    [Serializable]
    public partial class DnnDbContext
    {
        /// <summary>
        /// Initializes a context using a named connection string or connection string.
        /// </summary>
        /// <param name="nameOrConnectionString">The connection string or the name of a connection string in configuration.</param>
        public DnnDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// Initializes a context using an existing database connection.
        /// </summary>
        /// <param name="existingConnection">The database connection to use.</param>
        /// <param name="contextOwnsConnection">A value indicating whether the context should dispose the connection.</param>
        public DnnDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        /// <summary>
        /// Initializes a context from an existing Entity Framework object context.
        /// </summary>
        /// <param name="objectContext">The object context to wrap.</param>
        /// <param name="dbContextOwnsObjectContext">A value indicating whether this context should dispose the object context.</param>
        public DnnDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
        }

        /// <summary>
        /// Initializes a context using a named connection string or connection string and a compiled model.
        /// </summary>
        /// <param name="nameOrConnectionString">The connection string or the name of a connection string in configuration.</param>
        /// <param name="model">The compiled Entity Framework model to use.</param>
        public DnnDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        /// <summary>
        /// Initializes a context using an existing connection and a compiled model.
        /// </summary>
        /// <param name="existingConnection">The database connection to use.</param>
        /// <param name="model">The compiled Entity Framework model to use.</param>
        /// <param name="contextOwnsConnection">A value indicating whether the context should dispose the connection.</param>
        public DnnDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        /// <summary>
        /// Creates a context connected to the configured DNN database with the current table prefix and schema settings applied.
        /// </summary>
        /// <returns>A configured <see cref="DnnDbContext"/> instance.</returns>
        public static DnnDbContext Instantiate()
        {
            var connBuilder = new EntityConnectionStringBuilder();
            connBuilder.Provider = "System.Data.SqlClient";
            connBuilder.ProviderConnectionString = WebAppSettings.HccConnectionString;
            connBuilder.Metadata =
                @"res://*/Data.DnnDbContext.csdl|res://*/Data.DnnDbContext.ssdl|res://*/Data.DnnDbContext.msl";

            var objectQualifier = Config.GetObjectQualifer();
            var dataBaseOwner = Config.GetDataBaseOwner().TrimEnd('.');
            var connAdapter = new TablePrefixModelAdapter(objectQualifier, new TableSchemaModelAdapter(dataBaseOwner));
            var adapter = new ConnectionAdapter(connAdapter, Assembly.GetCallingAssembly());

            return new DnnDbContext(adapter.AdaptConnection(connBuilder.ConnectionString), true);
        }
    }
}
