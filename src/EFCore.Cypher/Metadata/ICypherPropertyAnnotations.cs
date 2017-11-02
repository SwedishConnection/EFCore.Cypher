// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface ICypherPropertyAnnotations {

        string StorageName { get; }

        string StorageType { get; }

        string DefaultStorageConstraint { get; }

        string ComputedStorageConstraint { get; }

        object DefaultValue { get; }
    }
}