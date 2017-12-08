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
        ///     Duplicate property {property} when creating an anonymous type.
        /// </summary>
        public static string DuplicatePropertyWithAnonymous(
            [NotNull] object property
        ) => string.Format(
            GetString(
                "DuplicatePropertyWithAnonymous",
                nameof(property)
            ),
            property
        );

        /// <summary>
        ///     Unexpected error when creating anonymous type.
        /// </summary>
        public static string BailCreatingAnonymous
            => GetString("BailCreatingAnonymous");

        /// <summary>
        ///     Anonymous types must have one or more properties.
        /// </summary>
        public static string NoPropertiesWhenCreatingAnonymous
            => GetString("NoPropertiesWhenCreatingAnonymous");

        /// <summary>
        ///     Property keys with anonymous types may not be null or whitespace.
        /// </summary>
        public static string PropertyNameMayNotBeNullOrWhitespace
            => GetString("PropertyNameMayNotBeNullOrWhitespace");

        /// <summary>
        ///     Property types with anonymous types may not be null.
        /// </summary>
        public static string PropertyTypeMayNotBeNull
            => GetString("PropertyTypeMayNotBeNull");

        /// <summary>
        ///     Failed to demote selectors in body clauses
        /// </summary>
        public static string UnableToDemoteSelectors
            => GetString("UnableToDemoteSelectors");

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