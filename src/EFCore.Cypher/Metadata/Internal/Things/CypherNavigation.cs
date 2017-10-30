// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherNavigation : PropertyBase, IMutableNavigation
    {
        private PropertyIndexes _indexes;

        public CypherNavigation(
            [NotNull] string name,
            [CanBeNull] PropertyInfo propertyInfo,
            [CanBeNull] FieldInfo fieldInfo,
            [NotNull] CypherForeignKey foreignKey
        ): base(name, propertyInfo, fieldInfo) {
            
        }

        /// <summary>
        /// Clr type
        /// </summary>
        /// <returns></returns>
        public override Type ClrType => PropertyInfo?.PropertyType ?? typeof(object);

        public virtual CypherEntity DeclaringEntityType
            => throw new System.NotImplementedException();

        public new virtual CypherEntity DeclaringType => DeclaringEntityType;

        protected override void PropertyMetadataChanged() => DeclaringType.PropertyMetadataChanged();

        IMutableEntityType IMutableNavigation.DeclaringEntityType => DeclaringEntityType;

        public IMutableForeignKey ForeignKey => throw new System.NotImplementedException();

        public bool IsEagerLoaded { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        IEntityType INavigation.DeclaringEntityType => DeclaringEntityType;

        IForeignKey INavigation.ForeignKey => throw new System.NotImplementedException();

        bool INavigation.IsEagerLoaded => throw new System.NotImplementedException();

        /// <summary>
        /// Property indexes
        /// </summary>
        /// <returns></returns>
        public virtual PropertyIndexes PropertyIndexes
        {
            get
            {
                return NonCapturingLazyInitializer.EnsureInitialized(
                    ref _indexes, this,
                    property => property.DeclaringType.CalculateIndexes(property));
            }

            [param: CanBeNull]
            set
            {
                if (value == null)
                {
                    // This path should only kick in when the model is still mutable and therefore access does not need
                    // to be thread-safe.
                    _indexes = null;
                }
                else
                {
                    NonCapturingLazyInitializer.EnsureInitialized(ref _indexes, value);
                }
            }
        }

        /// <summary>
        /// Is compatible
        /// </summary>
        /// <param name="name"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="shouldBeCollection"></param>
        /// <param name="shouldThrow"></param>
        /// <returns></returns>
        public static bool IsCompatible(
            [NotNull] string name,
            [CanBeNull] PropertyInfo propertyInfo,
            [NotNull] CypherEntity start,
            [NotNull] CypherEntity end,
            bool? shouldBeCollection,
            bool shouldThrow
        ) {
            var endClrType = end.ClrType;
            if (endClrType is null) {
                if (shouldThrow) {
                    throw new InvalidOperationException(
                        CoreStrings.NavigationToShadowEntity(
                            name, 
                            start.DisplayName(), 
                            end.DisplayName()
                        )
                    );
                }

                return false;
            }

            return propertyInfo is null 
                || IsCompatible(propertyInfo, start.ClrType, endClrType, shouldBeCollection, shouldThrow);
        }

        /// <summary>
        /// Is compatible
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="startClrType"></param>
        /// <param name="endClrType"></param>
        /// <param name="shouldBeCollection"></param>
        /// <param name="shouldThrow"></param>
        /// <returns></returns>
        public static bool IsCompatible(
            [NotNull] PropertyInfo propertyInfo,
            [NotNull] Type startClrType,
            [NotNull] Type endClrType,
            bool? shouldBeCollection,
            bool shouldThrow
        ) {
            if (!propertyInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(startClrType.GetTypeInfo())) {
                if (shouldThrow) {
                    throw new InvalidOperationException(
                        CoreStrings.NoClrNavigation(
                            propertyInfo.Name, 
                            startClrType.ShortDisplayName()
                        )
                    );
                }

                return false;
            }

            var navigationEndClrType = propertyInfo.PropertyType.TryGetSequenceType();
            if (shouldBeCollection == false
                || navigationEndClrType is null
                || !navigationEndClrType.GetTypeInfo().IsAssignableFrom(endClrType.GetTypeInfo()))
            {
                if (shouldBeCollection == true)
                {
                    if (shouldThrow)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.NavigationCollectionWrongClrType(
                                propertyInfo.Name,
                                startClrType.ShortDisplayName(),
                                propertyInfo.PropertyType.ShortDisplayName(),
                                endClrType.ShortDisplayName()
                            )
                        );
                    }
                    return false;
                }

                if (!propertyInfo.PropertyType.GetTypeInfo().IsAssignableFrom(endClrType.GetTypeInfo()))
                {
                    if (shouldThrow)
                    {
                        throw new InvalidOperationException(
                            CoreStrings.NavigationSingleWrongClrType(
                                propertyInfo.Name,
                                startClrType.ShortDisplayName(),
                                propertyInfo.PropertyType.ShortDisplayName(),
                                endClrType.ShortDisplayName()));
                    }

                    return false;
                }
            }

            return true;
        }
    }
}