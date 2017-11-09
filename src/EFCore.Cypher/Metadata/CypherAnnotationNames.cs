// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public static class CypherAnnotationNames {

        /// <summary>
        /// Prefix associated with cypher annotations
        /// </summary>
        public const string Prefix = "Cypher:";

        /// <summary>
        /// Labels
        /// </summary>
        public const string Labels = Prefix + "Labels";

        /// <summary>
        /// Storage name for properties
        /// </summary>
        public const string PropertyStorageName = Prefix + "PropertyStorageName";

        /// <summary>
        /// Storage type for properties
        /// </summary>
        public const string PropertyColumnType = Prefix + "PropertyStorageType";

        /// <summary>
        /// Default storage (property) constraint
        /// </summary>
        public const string DefaultStorageConstraint = Prefix + "DefaultStorageConstraint";

        /// <summary>
        /// Default storage (property) value
        /// </summary>
        public const string DefaultValue = Prefix + "DefaultValue";

        /// <summary>
        /// Computed storage (property) constraint
        /// </summary>
        public const string ComputedStorageConstraint = Prefix + "ComputedStorageConstraint";

        /// <summary>
        /// Relationship on foreign key
        /// </summary>
        public const string Relationship = Prefix + "Relationship";
    }
}