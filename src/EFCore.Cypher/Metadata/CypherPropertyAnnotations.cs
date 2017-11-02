// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class CypherPropertyAnnotations : ICypherPropertyAnnotations
    {
        public CypherPropertyAnnotations(
            [NotNull] IProperty property
        ): this(new CypherAnnotations(property)) {

        }

        protected CypherPropertyAnnotations(
            [NotNull] CypherAnnotations annotations
        ) {
            Annotations = annotations;
        }

        protected virtual CypherAnnotations Annotations { get; }

        protected virtual IProperty Property => (IProperty)Annotations.Metadata;

        /// <summary>
        /// Storage name
        /// </summary>
        /// <returns></returns>
        public virtual string StorageName {
            get => (string)Annotations.Metadata[CypherAnnotationNames.PropertyStorageName]
                ?? GetDefaultStorageName();

            [param: CanBeNull]
            set => SetStorageName(value);
        }

        /// <summary>
        /// Default storage name
        /// </summary>
        /// <returns></returns>
        private string GetDefaultStorageName() {
            return Property.Name;
        }

        /// <summary>
        /// Set storage name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool SetStorageName([CanBeNull] string value)
            => Annotations.SetAnnotation(
                CypherAnnotationNames.PropertyStorageName,
                Check.NullButNotEmpty(value, nameof(value))
            );

        public virtual string StorageType {
            get => (string)Annotations.Metadata[CypherAnnotationNames.PropertyColumnType];
                //?? Property.FindRelationalMapping()?.StoreType;

            [param: CanBeNull] 
            set => SetStorageType(value);
        }

        public string DefaultStorageConstraint => throw new System.NotImplementedException();

        public string ComputedStorageConstraint => throw new System.NotImplementedException();

        public object DefaultValue => throw new System.NotImplementedException();

        protected virtual bool SetStorageType([CanBeNull] string value)
            => Annotations.SetAnnotation(
                RelationalAnnotationNames.ColumnType,
                Check.NullButNotEmpty(value, nameof(value))
            );
    }
}