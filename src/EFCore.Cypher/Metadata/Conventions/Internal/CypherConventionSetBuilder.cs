// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public abstract class CypherConventionSetBuilder: IConventionSetBuilder {

        protected CypherConventionSetBuilder(
            [NotNull] RelationalConventionSetBuilderDependencies dependencies
        ) {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;
        }

        /// <summary>
        /// Access to type mappings (relinq), the database context and finder
        /// </summary>
        /// <returns></returns>
        protected virtual RelationalConventionSetBuilderDependencies Dependencies { get; }

        /// <summary>
        /// Replace or add cypher conventions
        /// </summary>
        /// <param name="conventionSet"></param>
        /// <returns></returns>
        public virtual ConventionSet AddConventions(ConventionSet conventionSet)
        {
            ValueGeneratorConvention valueGeneratorConvention = new CypherValueGeneratorConvention();
            var cypherStorageAttributeConvention = new CypherStorageAttributeConvention();

            ReplaceConvention(conventionSet.BaseEntityTypeChangedConventions, valueGeneratorConvention);
            ReplaceConvention(conventionSet.PrimaryKeyChangedConventions, valueGeneratorConvention);
            ReplaceConvention(conventionSet.ForeignKeyAddedConventions, valueGeneratorConvention);
            ReplaceConvention(conventionSet.ForeignKeyRemovedConventions, valueGeneratorConvention);

            conventionSet.EntityTypeAddedConventions.Add(new CypherLabelsAttributeConvention());
            conventionSet.PropertyAddedConventions.Add(cypherStorageAttributeConvention);
            conventionSet.PropertyFieldChangedConventions.Add(cypherStorageAttributeConvention);
            conventionSet.PropertyAnnotationChangedConventions.Add((CypherValueGeneratorConvention)valueGeneratorConvention);

            conventionSet.ModelBuiltConventions.Add(new RelationalTypeMappingConvention(Dependencies.TypeMapper));
            conventionSet.ModelAnnotationChangedConventions.Add(new RelationalDbFunctionConvention());

            conventionSet.ForeignKeyAddedConventions.Add(new CypherRelationshipAttributeConvention());

            return conventionSet;
        }

        protected virtual void ReplaceConvention<T1, T2>([NotNull] IList<T1> conventionsList, [NotNull] T2 newConvention)
            where T2 : T1
        {
            var oldConvention = conventionsList.OfType<T2>().FirstOrDefault();
            if (oldConvention == null)
            {
                throw new InvalidOperationException();
            }
            var index = conventionsList.IndexOf(oldConvention);
            conventionsList.RemoveAt(index);
            conventionsList.Insert(index, newConvention);
        }
    }
}