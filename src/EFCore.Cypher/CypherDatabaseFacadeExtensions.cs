// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
    public static class CypherDatabaseFacadeExtensions
    {

        public static int ExecuteCypherCommand(
            [NotNull] this DatabaseFacade databaseFacade,
            CypherRawString cypher,
            [NotNull] params object[] parameters
        ) => ExecuteCypherCommand(databaseFacade, cypher, (IEnumerable<object>)parameters);

        public static int ExecuteCypherCommand(
            [NotNull] this DatabaseFacade databaseFacade,
            [NotNull] FormattableString cypher)
            => ExecuteCypherCommand(databaseFacade, cypher.Format, cypher.GetArguments());

        public static int ExecuteCypherCommand(
            [NotNull] this DatabaseFacade databaseFacade,
            CypherRawString cypher,
            [NotNull] IEnumerable<object> parameters)
        {
            Check.NotNull(databaseFacade, nameof(databaseFacade));
            Check.NotNull(cypher, nameof(cypher));
            Check.NotNull(parameters, nameof(parameters));

            // from the EF Core (no relational implementation)
            var concurrencyDetector = databaseFacade.GetService<IConcurrencyDetector>();

            using (concurrencyDetector.EnterCriticalSection())
            {
                var command = databaseFacade
                    .GetService<IRawSqlCommandBuilder>()
                    .Build(cypher.Format, parameters);

                return command
                    .RelationalCommand
                    .ExecuteNonQuery(
                        databaseFacade.GetService<IRelationalConnection>(),
                        command.ParameterValues
                    );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseFacade"></param>
        /// <returns></returns>
        private static TService GetService<TService>(this IInfrastructure<IServiceProvider> databaseFacade)
        {
            Check.NotNull(databaseFacade, nameof(databaseFacade));

            var service = databaseFacade.Instance.GetService<TService>();
            if (service == null)
            {
                throw new InvalidOperationException(
                    CypherStrings.CypherNotInUse
                );
            }

            return service;
        }
    }
}