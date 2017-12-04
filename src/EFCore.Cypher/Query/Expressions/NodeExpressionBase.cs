// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.Expressions {

    public abstract class NodeExpressionBase: Expression {

        private IQuerySource _querySource;

        private string _alias;

        protected NodeExpressionBase(
            [CanBeNull] IQuerySource querySource,
            [CanBeNull] string alias
        ) {
            _querySource = querySource;
            _alias = alias;
        }

        /// <summary>
        /// Third-party expression
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// Type
        /// </summary>
        /// <returns></returns>
        public override Type Type => typeof(object);

        public virtual IQuerySource QuerySource
        {
            get { return _querySource; }
            [param: NotNull]
            set
            {
                Check.NotNull(value, nameof(value));

                _querySource = value;
            }
        }

        /// <summary>
        /// Variable for node patterns and relationship details
        /// </summary>
        /// <returns></returns>
        public virtual string Alias
        {
            get { return _alias; }
            [param: NotNull]
            set
            {
                Check.NotNull(value, nameof(value));

                _alias = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="querySource"></param>
        /// <returns></returns>
        public virtual bool HandlesQuerySource([NotNull] IQuerySource querySource) {
            Check.NotNull(querySource, nameof(querySource));

            return _querySource == PreProcessQuerySource(querySource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="querySource"></param>
        /// <returns></returns>
        protected virtual IQuerySource PreProcessQuerySource([NotNull] IQuerySource querySource)
        {
            // TODO: Grouing

            // TODO: Additional from clause

            return querySource;
        }   

        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;
    }
}