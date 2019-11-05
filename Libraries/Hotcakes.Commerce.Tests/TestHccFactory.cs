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
using System.Data.Entity.Core.EntityClient;
using System.IO;
using Effort;
using Effort.DataLoaders;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Dnn.Social;
using Hotcakes.Commerce.Social;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Tests
{
    public class TestHccFactory : DnnFactory
    {
        public TestHccFactory()
        {
            InitFromCsv = true;
            UseDbConnection = false;
        }

        public bool InitFromCsv { get; set; }
        public bool UseDbConnection { get; set; }

        public EntityConnection Connection { get; set; }

        public override ISocialService CreateSocialService(HccRequestContext context)
        {
            return new TestSocialService(context);
        }

        public override ILogger CreateEventLogger()
        {
            return new TestLogger();
        }

        //public override IRepoStrategy<T> CreateStrategy<T>()
        //{
        //	return new MemoryStrategy<T>();
        //}

        public override HccDbContext CreateHccDbContext()
        {
            if (UseDbConnection)
            {
                var connString = WebAppSettings.HccEFConnectionString;
                return new HccDbContext(connString);
            }

            if (Connection == null)
            {
                var connString = WebAppSettings.HccEFConnectionString;

                var csvDataPath = Path.Combine(Environment.CurrentDirectory, "CsvData");
                IDataLoader loader = null;
                if (InitFromCsv)
                    loader = new CsvDataLoader(csvDataPath);

                Connection = EntityConnectionFactory.CreateTransient(connString, loader);
            }
            return new HccDbContext(Connection, false);
        }

        public class TestLogger : ILogger
        {
            public void LogMessage(string message)
            {
                //throw new NotImplementedException();
            }

            public void LogMessage(string source, string message, EventLogSeverity severity)
            {
                //throw new NotImplementedException();
            }

            public void LogException(Exception ex)
            {
                //throw new NotImplementedException();
            }

            public void LogException(Exception ex, EventLogSeverity severity)
            {
                //throw new NotImplementedException();
            }
        }

        public class TestSocialService : SocialServiceBase
        {
            public TestSocialService(HccRequestContext context)
                : base(context)
            {
            }

            public override void UpdateJournalRecord(Category cat)
            {
                //base.UpdateJournalRecord(cat);
            }

            public override void UpdateJournalRecord(Product product)
            {
                //base.UpdateJournalRecord(product);
            }

            public override void UpdateJournalRecord(ProductReview review, Product product)
            {
                //base.UpdateJournalRecord(review, product);
            }
        }
    }
}