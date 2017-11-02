// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider
{
    public class FakeCypherCommandExecutor
    {
        private readonly Func<FakeCypherDbCommand, int> _executeNonQuery;
        private readonly Func<FakeCypherDbCommand, object> _executeScalar;
        private readonly Func<FakeCypherDbCommand, CommandBehavior, DbDataReader> _executeReader;
        private readonly Func<FakeCypherDbCommand, CancellationToken, Task<int>> _executeNonQueryAsync;
        private readonly Func<FakeCypherDbCommand, CancellationToken, Task<object>> _executeScalarAsync;
        private readonly Func<FakeCypherDbCommand, CommandBehavior, CancellationToken, Task<DbDataReader>> _executeReaderAsync;

        public FakeCypherCommandExecutor(
            Func<FakeCypherDbCommand, int> executeNonQuery = null,
            Func<FakeCypherDbCommand, object> executeScalar = null,
            Func<FakeCypherDbCommand, CommandBehavior, DbDataReader> executeReader = null,
            Func<FakeCypherDbCommand, CancellationToken, Task<int>> executeNonQueryAsync = null,
            Func<FakeCypherDbCommand, CancellationToken, Task<object>> executeScalarAsync = null,
            Func<FakeCypherDbCommand, CommandBehavior, CancellationToken, Task<DbDataReader>> executeReaderAsync = null)
        {
            _executeNonQuery = executeNonQuery
                               ?? (c => -1);

            _executeScalar = executeScalar
                             ?? (c => null);

            _executeReader = executeReader
                             ?? ((c, b) => new FakeCypherDbDataReader());

            _executeNonQueryAsync = executeNonQueryAsync
                                    ?? ((c, ct) => Task.FromResult(-1));

            _executeScalarAsync = executeScalarAsync
                                  ?? ((c, ct) => Task.FromResult<object>(null));

            _executeReaderAsync = executeReaderAsync
                                  ?? ((c, ct, b) => Task.FromResult<DbDataReader>(new FakeCypherDbDataReader()));
        }

        public virtual int ExecuteNonQuery(FakeCypherDbCommand command) => _executeNonQuery(command);

        public virtual object ExecuteScalar(FakeCypherDbCommand command) => _executeScalar(command);

        public virtual DbDataReader ExecuteReader(FakeCypherDbCommand command, CommandBehavior behavior)
            => _executeReader(command, behavior);

        public Task<int> ExecuteNonQueryAsync(FakeCypherDbCommand command, CancellationToken cancellationToken)
            => _executeNonQueryAsync(command, cancellationToken);

        public Task<object> ExecuteScalarAsync(FakeCypherDbCommand command, CancellationToken cancellationToken)
            => _executeScalarAsync(command, cancellationToken);

        public Task<DbDataReader> ExecuteReaderAsync(FakeCypherDbCommand command, CommandBehavior behavior, CancellationToken cancellationToken)
            => _executeReaderAsync(command, behavior, cancellationToken);
    }
}
