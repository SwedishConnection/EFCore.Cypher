// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Parsing;

namespace Microsoft.EntityFrameworkCore.Query.Cypher
{

    public class DefaultQueryCypherGenerator : ThrowingExpressionVisitor, ICypherExpressionVisitor, IQuerySqlGenerator
    {
        private IRelationalCommandBuilder _commandBuilder;

        private ParameterNameGenerator _parameterNameGenerator;

        private IReadOnlyDictionary<string, object> _parametersValues;

        protected DefaultQueryCypherGenerator(
            [NotNull] QuerySqlGeneratorDependencies dependencies,
            [NotNull] ReadOnlyExpression readOnlyExpression
        ) {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(readOnlyExpression, nameof(readOnlyExpression));

            Dependencies = dependencies;
            ReadOnlyExpression = readOnlyExpression;
        }

        /// <summary>
        /// Dependencies
        /// </summary>
        /// <returns></returns>
        protected virtual QuerySqlGeneratorDependencies Dependencies { get; }

        /// <summary>
        /// Read only expression
        /// </summary>
        /// <returns></returns>
        protected virtual ReadOnlyExpression ReadOnlyExpression { get; }

        /// <summary>
        /// Is query cacheable
        /// </summary>
        /// <returns></returns>
        public virtual bool IsCacheable { get; private set; }

        /// <summary>
        /// Sql (cypher) generation helper
        /// </summary>
        protected virtual ISqlGenerationHelper SqlGenerator => Dependencies.SqlGenerationHelper;

        /// <summary>
        /// Relational command from the read only expression
        /// </summary>
        /// <param name="IReadOnlyDictionary<string"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public IRelationalCommand GenerateSql([NotNull] IReadOnlyDictionary<string, object> parameterValues)
        {
            Check.NotNull(parameterValues, nameof(parameterValues));

            _commandBuilder = Dependencies.CommandBuilderFactory.Create();
            _parameterNameGenerator = Dependencies.ParameterNameGeneratorFactory.Create();
            _parametersValues = parameterValues;

            // TODO: null comparison transformation

            Visit(ReadOnlyExpression);

            return _commandBuilder.Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relationalValueBufferFactoryFactory"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public virtual IRelationalValueBufferFactory CreateValueBufferFactory(
            IRelationalValueBufferFactoryFactory relationalValueBufferFactoryFactory, 
            DbDataReader dataReader
        ) {
            Check.NotNull(relationalValueBufferFactoryFactory, nameof(relationalValueBufferFactoryFactory));

            return relationalValueBufferFactoryFactory
                .Create(
                    ReadOnlyExpression.GetReturnTypes().ToArray(), 
                    indexMap: null
                );
        }

        public Expression VisitReadOnly([NotNull] ReadOnlyExpression readOnlyExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException();
        }
    }
}