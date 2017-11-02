// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.Common;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public abstract class CypherOptionsExtension : IDbContextOptionsExtension
    {
        private string _logFragment;

        private DbConnection _connection;

        protected CypherOptionsExtension() {

        }

        protected CypherOptionsExtension(
            [NotNull] CypherOptionsExtension copyFrom
        ) {
            Check.NotNull(copyFrom, nameof(copyFrom));

            _connection = copyFrom._connection;
        }

        /// <summary>
        /// Adds the services required to make options work when there is no external
        /// <see cref="IServiceProvider" /> allowing database providers to register
        /// their required services during EF creation of a service provider.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public abstract bool ApplyServices(IServiceCollection services);

        /// <summary>
        /// Hash code createed from any options causing the need for a 
        /// new <see cref="IServiceProvider" />.
        /// </summary>
        /// <returns></returns>
        public virtual long GetServiceProviderHashCode() => 0;

        /// <summary>
        /// Gives the extension a chance to validate (exceptions should be thrown)
        /// </summary>
        /// <param name="options"></param>
        public virtual void Validate(IDbContextOptions options)
        {
        }

        /// <summary>
        /// Fragment (metadata/headers) for logging
        /// </summary>
        /// <returns></returns>
        public virtual string LogFragment
        {
            get {
                if (_logFragment is null) {
                    var builder = new StringBuilder();

                    // TODO: Append metadata

                    _logFragment = builder.ToString();
                }

                return _logFragment;
            }
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        protected abstract CypherOptionsExtension Clone();

        /// <summary>
        /// Database (<see ref="DbConnection" />) connection
        /// </summary>
        public virtual DbConnection Connection => _connection;

        /// <summary>
        /// Use database connection
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public virtual CypherOptionsExtension WithConnection([NotNull] DbConnection connection)
        {
            Check.NotNull(connection, nameof(connection));

            var clone = Clone();

            clone._connection = connection;

            return clone;
        }
    }
}