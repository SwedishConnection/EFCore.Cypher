// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public interface ICypherForeignKeyUniqueChangedConvention {
        CypherInternalRelationshipBuilder Apply([NotNull] CypherInternalRelationshipBuilder Builder);
    }
}