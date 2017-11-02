// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data;
using System.Data.Common;

namespace Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider
{
    public class FakeCypherDbTransaction : DbTransaction {
        public FakeCypherDbTransaction(
            FakeCypherDbConnection connection, 
            IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            DbConnection = connection;
            IsolationLevel = isolationLevel;
        }

        protected override DbConnection DbConnection { get; }

        public override IsolationLevel IsolationLevel { get; }

        public int CommitCount { get; private set; }

        public override void Commit()
        {
            CommitCount++;
        }

        public int RollbackCount { get; private set; }

        public override void Rollback()
        {
            RollbackCount++;
        }

        public int DisposeCount { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeCount++;

                ((FakeCypherDbConnection)DbConnection).ActiveTransaction = null;
            }

            base.Dispose(disposing);
        }
    }
}