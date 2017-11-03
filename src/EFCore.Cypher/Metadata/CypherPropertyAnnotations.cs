// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        /// <summary>
        /// Annotations
        /// </summary>
        /// <returns></returns>
        protected virtual CypherAnnotations Annotations { get; }

        /// <summary>
        /// Wrapped property
        /// </summary>
        /// <returns></returns>
        protected virtual IProperty Property => (IProperty)Annotations.Metadata;

        /// <summary>
        /// Whether an exception shoudl be thrown if conflicting configuration is set
        /// </summary>
        protected virtual bool ShouldThrowOnConflict => true;

        /// <summary>
        /// Whether an exception should be thrown if invalid configuration is set
        /// </summary>
        protected virtual bool ShouldThrowOnInvalidConfiguration => true;

        /// <summary>
        /// Entity type cypher annotations
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected virtual CypherEntityTypeAnnotations GetAnnotations([NotNull] IEntityType entityType)
            => new CypherEntityTypeAnnotations(entityType);

        /// <summary>
        /// Property cypher annotations
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected virtual CypherPropertyAnnotations GetAnnotations([NotNull] IProperty property)
            => new CypherPropertyAnnotations(property);

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
        /// Default storage name, null if property is foreign key (i.e. not persisted)
        /// </summary>
        /// <returns></returns>
        private string GetDefaultStorageName() {
            return Property.IsForeignKey()
                ? null
                :Property.Name;
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

        /// <summary>
        /// Storage type
        /// </summary>
        /// <returns></returns>
        public virtual string StorageType {
            get => (string)Annotations.Metadata[CypherAnnotationNames.PropertyColumnType];

            [param: CanBeNull] 
            set => SetStorageType(value);
        }

        /// <summary>
        /// Set storage type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool SetStorageType([CanBeNull] string value)
            => Annotations.SetAnnotation(
                CypherAnnotationNames.PropertyColumnType,
                Check.NullButNotEmpty(value, nameof(value))
            );

        /// <summary>
        /// Default storage constriant
        /// </summary>
        /// <returns></returns>
        public string DefaultStorageConstraint {
            get => GetDefaultStorageConstraint(true);

            [param: CanBeNull]
            set => SetDefaultStorageConstraint(value);
        }

        /// <summary>
        /// Get default storage constraint
        /// </summary>
        /// <param name="fallback"></param>
        /// <returns></returns>
        protected virtual string GetDefaultStorageConstraint(bool fallback)
            => fallback
               && (GetDefaultValue(false) != null || GetComputedStorageConstraint(false) != null)
                ? null
                : (string)Annotations.Metadata[CypherAnnotationNames.DefaultStorageConstraint];

        /// <summary>
        /// Set default storage constraint
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool SetDefaultStorageConstraint([CanBeNull] string value) {
            if (!CanSetDefaultStorageConstraint(value)) {
                return false;
            }

            if (!ShouldThrowOnConflict 
                && DefaultStorageConstraint != value 
                && (!(value is null))) {
                ClearAllServerGeneratedValues();
            }

            return Annotations.SetAnnotation(
                CypherAnnotationNames.DefaultStorageConstraint,
                Check.NullButNotEmpty(value, nameof(value))
            );
        }

        protected virtual bool CanSetDefaultStorageConstraint([CanBeNull] string value)
        {
            if (GetDefaultStorageConstraint(false) == value) {
                return true;
            }

            if (!Annotations.CanSetAnnotation(
                    CypherAnnotationNames.DefaultStorageConstraint,
                    Check.NullButNotEmpty(value, nameof(value)))
                ) {
                return false;
            }

            if (ShouldThrowOnConflict)
            {
                if (GetDefaultValue(false) != null)
                {
                    throw new InvalidOperationException(
                        CypherStrings.ConflictingStorageServerGeneration(
                            nameof(DefaultStorageConstraint), 
                            Property.Name, 
                            nameof(DefaultValue)
                        )
                    );
                }
                if (GetComputedStorageConstraint(false) != null)
                {
                    throw new InvalidOperationException(
                        CypherStrings.ConflictingStorageServerGeneration(
                            nameof(DefaultStorageConstraint), 
                            Property.Name, 
                            nameof(ComputedStorageConstraint)
                        )
                    );
                }
            }
            else if (value != null
                     && (!CanSetDefaultValue(null) || !CanSetComputedStorageConstraint(null)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Default value
        /// </summary>
        /// <returns></returns>
        public object DefaultValue {
            get => GetDefaultValue(true);
            
            [param: CanBeNull]
            set => SetDefaultValue(value);
        }

        /// <summary>
        /// Get default value
        /// </summary>
        /// <param name="fallback"></param>
        /// <returns></returns>
        protected virtual object GetDefaultValue(bool fallback)
            => fallback
               && (GetDefaultStorageConstraint(false) != null || GetComputedStorageConstraint(false) != null)
                ? null
                : Annotations.Metadata[CypherAnnotationNames.DefaultValue];

        /// <summary>
        /// Set default value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool SetDefaultValue([CanBeNull] object value) {
            if (!(value is null) && value != DBNull.Value) {
                var vType = value.GetType();
                if (Property.ClrType.UnwrapNullableType() != vType) {
                    try {
                        value = Convert.ChangeType(
                            value,
                            Property.ClrType,
                            CultureInfo.InvariantCulture
                        );
                    } catch (Exception) {
                        throw new InvalidOperationException(
                            RelationalStrings.IncorrectDefaultValueType(
                                value, 
                                vType, 
                                Property.Name, 
                                Property.ClrType, 
                                Property.DeclaringEntityType.DisplayName()
                            )
                        );
                    }
                }

                if (vType.GetTypeInfo().IsEnum) {
                    value = Convert.ChangeType(
                        value, 
                        vType.UnwrapEnumType(), 
                        CultureInfo.InvariantCulture
                    );
                }
            }

            if (!CanSetDefaultValue(value))
            {
                return false;
            }

            if (!ShouldThrowOnConflict && DefaultValue != value && (!(value is null))) {
                ClearAllServerGeneratedValues();
            }

            return Annotations.SetAnnotation(
                CypherAnnotationNames.DefaultValue,
                value
            );
        }

        /// <summary>
        /// Can set default value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool CanSetDefaultValue([CanBeNull] object value) {
            if (GetDefaultValue(false) == value)
            {
                return true;
            }

            if (!Annotations.CanSetAnnotation(
                RelationalAnnotationNames.DefaultValue,
                value))
            {
                return false;
            }

            if (ShouldThrowOnConflict)
            {
                if (GetDefaultStorageConstraint(false) != null)
                {
                    throw new InvalidOperationException(
                        CypherStrings.ConflictingStorageServerGeneration(
                            nameof(DefaultValue), 
                            Property.Name, 
                            nameof(DefaultStorageConstraint)
                        )
                    );
                }
                if (GetComputedStorageConstraint(false) != null)
                {
                    throw new InvalidOperationException(
                        CypherStrings.ConflictingStorageServerGeneration(
                            nameof(DefaultValue), 
                            Property.Name, 
                            nameof(ComputedStorageConstraint)
                        )
                    );
                }
            }
            else if (value != null
                     && (!CanSetDefaultStorageConstraint(null) || !CanSetComputedStorageConstraint(null)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Computed storage constraint
        /// </summary>
        /// <returns></returns>
        public string ComputedStorageConstraint {
            get => GetComputedStorageConstraint(true);

            [param: CanBeNull]
            set => SetComputedStorageConstraint(value);
        }

        /// <summary>
        /// Get computed storage costraint
        /// </summary>
        /// <param name="fallback"></param>
        /// <returns></returns>
        protected virtual string GetComputedStorageConstraint(bool fallback)
            => fallback
               && (GetDefaultValue(false) != null || GetDefaultStorageConstraint(false) != null)
                ? null
                : (string)Annotations.Metadata[CypherAnnotationNames.ComputedStorageConstraint];

        /// <summary>
        /// Set computed storage constraint
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool SetComputedStorageConstraint([CanBeNull] string value)
        {
            if (!CanSetComputedStorageConstraint(value))
            {
                return false;
            }

            if (!ShouldThrowOnConflict
                && ComputedStorageConstraint != value
                && value != null)
            {
                ClearAllServerGeneratedValues();
            }

            return Annotations.SetAnnotation(
                CypherAnnotationNames.ComputedStorageConstraint,
                Check.NullButNotEmpty(value, nameof(value))
            );
        }

        /// <summary>
        /// Can set computed storage constraint
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool CanSetComputedStorageConstraint([CanBeNull] string value)
        {
            if (GetComputedStorageConstraint(false) == value)
            {
                return true;
            }

            if (!Annotations.CanSetAnnotation(
                CypherAnnotationNames.ComputedStorageConstraint,
                Check.NullButNotEmpty(value, nameof(value))))
            {
                return false;
            }

            if (ShouldThrowOnConflict)
            {
                if (GetDefaultValue(false) != null)
                {
                    throw new InvalidOperationException(
                        CypherStrings.ConflictingStorageServerGeneration(
                            nameof(ComputedStorageConstraint), 
                            Property.Name, 
                            nameof(DefaultValue)
                        )
                    );
                }
                if (GetDefaultStorageConstraint(false) != null)
                {
                    throw new InvalidOperationException(
                        CypherStrings.ConflictingStorageServerGeneration(
                            nameof(ComputedStorageConstraint), 
                            Property.Name, 
                            nameof(DefaultStorageConstraint)
                        )
                    );
                }
            }
            else if (value != null
                     && (!CanSetDefaultValue(null) || !CanSetDefaultStorageConstraint(null)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clear generated values
        /// </summary>
        protected virtual void ClearAllServerGeneratedValues()
        {
            SetDefaultValue(null);
            SetDefaultStorageConstraint(null);
            SetComputedStorageConstraint(null);
        }
    }
}