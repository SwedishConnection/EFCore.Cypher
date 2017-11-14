// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public class CypherModelCustomizer: RelationalModelCustomizer {
        public CypherModelCustomizer(
            [NotNull] ModelCustomizerDependencies dependencies
        ) : base(dependencies)
        {
        }

        protected override void FindSets(ModelBuilder modelBuilder, DbContext context)
        {
            foreach (var setInfo in Dependencies.SetFinder.FindSets(context))
            {
                modelBuilder.Entity(setInfo.ClrType);
            }
        }
    }
}