#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    [Serializable]
    public partial class DnnDbContext
    {
        public DnnDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DnnDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public DnnDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
        }

        public DnnDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        public DnnDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

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