using System;
using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Storage
{

    public interface ICypherParameterBuilder {

        IReadOnlyList<ICypherParameter> Parameters { get; }

        void AddParameter(
            [NotNull] string invariantName,
            [NotNull] string name);

        void AddParameter(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] GraphTypeMapping typeMapping,
            bool nullable);

        void AddParameter(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] INodeProperty property);

        void AddCompositeParameter(
            [NotNull] string invariantName,
            [NotNull] Action<ICypherParameterBuilder> buildAction);

        void AddPropertyParameter(
            [NotNull] string invariantName,
            [NotNull] string name,
            [NotNull] INodeProperty property);

        void AddRawParameter(
            [NotNull] string invariantName,
            [NotNull] DbParameter dbParameter);
    }
}