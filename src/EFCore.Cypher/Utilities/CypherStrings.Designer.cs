using System.Reflection;
using System.Resources;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Utilities
{
    public static class CypherStrings {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager(
                typeof(CypherStrings).GetTypeInfo().Assembly.GetName().Name + ".Utilities.CypherStrings", 
                typeof(CypherStrings).GetTypeInfo().Assembly
            );
        
        /// <summary>
        ///     {conflictingConfiguration} cannot be set for '{property}', because {existingConfiguration} is already set.
        /// </summary>
        public static string ConflictingStorageServerGeneration(
            [CanBeNull] object conflictingConfiguration, 
            [CanBeNull] object property, 
            [CanBeNull] object existingConfiguration
        ) => string.Format(
                GetString(
                    "ConflictingStorageServerGeneration", 
                    nameof(conflictingConfiguration), 
                    nameof(property), 
                    nameof(existingConfiguration)
                ),
                conflictingConfiguration, 
                property, 
                existingConfiguration
            );

        /// <summary>
        ///     Duplicate relationship attribute on '{declaringEntity}.{declaringProperty}' and '{principalEntity}.{principalProperty}'
        /// </summary>
        public static string DuplicateRelationshipAttribute(
            [NotNull] object declaringEntity, 
            [NotNull] object declaringProperty,
            [NotNull] object principalEntity, 
            [NotNull] object principalProperty
        ) => string.Format(
                GetString(
                    "DuplicateRelationshipAttribute", 
                    nameof(declaringEntity), 
                    nameof(declaringProperty),
                    nameof(principalEntity),
                    nameof(principalProperty)
                ),
                declaringEntity, 
                declaringProperty,
                principalEntity,
                principalProperty
            );

        /// <summary>
        ///    {entity} is not a foreign key member [declaring entity: {declaringEntity}, principal entity: {principalEntity}]
        /// </summary>
        public static string NotAForeignKeyMember(
            [NotNull] object entity,
            [NotNull] object declaringEntity,
            [NotNull] object principalEntity
        ) => string.Format(
            GetString(
                "NotAForeignKeyMember",
                nameof(entity),
                nameof(declaringEntity),
                nameof(principalEntity)
            ),
            entity,
            declaringEntity,
            principalEntity
        );

        /// <summary>
        ///    Cypher-specific methods can only be used when the context is using a cypher database provider.
        /// </summary>
        public static string CypherNotInUse
            => GetString("CypherNotInUse");

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);
            for (var i = 0; i < formatterNames.Length; i++)
            {
                value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
            }
            return value;
        }

    }
}