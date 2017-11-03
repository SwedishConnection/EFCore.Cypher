// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class CypherValueGeneratorConvention : ValueGeneratorConvention, IPropertyAnnotationChangedConvention
    {
        public virtual Annotation Apply(InternalPropertyBuilder propertyBuilder, string name, Annotation annotation, Annotation oldAnnotation)
        {
            var property = propertyBuilder.Metadata;
            if (name == CypherAnnotationNames.DefaultValue
                || name == CypherAnnotationNames.DefaultStorageConstraint
                || name == CypherAnnotationNames.ComputedStorageConstraint)
            {
                propertyBuilder.ValueGenerated(GetValueGenerated(property), ConfigurationSource.Convention);
            }

            return annotation;
        }

        public override ValueGenerated? GetValueGenerated(Property property)
        {
            var valueGenerated = base.GetValueGenerated(property);
            if (valueGenerated != null)
            {
                return valueGenerated;
            }

            var cypherProperty = property.Cypher();
            return cypherProperty.ComputedStorageConstraint != null
                ? ValueGenerated.OnAddOrUpdate
                : cypherProperty.DefaultValue != null || cypherProperty.DefaultStorageConstraint != null
                    ? ValueGenerated.OnAdd
                    : (ValueGenerated?)null;
        }
    }
}