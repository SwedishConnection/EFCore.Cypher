// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Internal
{
    public static class CypherNonCapturingLazyInitializer {
        public static TValue EnsureInitialized<TParam, TValue>(
            [CanBeNull] ref TValue target,
            [CanBeNull] TParam param,
            [NotNull] Action<TParam> valueFactory)
            where TValue : class
        {
            if (Volatile.Read(ref target) != null)
            {
                return target;
            }

            valueFactory(param);

            return Volatile.Read(ref target);
        }
    }
}