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
        /// Uses the return types (from the read only expression) to 
        /// create a value buffer factory (either typed or untyped)
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

        /// <summary>
        /// Visit read only expression
        /// </summary>
        /// <param name="readOnlyExpression"></param>
        /// <returns></returns>
        public Expression VisitReadOnly([NotNull] ReadOnlyExpression readOnlyExpression)
        {
            Check.NotNull(readOnlyExpression, nameof(readOnlyExpression));

            // TODO: nested read only expression

            if (readOnlyExpression.ReadingClauses.Count > 0) {
                IterateGrammer(readOnlyExpression.ReadingClauses);
            } else {
                CreatePseudoMatchClause();
            }

            if (readOnlyExpression.ReturnItems.Count > 0) {
                if (readOnlyExpression.IsReturnStar) {
                    _commandBuilder.Append(", ");
                }

                // TODO: Optimization visitors
                IterateGrammer(readOnlyExpression.ReturnItems, s => s.Append(", "));
            }

            // TODO: Order, Skip, Limit

            return readOnlyExpression;
        }

        /// <summary>
        /// Visit match expression
        /// </summary>
        /// <param name="matchExpression"></param>
        /// <returns></returns>
        public Expression VisitMatch([NotNull] MatchExpression matchExpression) {
            Check.NotNull(matchExpression, nameof(matchExpression));

            var optional = matchExpression.Optional
                ? "OPTIONAL"
                : String.Empty;

            _commandBuilder
                .Append($"{optional} MATCH (")
                .Append(matchExpression.Alias)
                .Append(":")
                .Append(String.Join(":", matchExpression.Labels))
                .Append(")");

            return matchExpression;
        }

        /// <summary>
        /// Visit storage expression
        /// </summary>
        /// <param name="storageExpression"></param>
        /// <returns></returns>
        public Expression VisitStorage([NotNull] StorageExpression storageExpression) {
            Check.NotNull(storageExpression, nameof(storageExpression));

            _commandBuilder
                .Append(SqlGenerator.DelimitIdentifier(storageExpression.Node.Alias))
                .Append(".")
                .Append(SqlGenerator.DelimitIdentifier(storageExpression.Name));

            return storageExpression;
        }

        /// <summary>
        /// Iterate over grammer visiting each term
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="handler"></param>
        protected virtual void IterateGrammer(
            [NotNull] IReadOnlyList<Expression> items,
            [CanBeNull] Action<IRelationalCommandBuilder> stringJoinAction = null
        ) => IterateGrammer(
                items, 
                e => Visit(e), 
                stringJoinAction
            );

        /// <summary>
        /// Iterate over grammer using the string join action between handled items
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="seed"></param>
        /// <param name="handler"></param>
        protected virtual void IterateGrammer<T>(
            [NotNull] IReadOnlyList<T> items,
            [NotNull] Action<T> handler,
            [CanBeNull] Action<IRelationalCommandBuilder> stringJoinAction = null
        ) {
            Check.NotNull(items, nameof(items));
            Check.NotNull(handler, nameof(handler));

            stringJoinAction = stringJoinAction ?? (s => s.AppendLine());

            for (var index = 0; index < items.Count; index++) {
                if (index > 0) {
                    stringJoinAction(_commandBuilder);
                }

                handler(items[index]);
            }
        }

        /// <summary>
        /// Let provider supply pseudo match clause
        /// </summary>
        protected virtual void CreatePseudoMatchClause() {}

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException();
        }
    }
}