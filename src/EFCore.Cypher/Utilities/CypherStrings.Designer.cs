using System.Reflection;
using System.Resources;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Internal
{
    public static class CypherStrings {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("CypherStrings", typeof(CypherStrings).GetTypeInfo().Assembly);
        
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