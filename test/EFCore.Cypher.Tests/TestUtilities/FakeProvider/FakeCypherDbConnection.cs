// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider
{
    public class FakeCypherDbConnection : DbConnection
    {
        private readonly FakeCypherCommandExecutor _commandExecutor;

        private ConnectionState _state;

        private readonly List<FakeCypherDbCommand> _dbCommands = new List<FakeCypherDbCommand>();

        private readonly List<FakeCypherDbTransaction> _dbTransactions = new List<FakeCypherDbTransaction>();


        public FakeCypherDbConnection (
            string connectionString,
            FakeCypherCommandExecutor commandExecutor = null,
            ConnectionState state = ConnectionState.Closed)
        {
            ConnectionString = connectionString;
            _commandExecutor = commandExecutor ?? new FakeCypherCommandExecutor();
            _state = state;
        }

        public override string ConnectionString { get; set; }

        public override string DataSource { get; } = "Fake DataSource";

        public override string Database { get; } = "Fake Database";

        public override string ServerVersion
        {
            get { throw new NotImplementedException(); }
        }

        public override ConnectionState State => _state;

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public int OpenCount { get; private set; }

        public override void Open()
        {
            OpenCount++;
            _state = ConnectionState.Open;
        }

        protected override DbCommand CreateDbCommand()
        {
            var command = new FakeCypherDbCommand(this, _commandExecutor);

            _dbCommands.Add(command);

            return command;
        }

        public int CloseCount { get; private set; }

        public override void Close()
        {
            CloseCount++;
            _state = ConnectionState.Closed;
        }

        public FakeCypherDbTransaction ActiveTransaction { get; set; }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            ActiveTransaction = new FakeCypherDbTransaction(this, isolationLevel);

            _dbTransactions.Add(ActiveTransaction);

            return ActiveTransaction;
        }

        public int DisposeCount { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeCount++;
            }

            base.Dispose(disposing);
        }
    }
}