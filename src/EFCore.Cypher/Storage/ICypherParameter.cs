using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public interface ICypherParameter {

        string Name { get; }

        void AddDbParameter([NotNull] DbCommand command, [CanBeNull] object value);

        void AddDbParameter([NotNull] DbCommand command, [NotNull] IReadOnlyDictionary<string, object> parameterValues);
    }
}