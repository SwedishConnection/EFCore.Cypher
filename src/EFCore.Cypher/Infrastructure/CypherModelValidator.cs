// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public class CypherModelValidator: RelationalModelValidator {

        public CypherModelValidator(
            [NotNull] ModelValidatorDependencies dependencies,
            [NotNull] RelationalModelValidatorDependencies relationalDependencies
        ): base(dependencies, relationalDependencies) {

        }

        public override void Validate(IModel model)
        {
            base.Validate(model);

            ValidateSharedLabels(model);
        }

        protected virtual void ValidateSharedLabels([NotNull] IModel model) {
            // TODO: When labels can not be shared between different entity types
        }

        protected override void ValidateSharedTableCompatibility([NotNull] IModel model)
        {
            // TODO: Review what to keep and what to throw
        }

        protected override void ValidateInheritanceMapping([NotNull] IModel model)
        {
            // TODO: Review what to keep and what to throw
        }

        protected override void ValidateDefaultValuesOnKeys([NotNull] IModel model)
        {
            // TODO: Review what to keep and what to throw
        }
    }
}