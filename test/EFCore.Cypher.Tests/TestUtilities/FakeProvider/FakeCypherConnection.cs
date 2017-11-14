// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider
{
    public class FakeCypherConnection : RelationalConnection
    {
        private readonly List<FakeCypherDbConnection> _dbConnections = new List<FakeCypherDbConnection>();
        
        public FakeCypherConnection(
            IDbContextOptions options
        ) : base(
            new RelationalConnectionDependencies(
                options,
                new DiagnosticsLogger<DbLoggerCategory.Database.Transaction>(
                    new LoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("FakeDiagnosticListener")
                ),
                new DiagnosticsLogger<DbLoggerCategory.Database.Connection>(
                    new LoggerFactory(),
                    new LoggingOptions(),
                    new DiagnosticListener("FakeDiagnosticListener")
                ),
                new NamedConnectionStringResolver(options)
            )
        ) 
        {
        }

        /// <summary>
        /// Database connections
        /// </summary>
        public IReadOnlyList<FakeCypherDbConnection> DbConnections => _dbConnections;

        /// <summary>
        /// Create database connection
        /// </summary>
        /// <returns></returns>
        protected override DbConnection CreateDbConnection()
        {
            var connection = new FakeCypherDbConnection(ConnectionString);
            _dbConnections.Add(connection);

            return connection;
        }
    }
}