// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherForeignKeyBuilderAnnotations: CypherForeignKeyAnnotations {

        public CypherForeignKeyBuilderAnnotations(
            [NotNull] InternalRelationshipBuilder internalBuilder,
            ConfigurationSource configurationSource
        ) : base(new CypherAnnotationsBuilder(internalBuilder, configurationSource)) {
        }

        /// <summary>
        /// Annotations builder
        /// </summary>
        /// <returns></returns>
        protected new virtual CypherAnnotationsBuilder Annotations => (CypherAnnotationsBuilder)base.Annotations;

        /// <summary>
        /// Model
        /// </summary>
        protected virtual Model Model => Annotations
            .MetadataBuilder
            .ModelBuilder
            .Metadata;

        /// <summary>
        /// Set relationship by name with the starting Clr type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public virtual bool HasRelationship(
            [CanBeNull] string name,
            [CanBeNull] Type startingClrType
        ) => SetRelationship(
            Model.GetOrAddEntityType(name),
            Model.FindEntityType(startingClrType)
        );

        /// <summary>
        /// Set relationship by name with the starting (shadow) name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public virtual bool HasRelationship(
            [CanBeNull] string name,
            [CanBeNull] string startingName
        ) => SetRelationship(
            Model.GetOrAddEntityType(name),
            Model.FindEntityType(startingName)
        );

        /// <summary>
        /// Set relationship by Clr type with the starting Clr type
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public virtual bool HasRelationship(
            [CanBeNull] Type clrType,
            [CanBeNull] Type startingClrType
        ) => SetRelationship(
            Model.GetOrAddEntityType(clrType),
            Model.FindEntityType(startingClrType)
        );

        /// <summary>
        /// Set relationship by Clr type with the starting (shadow) name
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public virtual bool HasRelationship(
            [CanBeNull] Type clrType,
            [CanBeNull] string startingName
        ) => SetRelationship(
            Model.GetOrAddEntityType(clrType),
            Model.FindEntityType(startingName)
        );
    }
}