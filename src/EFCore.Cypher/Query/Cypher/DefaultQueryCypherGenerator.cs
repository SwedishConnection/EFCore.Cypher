// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
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

        private RelationalTypeMapping _typeMapping;

        protected virtual string AliasSeparator { get; } = " AS ";

        private static readonly Dictionary<ExpressionType, string> _operators = new Dictionary<ExpressionType, string> {
            // comparison
            { ExpressionType.Equal, " = "},
            { ExpressionType.NotEqual, " <> " },
            { ExpressionType.GreaterThan, " > " },
            { ExpressionType.GreaterThanOrEqual, " >= " },
            { ExpressionType.LessThan, " < " },
            { ExpressionType.LessThanOrEqual, " <= " },
            // boolean
            { ExpressionType.And, " AND " },
            { ExpressionType.Or, " OR " },
            // math
            { ExpressionType.Add, " + " },
            { ExpressionType.Subtract, " - " },
            { ExpressionType.Multiply, " * " },
            { ExpressionType.Divide, " / " },
            { ExpressionType.Modulo, " % " },
            { ExpressionType.ExclusiveOr, " ^ "}
        };

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
        /// Default true literal
        /// </summary>
        /// <returns></returns>
        protected virtual string TypedTrueLiteral => "true";

        /// <summary>
        /// Default false literal
        /// </summary>
        /// <returns></returns>
        protected virtual string TypedFalseLiteral => "false";

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

            // reading clauses
            if (readOnlyExpression.ReadingClauses.Count > 0) {
                IterateGrammer(readOnlyExpression.ReadingClauses);
            } else {
                CreatePseudoMatchClause();
            }


            // return items
            _commandBuilder.Append(" RETURN ");
            var returnItemsAdded = false;

            if (readOnlyExpression.IsReturnStar) {
                _commandBuilder
                    .Append(
                        SqlGenerator.DelimitIdentifier(
                            readOnlyExpression.ReturnStarNode.Alias
                        )
                    )
                    .Append(".*");

                returnItemsAdded = true;
            }

            if (readOnlyExpression.ReturnItems.Count > 0) {
                if (readOnlyExpression.IsReturnStar) {
                    _commandBuilder.Append(", ");
                }

                // TODO: Optimization visitors
                IterateGrammer(readOnlyExpression.ReturnItems, s => s.Append(", "));

                returnItemsAdded = true;
            }

            if (!returnItemsAdded) {
                _commandBuilder.Append("1");
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
                ? "OPTIONAL "
                : String.Empty;

            _commandBuilder
                .Append($"{optional}MATCH (")
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
        /// Visit alias expression
        /// </summary>
        /// <param name="aliasExpression"></param>
        /// <returns></returns>
        public virtual Expression VisitAlias(CypherAliasExpression aliasExpression)
        {
            Check.NotNull(aliasExpression, nameof(aliasExpression));

            Visit(aliasExpression.Expression);

            if (aliasExpression.Alias != null) {
                _commandBuilder
                    .Append(AliasSeparator)
                    .Append(SqlGenerator.DelimitIdentifier(aliasExpression.Alias));
            }

            return aliasExpression;
        }

        /// <summary>
        /// Visit conditional
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitConditional(ConditionalExpression expression) {
            Check.NotNull(expression, nameof(expression));

            // Boolean valid return type in Cypher otherwise use case
            if (expression.Type == typeof(bool)) {
                Visit(expression.Test);
            } else {
                _commandBuilder.AppendLine("CASE");

                using (_commandBuilder.Indent()) {
                    _commandBuilder.Append("WHEN ");

                    Visit(expression.Test);

                    _commandBuilder.AppendLine();
                    _commandBuilder.Append("THEN ");

                    // when true is bool or other
                    if (expression.IfTrue.RemoveConvert() is ConstantExpression constantIfTrue
                        && !(constantIfTrue.Value is null)
                        && constantIfTrue.Type.UnwrapNullableType() == typeof(bool) ) {
                        _commandBuilder.Append(
                            (bool)constantIfTrue.Value
                                ? TypedTrueLiteral
                                : TypedFalseLiteral
                        );
                    } else {
                        Visit(expression.IfTrue);
                    }

                    _commandBuilder.Append(" ELSE ");

                    // when false is bool or other
                    if (expression.IfFalse.RemoveConvert() is ConstantExpression constantIfFalse
                        && !(constantIfFalse.Value is null)
                        && constantIfFalse.Type.UnwrapNullableType() == typeof(bool) ) {
                        _commandBuilder.Append(
                            (bool)constantIfFalse.Value
                                ? TypedTrueLiteral
                                : TypedFalseLiteral
                        );
                    } else {
                        Visit(expression.IfFalse);
                    }

                    _commandBuilder.AppendLine();
                }

                _commandBuilder.Append("END");
            }

            return expression;
        }

        /// <summary>
        /// Visit binary
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitBinary(BinaryExpression expression)
        {
            Check.NotNull(expression, nameof(expression));

            switch(expression.NodeType) {
                case ExpressionType.Coalesce:
                    break;
                default:
                    var typeMapping = _typeMapping;
                    if (expression.IsComparisonOperation() 
                        || expression.NodeType == ExpressionType.Add) {
                        _typeMapping = FindTypeMapping(expression.Left)
                            ?? FindTypeMapping(expression.Right)
                            ?? typeMapping;
                    }

                    // left
                    var hasLeftParens = expression.Left.RemoveConvert() is BinaryExpression left
                        && left.NodeType != ExpressionType.Coalesce;

                    if (hasLeftParens) {
                        _commandBuilder.Append("(");
                    }

                    Visit(expression.Left);

                    if (hasLeftParens) {
                        _commandBuilder.Append(")");
                    }

                    _commandBuilder.Append(
                        VisitOperator(expression)
                    );

                    // right
                    var hasRightParens = expression.Right.RemoveConvert() is BinaryExpression right
                        && right.NodeType != ExpressionType.Coalesce;

                    if (hasLeftParens) {
                        _commandBuilder.Append("(");
                    }

                    Visit(expression.Right);

                    if (hasLeftParens) {
                        _commandBuilder.Append(")");
                    }

                    _typeMapping = typeMapping;

                    break;
            }

            return expression;
        }

        /// <summary>
        /// Visit constant
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitConstant(ConstantExpression expression) {
            Check.NotNull(expression, nameof(expression));

            var value = expression.Value;

            // grab underlying type
            if (expression.Type.UnwrapNullableType().IsEnum) {
                var underlyingType = expression.Type.UnwrapEnumType();
                value = Convert.ChangeType(value, underlyingType);
            }

            _commandBuilder.Append(
                value is null
                    ? "null"
                    : GetTypeMapping(value).GenerateSqlLiteral(value)
            );
            
            return expression;
        }

        /// <summary>
        /// Visit operator
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual string VisitOperator([NotNull] Expression expression) {
            switch (expression.NodeType) {
                case ExpressionType.Extension:
                    if (expression is StringCompareExpression stringCompareExpression) {
                        return GetOperator(stringCompareExpression.Operator);
                    }

                    goto default;
                default:
                    string symbol;
                    if (!TryGetOperator(expression.NodeType, out symbol)) {
                        throw new ArgumentOutOfRangeException();
                    }

                    return symbol;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        protected virtual string GetOperator(ExpressionType kind) => _operators[kind];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        protected virtual bool TryGetOperator(
            ExpressionType kind,
            [NotNull] out string symbol
        ) => _operators.TryGetValue(kind, out symbol);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unhandledItem"></param>
        /// <param name="visitMethod"></param>
        /// <returns></returns>
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
            => new NotImplementedException(visitMethod);

        /// <summary>
        /// Find (relational) type mapping from storage expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected virtual RelationalTypeMapping FindTypeMapping(
            [NotNull] Expression expression
        ) {
            switch(expression) {
                case StorageExpression storageExpression:
                    return Dependencies.RelationalTypeMapper.FindMapping(storageExpression.Property);
                case CypherAliasExpression aliasExpression:
                    return FindTypeMapping(aliasExpression.Expression);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private RelationalTypeMapping GetTypeMapping(object value) {
            return _typeMapping != null
                   && (value == null || _typeMapping.ClrType.IsInstanceOfType(value))
                ? _typeMapping
                : Dependencies.RelationalTypeMapper.GetMappingForValue(value);
        }
    }
}