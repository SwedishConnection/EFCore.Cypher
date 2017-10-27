using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    public class CypherReferenceNavigationBuilder : IInfrastructure<CypherInternalRelationshipBuilder>
    {
        public CypherReferenceNavigationBuilder(
            [NotNull] CypherEntity declaringEntity,
            [NotNull] CypherEntity relatedEntity,
            [CanBeNull] string navigationName,
            [NotNull] CypherInternalRelationshipBuilder builder
        ) {
            Check.NotNull(relatedEntity, nameof(relatedEntity));
            Check.NotNull(builder, nameof(builder));

            Builder = builder;
            DeclaringEntity = declaringEntity;
            RelatedEntity = relatedEntity;
            ReferenceName = navigationName;
        }

        public CypherReferenceNavigationBuilder(
            [NotNull] CypherEntity declaringEntity,
            [NotNull] CypherEntity relatedEntity,
            [CanBeNull] PropertyInfo navigationProperty,
            [NotNull] CypherInternalRelationshipBuilder builder
        ) {
            Check.NotNull(relatedEntity, nameof(relatedEntity));
            Check.NotNull(builder, nameof(builder));

            DeclaringEntity = declaringEntity;
            RelatedEntity = relatedEntity;
            ReferenceProperty = navigationProperty;
            ReferenceName = navigationProperty?.Name;
            Builder = builder;
        }

        private CypherInternalRelationshipBuilder Builder { get; }

        /// <summary>
        /// Referencing name
        /// </summary>
        /// <returns></returns>
        protected virtual string ReferenceName { get; }

        /// <summary>
        /// Referencing property info
        /// </summary>
        /// <returns></returns>
        protected virtual PropertyInfo ReferenceProperty { get; }

        /// <summary>
        /// Related entity
        /// </summary>
        /// <returns></returns>
        protected virtual CypherEntity RelatedEntity { get; }

        /// <summary>
        /// Declaring entity
        /// </summary>
        /// <returns></returns>
        protected virtual CypherEntity DeclaringEntity { get; }

        /// <summary>
        /// Internal builder
        /// </summary>
        public CypherInternalRelationshipBuilder Instance => Builder;
    }
}