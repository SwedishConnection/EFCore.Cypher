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