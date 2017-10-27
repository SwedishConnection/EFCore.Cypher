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
            [CanBeNull] FieldInfo fieldInfo
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
    }
}