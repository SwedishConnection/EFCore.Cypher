// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class CypherNotMappedMemberAttributeConvention : ICypherEntityAddedConvention
    {
        public CypherInternalEntityBuilder Apply([NotNull] CypherInternalEntityBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            var clrType = builder.Metadata.ClrType;
            if (clrType is null) {
                return builder;
            }

            var members = clrType.GetRuntimeProperties().Cast<MemberInfo>().Concat(clrType.GetRuntimeFields());

            foreach (var member in members)
            {
                var attributes = member.GetCustomAttributes<NotMappedAttribute>(inherit: true);
                if (attributes.Any())
                {
                    builder.Ignore(member.Name, ConfigurationSource.DataAnnotation);
                }
            }

            return builder;
        }
    }
}