using System;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class ClrTypeExtensions {

        public static string[] DisplayLabels([NotNull] this Type clrType, bool fullName = true) {
            return new string[] { clrType.DisplayName() };
        }
    }
}