// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public static class CypherModelExtensions {

        /// <summary>
        /// Relationships
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IEnumerable<ICypherRelationship> Relationships(this IModel model) {
            return model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())
                .Select(fk => fk.Cypher().Relationship);
        }

        /// <summary>
        /// Is Clr type a relationship
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static bool IsRelationship(
            this IModel model,
            Type clrType
        ) => model.Relationships()
            .Any(r => r.Relation.ClrType == clrType);

        /// <summary>
        /// Is name a relationship
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsRelationship(
            this IModel model,
            string name
        ) => model.Relationships()
            .Any(r => r.Relation.Name == name);

        /// <summary>
        /// Relationships starting with an entity type by name
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<ICypherRelationship> Starting(
            this IModel model,
            string name
        ) => model.Relationships()
            .Where(r => r.Starting.DisplayName() == name);

        /// <summary>
        /// Relationships starting with an entity type by Clr type
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static IEnumerable<ICypherRelationship> Starting(
            this IModel model,
            Type clrType
        ) => model.Relationships()
            .Where(r => r.Starting.ClrType == clrType);

        /// <summary>
        /// Relationships ending with an entity type by name
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<ICypherRelationship> Ending(
            this IModel model,
            string name
        ) => model.Relationships()
            .Where(r => r.Ending.DisplayName() == name);

        /// <summary>
        /// Relationships ending with an entity type by Clr type
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static IEnumerable<ICypherRelationship> Ending(
            this IModel model,
            Type clrType
        ) => model.Relationships()
            .Where(r => r.Ending.ClrType == clrType);
    }
}