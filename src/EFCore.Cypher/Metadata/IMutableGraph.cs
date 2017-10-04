using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableGraph: IGraph, IMutableAnnotatable {

        IMutableEntity AddEntity([NotNull] string[] labels);

        IMutableRelationship AddRelationship([NotNull] string[] labels);

        IMutableEntity FindEntity([NotNull] string[] labels);

        IMutableRelationship FindRelationship([NotNull] string[] labels);

        IMutableNode RemoveEntity([NotNull] string[] labels);

        IMutableNode RemoveRelationship([NotNull] string[] labels);
    }
}